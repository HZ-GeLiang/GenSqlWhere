using ExpressionToSqlWhereClause.Consts;
using ExpressionToSqlWhereClause.ExpressionTree.Adapter;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Helpers;
using ExpressionToSqlWhereClause.SqlFunc;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToSqlWhereClause.ExpressionTree;

/// <summary>
/// Where子句解析器
/// </summary>
internal static class WhereClauseParser
{
    internal static ClauseParserResult Parse(
        Expression body,
        Dictionary<string, string> aliasDict,
        ISqlAdapter sqlAdapter = default)
    {
        var adhesive = new WhereClauseAdhesive(sqlAdapter);

        if (body is BinaryExpression binaryExpression)
        {
            string whereClause = ParseBinaryExpression(adhesive, binaryExpression, aliasDict).ToString();

            return new ClauseParserResult()
            {
                WhereClause = $"({whereClause})",
                Adhesive = adhesive
            };
        }
        if (body is MethodCallExpression methodCallExpression)
        {
            var whereClause = ParseMethodCallExpression(adhesive, methodCallExpression);
            return new ClauseParserResult()
            {
                WhereClause = $"({whereClause.SqlClause})",
                Adhesive = adhesive
            };
        }
        if (body is UnaryExpression unaryExpression)// UnaryExpression : Expression
        {
            var pageResult = Parse(body.NodeType, body, adhesive, aliasDict);
            if (pageResult.NeedAddPara)
            {
                if (body.NodeType == ExpressionType.Not)
                {
                    //not 只支持 bool 类型
                    if (unaryExpression.Type == typeof(bool))
                    {
                        if (unaryExpression.Operand is MemberExpression operandMemberExpression ||
                            unaryExpression.Operand.GetType().Name == "LogicalBinaryExpression")
                        {
                            var val = ConstantExtractor.ParseConstant(unaryExpression.Operand);
                            pageResult.SqlClauseParametersInfo.Value = !Convert.ToBoolean(val);
                            pageResult.SqlClauseParametersInfo.Symbol = SqlKeys.NotEqual;

                            return new ClauseParserResult()
                            {
                                WhereClause = $"({pageResult.WhereClause})",
                                Adhesive = adhesive
                            };
                        }
                    }
                }
            }
            else
            {
                return new ClauseParserResult()
                {
                    WhereClause = $"({pageResult.WhereClause})",
                    Adhesive = adhesive
                };
            }
        }
        if (body is Expression)
        {
            throw new NotSupportedException("暂不支持Expression,修改程序");
        }
        if (body is MemberExpression)
        {
            throw new NotSupportedException("暂不支持MemberExpression,修改程序");
        }
        throw new NotSupportedException("暂不支持,修改程序");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="comparison"></param>
    /// <param name="expression">表达式</param>
    /// <param name="adhesive">胶粘剂</param>
    /// <param name="aliasDict"></param>
    /// <returns> </returns>
    public static ParseResult Parse(ExpressionType comparison, Expression expression, WhereClauseAdhesive adhesive, Dictionary<string, string> aliasDict)
    {
        if (expression is BinaryExpression binaryExpression)
        {
            var whereClause = ParseBinaryExpression(adhesive, binaryExpression, aliasDict);
            return new ParseResult() { WhereClause = whereClause };
        }
        if (expression is MethodCallExpression methodCallExpression)
        {
            var method = methodCallExpression.Method;
            if (method.DeclaringType == typeof(DbFunctions))
            {
                // int Month (datetime dt) => public static Int32 Month(DateTime dt)
                var methodInfo = methodCallExpression.Method;  //这个静态方法的定义

                var attrs = ReflectionHelper.GetAttributeForMethod<DbFunctionAttribute>(methodInfo);
                if (attrs.Length > 0)
                {
                    var leftName = ConstantExtractor.ParseMethodCallConstantExpression(methodCallExpression).ToString();//u.CreateAt
                    var symbol = ConditionBuilder.ToComparisonSymbol(comparison, methodCallExpression); //=

                    var attr = attrs[0];
                    string clauseLeft;
                    if (attr.FormatOnlyName) //目前为止, 这里永远为true
                    {
                        // u.CreateAt  变成  CreateAt
                        int startIndex = leftName.IndexOf(".", StringComparison.Ordinal);
                        var leftNameNew = startIndex != -1 ? leftName.Substring(startIndex + 1, leftName.Length - 1 - startIndex) : leftName;

                        clauseLeft = string.Format(attr.Format, leftNameNew);// Month(CreateAt)
                    }
                    else
                    {
                        clauseLeft = string.Format(attr.Format, leftName);// Month(u.CreateAt)
                    }

                    string parameterName = ConditionBuilder.EnsureParameter(methodInfo.Name, adhesive);
                    var parametersKey = $"@{parameterName}";
                    var param = adhesive.GetParameter(parametersKey);

                    param.Symbol = symbol;
                    if (symbol == SqlKeys.@in)
                    {
                        param.SqlClause = $"{clauseLeft} {symbol} ({parametersKey})";
                    }
                    else
                    {
                        param.SqlClause = $"{clauseLeft} {symbol} {parametersKey}";
                    }
                    param.IsDbFunction = true;

                    return new ParseResult()
                    {
                        WhereClause = new StringBuilder(param.SqlClause),
                        SqlClauseParametersInfo = param,
                        NeedAddPara = true,
                        MemberExpression = null,
                        MemberInfo = methodCallExpression.Method
                    };
                }
            }
            else
            {
                SqlClauseParametersInfo param = ParseMethodCallExpression(adhesive, methodCallExpression);
                return new ParseResult()
                {
                    WhereClause = new StringBuilder(param.SqlClause),
                    SqlClauseParametersInfo = param,
                };
            }
        }
        if (expression is MemberExpression memberExpression)
        {
            SqlClauseParametersInfo param = null;
            //m,[fn],bool值的处理
            PropertyInfo pi = memberExpression.Member as PropertyInfo;
            if (pi != null && pi.PropertyType == typeof(bool))
            {
                if (comparison == ExpressionType.AndAlso || comparison == ExpressionType.OrElse)
                {
                    param = ConditionBuilder.BuildCondition(memberExpression, memberExpression.Member, adhesive, ExpressionType.Equal);
                }
            }

            if (param == null)
            {
                param = ConditionBuilder.BuildCondition(memberExpression, memberExpression.Member, adhesive, comparison);

                if (pi?.PropertyType == typeof(bool?))
                {
                    //如果是判断符号是 != +类型是可空,  生成的语句中需要 IS NULL
                    if (param.Symbol == "<>")
                    {
                        param.SqlClause += $" OR ({param.Field} IS NULL)";
                    }
                }
            }

            return new ParseResult()
            {
                WhereClause = new StringBuilder(param.SqlClause),
                SqlClauseParametersInfo = param,
                NeedAddPara = true,
                MemberExpression = memberExpression,
                MemberInfo = memberExpression.Member
            };
        }
        if (expression is UnaryExpression unaryExpression)
        {
            if (unaryExpression.NodeType == ExpressionType.Not)
            {
                if (unaryExpression.Operand is MemberExpression operandMemberExpression)
                {
                    if (unaryExpression.Type == typeof(bool))
                    {
                        SqlClauseParametersInfo param = ConditionBuilder.BuildCondition(
                            operandMemberExpression,
                            operandMemberExpression.Member,
                            adhesive,
                            comparison);
                        return new ParseResult()
                        {
                            WhereClause = new StringBuilder(param.SqlClause),
                            SqlClauseParametersInfo = param,
                            NeedAddPara = true,
                            MemberExpression = null,
                            MemberInfo = operandMemberExpression.Member
                        };
                    }
                }
                else if (unaryExpression.Operand.GetType().Name == "LogicalBinaryExpression")
                {
                    var memberExpression2 = ((unaryExpression.Operand as BinaryExpression).Left as MemberExpression);
                    var member = memberExpression2.Member;
                    var param = ConditionBuilder.BuildCondition(
                        memberExpression2,
                        member,
                        adhesive,
                        comparison);
                    return new ParseResult()
                    {
                        WhereClause = new StringBuilder(param.SqlClause),
                        SqlClauseParametersInfo = param,
                        NeedAddPara = true,
                        MemberExpression = memberExpression2,
                        MemberInfo = member
                    };
                }
            }
            else if (unaryExpression.NodeType == ExpressionType.Convert)
            {
                if (unaryExpression.Operand is MemberExpression operandMemberExpression)
                {
                    var param = ConditionBuilder.BuildCondition(
                            operandMemberExpression,
                            operandMemberExpression.Member,
                            adhesive,
                            comparison);
                    return new ParseResult()
                    {
                        WhereClause = new StringBuilder(param.SqlClause),
                        SqlClauseParametersInfo = param,
                        NeedAddPara = true,
                        MemberExpression = null,
                        MemberInfo = operandMemberExpression.Member
                    };
                }
                else if (unaryExpression.Operand is System.Linq.Expressions.UnaryExpression operandUnaryExpression)
                {
                    //IssusDemo 的 类型问题
                    // 从嵌套的 UnaryExpression 中获取 MemberExpression
                    var nestedMemberExpression = operandUnaryExpression.Operand as MemberExpression;
                    if (nestedMemberExpression != null)
                    {
                        var param = ConditionBuilder.BuildCondition(
                            nestedMemberExpression,
                            nestedMemberExpression.Member,
                            adhesive,
                            comparison);

                        return new ParseResult()
                        {
                            WhereClause = new StringBuilder(param.SqlClause),
                            SqlClauseParametersInfo = param,
                            NeedAddPara = true,
                            MemberExpression = null,
                            MemberInfo = nestedMemberExpression.Member
                        };
                    }
                }
            }
        }


        //不支持的:
        //-expression  { Not(u.Name.Contains("Name"))}
        //System.Linq.Expressions.Expression { System.Linq.Expressions.UnaryExpression}
        DebuggerHelper.Break();
        throw new NotSupportedException($"Unknow expression {expression.GetType()}");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="adhesive">粘合剂</param>
    /// <param name="methodCallExpression"></param>
    /// <returns></returns>
    private static SqlClauseParametersInfo ParseMethodCallExpression(WhereClauseAdhesive adhesive, MethodCallExpression methodCallExpression)
    {
        var method = methodCallExpression.Method;

        if (method.DeclaringType == typeof(string))
        {
            if (method.Name is "Contains" or "StartsWith" or "EndsWith")
            {
                //like 的3种
                return ConditionBuilder.BuildLikeOrEqualCondition(methodCallExpression, adhesive);
            }
            if (method.Name == "Equals")
            {
                // equal
                //"Like" condition for string property, For example: u.Name.Contains("A")
                return ConditionBuilder.BuildLikeOrEqualCondition(methodCallExpression, adhesive);
            }
            if (method.Name == "IsNullOrEmpty")
            {
                var exp = methodCallExpression.Arguments.First();
                string msg = $"Please use ({exp} ?? \"\") != \"\" replace string.IsNullOrEmpty({exp}).";
                throw new Exception(msg);
            }
        }
        else if (method.DeclaringType == typeof(System.Linq.Enumerable))
        {
            if (methodCallExpression.Arguments?.Count == 2 && method.Name == "Contains")
            {
                Expression valueExpression;
                if (methodCallExpression.Arguments[1] is MemberExpression memberExpression)
                {
                    //"In" condition, Support the `Contains` extension Method of IEnumerable<TSource> Type
                    //For example: List<string> values = new List<string> { "foo", "bar"};
                    //values.Contains(u.Name)

                    memberExpression = methodCallExpression.Arguments[1] as MemberExpression;
                    valueExpression = methodCallExpression.Arguments[0];
                    return ConditionBuilder.BuildInCondition(memberExpression, valueExpression, adhesive);
                }
                else if (methodCallExpression.Arguments[1] is UnaryExpression unaryExpression)
                {
                    if (unaryExpression.NodeType == ExpressionType.Convert)
                    {
                        // 提取 UnaryExpression 的 Operand，确保它是 MemberExpression
                        memberExpression = unaryExpression.Operand as MemberExpression;
                        valueExpression = methodCallExpression.Arguments[0];
                        return ConditionBuilder.BuildInCondition(memberExpression, valueExpression, adhesive);
                    }
                }
                throw new NotImplementedException("请完善Enumerable.Contains()");
            }
        }
        else if (method.DeclaringType.IsGenericType)
        {
            if (method.DeclaringType.GetGenericTypeDefinition() == typeof(List<>)
                 && methodCallExpression.Arguments?.Count == 1
                 && method.Name == "Contains")
            {
                Expression valueExpression;
                if (methodCallExpression.Arguments[0] is MemberExpression memberExpression)
                {
                    valueExpression = methodCallExpression.Object;
                    return ConditionBuilder.BuildInCondition(memberExpression, valueExpression, adhesive);
                }
                else if (methodCallExpression.Arguments[0] is UnaryExpression unaryExpression)
                {
                    //issue:#01
                    if (unaryExpression.NodeType == ExpressionType.Convert)
                    {
                        // 提取 UnaryExpression 的 Operand，确保它是 MemberExpression
                        memberExpression = unaryExpression.Operand as MemberExpression;
                        valueExpression = methodCallExpression.Object;
                        return ConditionBuilder.BuildInCondition(memberExpression, valueExpression, adhesive);
                    }
                }

                throw new NotImplementedException("请完善List<xxx>.Contains()");
            }
        }

        throw new NotSupportedException();
    }

    private static StringBuilder ParseBinaryExpression(
        WhereClauseAdhesive adhesive,
        BinaryExpression binaryExpression,
        Dictionary<string, string> aliasDict)
    {
        #region 内部方法

        //处理别名
        StringBuilder ReplaceAlias(ParseResult parseResult)
        {
            if (parseResult.MemberInfo == null ||
                !aliasDict.ContainsKey(parseResult.MemberInfo.Name))
            {
                return parseResult.WhereClause;
            }

            var index = parseResult.WhereClause.IndexOf(' ');// IndexOf 是扩展方法
                                                             //Remove的index小于0会报错,所以,这里添加一个判断
                                                             //虽然这个判断永远不会生效(以目前逻辑来讲,2021.12.12),但是为了保险起见, 还是添加了
            if (index == -1)
            {
                return parseResult.WhereClause;
            }

            return parseResult.WhereClause.Remove(0, index).Insert(0, aliasDict[parseResult.MemberInfo.Name]);
        }

        #endregion

        if (binaryExpression.NodeType is
             ExpressionType.OrElse or
             ExpressionType.AndAlso or
             ExpressionType.Equal or
             ExpressionType.NotEqual or
             ExpressionType.GreaterThan or
             ExpressionType.GreaterThanOrEqual or
             ExpressionType.LessThan or
             ExpressionType.LessThanOrEqual
            )
        {
            var sqlBuilder = new StringBuilder();

            #region 处理left

            var leftParseResult = Parse(binaryExpression.NodeType, binaryExpression.Left, adhesive, aliasDict); //调用自身

            var leftClause = $"({ReplaceAlias(leftParseResult)})";
            sqlBuilder.Append(leftClause);

            if (leftParseResult.NeedAddPara)
            {
                if (IsDataComparator(binaryExpression.NodeType))
                {
                    object val;
                    if (binaryExpression.Right is ListInitExpression listInitExpression)
                    {
                        var valList = TypeHelper.MakeList(listInitExpression.Type.GenericTypeArguments);
                        foreach (var elementInit in listInitExpression.Initializers)
                        {
                            foreach (var arg in elementInit.Arguments)
                            {
                                valList.Add(ConstantExtractor.ParseConstant(arg));
                            }
                        }
                        val = valList;
                    }
                    else
                    {
                        //Basic case, For example: u.Age > 18
                        val = ConstantExtractor.ParseConstant(binaryExpression.Right);
                    }

                    leftParseResult.SqlClauseParametersInfo.Value = val;
                    leftParseResult.SqlClauseParametersInfo.SqlClause = leftClause;
                }
                else
                {
                    leftParseResult.SqlClauseParametersInfo.Value = ConstantExtractor.ParseConstant(leftParseResult.MemberExpression);
                    leftParseResult.SqlClauseParametersInfo.SqlClause = leftClause;
                }
            }

            #endregion

            #region 处理right

            var needParseRight = false;
            if (binaryExpression.NodeType == ExpressionType.OrElse) // {((a.Id == 1) OrElse (a.Id == 2))}
            {
                sqlBuilder.Append(SqlKeys.Or);
                needParseRight = true;
            }
            else if (binaryExpression.NodeType == ExpressionType.AndAlso) //{(((a.Id == 1) OrElse (a.Id == 2)) AndAlso a.IsDel == true )}
            {
                sqlBuilder.Append(SqlKeys.And);
                needParseRight = true;
            }
            else if (
                binaryExpression.Left.GetType().FullName == ExpressionFullNameSpaceConst.SimpleBinary &&
                (
                    binaryExpression.NodeType == ExpressionType.Equal ||
                    binaryExpression.NodeType == ExpressionType.NotEqual
                ) &&
                binaryExpression.Right is ConstantExpression constantExpression
                )
            {
                var val = ConstantExtractor.ParseConstantExpression(constantExpression);
                if (val.GetType() == typeof(string))
                {
                    if (binaryExpression.NodeType == ExpressionType.Equal)
                    {
                        sqlBuilder.Append($" {SqlKeys.Equal} '{val}'");
                    }
                    else if (binaryExpression.NodeType == ExpressionType.NotEqual)
                    {
                        sqlBuilder.Append($" {SqlKeys.NotEqual} '{val}'");
                    }
                }
                else
                {
                    if (binaryExpression.NodeType == ExpressionType.Equal)
                    {
                        sqlBuilder.Append($" {SqlKeys.Equal} {val}");
                    }
                    else if (binaryExpression.NodeType == ExpressionType.NotEqual)
                    {
                        sqlBuilder.Append($" {SqlKeys.NotEqual} {val}");
                    }
                }
            }

            if (needParseRight)
            {
                ParseResult rightParseResult = binaryExpression.Right.NodeType switch
                {
                    ExpressionType.MemberAccess => Parse(binaryExpression.NodeType, binaryExpression.Right, adhesive, aliasDict),//调用自身
                    ExpressionType.Constant => Parse(binaryExpression.NodeType, binaryExpression.Right, adhesive, aliasDict),//调用自身, {(a.Id == 1)}
                    _ => Parse(binaryExpression.Right.NodeType, binaryExpression.Right, adhesive, aliasDict),//调用自身
                };

                var rightClause = $"({ReplaceAlias(rightParseResult)})";
                sqlBuilder.Append(rightClause);

                if (rightParseResult.NeedAddPara)
                {
                    rightParseResult.SqlClauseParametersInfo.Value = ConstantExtractor.ParseConstant(rightParseResult.MemberExpression);
                    rightParseResult.SqlClauseParametersInfo.SqlClause = rightClause;
                }
            }

            #endregion

            return sqlBuilder;
        }
        else if (binaryExpression.NodeType is ExpressionType.Coalesce)
        {
            if (binaryExpression.Left is MemberExpression memberExpression &&
                binaryExpression.Right is ConstantExpression constantExpression)
            {
                string fieldName = ConditionBuilder.GetFieldName(memberExpression, memberExpression.Member);
                fieldName = adhesive.SqlAdapter.FormatColumnName(fieldName);

                var isnull_pms2_val = ConstantExtractor.ParseConstantExpression(constantExpression);

                if (isnull_pms2_val.GetType() == typeof(string))
                {
                    return new StringBuilder($"IsNull({fieldName}, N'{isnull_pms2_val}')");
                }
                else
                {
                    return new StringBuilder($"IsNull({fieldName}, {isnull_pms2_val})");
                }
            }
        }

        #region 这边的 if 好像进不来了, 先注释

        /*
        if (binaryExpression.Left is UnaryExpression convertExpression)
        {
            if (convertExpression.NodeType == ExpressionType.Convert
                && convertExpression.Operand.Type.IsEnum
                && convertExpression.Operand is MemberExpression enumMemberExpression
                && IsDataComparator(binaryExpression.NodeType))
            {
                //Support the enum Property, For example: u.UserType == UserType.Admin
                MemberInfo memberInfo = enumMemberExpression.Member;
                ExpressionType comparison = binaryExpression.NodeType;
                object value = ConstantExtractor.ParseConstant(binaryExpression.Right);
                var sqlBuilder = ConditionBuilder.BuildCondition(memberInfo, adhesive, comparison, value);
                return sqlBuilder;
            }
        }
        if (binaryExpression.Left is MemberExpression memberExpression)
        {
            if (IsDataComparator(binaryExpression.NodeType))
            {
                //Basic case, For example: u.Age > 18
                MemberInfo memberInfo = memberExpression.Member;
                ExpressionType comparison = binaryExpression.NodeType;
                object value = ConstantExtractor.ParseConstant(binaryExpression.Right);
                var sqlBuilder = ConditionBuilder.BuildCondition(memberInfo, adhesive, comparison, value);
                return sqlBuilder;
            }
        }
        */

        #endregion

        var msg = $"Unknow Left:{binaryExpression.Left.GetType()} ,Right:{binaryExpression.Right.GetType()} ,NodeType:{binaryExpression.NodeType}";
        throw new NotSupportedException(msg);
    }

    /// <summary>
    /// is数据比较符
    /// </summary>
    /// <param name="expressionType"></param>
    /// <returns></returns>
    private static bool IsDataComparator(ExpressionType expressionType)
    {
        if (expressionType is
           ExpressionType.Equal or
           ExpressionType.LessThan or
           ExpressionType.LessThanOrEqual or
           ExpressionType.GreaterThan or
           ExpressionType.GreaterThanOrEqual or
           ExpressionType.NotEqual)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 转换为逻辑符号(And 或 Or)
    /// </summary>
    /// <param name="expressionType"></param>
    /// <returns></returns>
    private static string ToLogicSymbol(this ExpressionType expressionType)
    {
        return expressionType switch
        {
            ExpressionType.AndAlso => SqlKeys.LogicSymbolAnd,
            ExpressionType.OrElse => SqlKeys.LogicSymbolOr,
            _ => throw new NotSupportedException($"Unknown ExpressionType {expressionType}"),
        };
    }
}