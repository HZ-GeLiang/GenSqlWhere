using ExpressionToSqlWhereClause.Helper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ExpressionToSqlWhereClause.SqlFunc;

namespace ExpressionToSqlWhereClause
{
    /// <summary>
    /// Where子句解析器
    /// </summary>
    internal static class WhereClauseParser
    {
        internal static (string WhereClause, WhereClauseAdhesive Adhesive) Parse(
            Expression body,
            Dictionary<string, string> aliasDict,
            ISqlAdapter sqlAdapter = default)
        {
            var adhesive = new WhereClauseAdhesive(sqlAdapter);

            if (body is BinaryExpression binaryExpression)
            {
                string whereClause = ParseBinaryExpression(adhesive, binaryExpression, aliasDict).ToString();
                return ($"({whereClause})", adhesive);
            }
            if (body is MethodCallExpression methodCallExpression)
            {
                var whereClause = ParseMethodCallExpression(adhesive, methodCallExpression);
                return ($"({whereClause.SqlClause})", adhesive);
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

                                return ($"({pageResult.WhereClause})", adhesive);
                            }
                        }
                    }

                }
                else
                {
                    return ($"({pageResult.WhereClause})", adhesive);
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
                if (method.DeclaringType == typeof(ExpressionToSqlWhereClause.SqlFunc.DbFunctions))
                {
                    // int Month (datetime dt)   =>  public static Int32 Month(DateTime dt)
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

                        string parameterName = ConditionBuilder.EnsureParameter(methodInfo, adhesive);
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
                SqlClauseParametersInfo param;
                if (memberExpression.Member is PropertyInfo pi && pi.PropertyType == typeof(bool))
                {
                    param = ConditionBuilder.BuildCondition(memberExpression.Member, adhesive, ExpressionType.Equal);
                }
                else
                {
                    param = ConditionBuilder.BuildCondition(memberExpression.Member, adhesive, comparison);
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
                            SqlClauseParametersInfo param = ConditionBuilder.BuildCondition(operandMemberExpression.Member, adhesive, comparison);
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
                        SqlClauseParametersInfo param = ConditionBuilder.BuildCondition(member, adhesive, comparison);
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
                        SqlClauseParametersInfo param = ConditionBuilder.BuildCondition(operandMemberExpression.Member, adhesive, comparison);
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
            }

            //不支持的: 
            //-expression  { Not(u.Name.Contains("Name"))}
            //System.Linq.Expressions.Expression { System.Linq.Expressions.UnaryExpression}

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
            }
            else if (method.DeclaringType == typeof(System.Linq.Enumerable))
            {
                if (methodCallExpression.Arguments?.Count == 2 && method.Name == "Contains")
                {
                    //"In" condition, Support the `Contains` extension Method of IEnumerable<TSource> Type
                    //For example: List<string> values = new List<string> { "foo", "bar"};
                    //values.Contains(u.Name)  
                    MemberExpression memberExpression = methodCallExpression.Arguments[1] as MemberExpression;
                    Expression valueExpression = methodCallExpression.Arguments[0];
                    return ConditionBuilder.BuildInCondition(memberExpression, valueExpression, adhesive);
                }
            }
            else if (method.DeclaringType.IsGenericType)
            {
                if (method.DeclaringType.GetGenericTypeDefinition() == typeof(List<>)
                     && methodCallExpression.Arguments?.Count == 1
                     && method.Name == "Contains")
                {
                    MemberExpression memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
                    Expression valueExpression = methodCallExpression.Object;
                    return ConditionBuilder.BuildInCondition(memberExpression, valueExpression, adhesive);
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
                 ExpressionType.LessThanOrEqual)
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
                    sqlBuilder.Append(SqlKeys.or);
                    needParseRight = true;
                }
                else if (binaryExpression.NodeType == ExpressionType.AndAlso) //{(((a.Id == 1) OrElse (a.Id == 2)) AndAlso a.IsDel == true )}
                {
                    sqlBuilder.Append(SqlKeys.and);
                    needParseRight = true;
                }

                if (needParseRight)
                {
                    ParseResult rightParseResult;

                    switch (binaryExpression.Right.NodeType)
                    {
                        case ExpressionType.MemberAccess:
                            rightParseResult = Parse(binaryExpression.NodeType, binaryExpression.Right, adhesive, aliasDict); //调用自身
                            break;
                        case ExpressionType.Constant:// {(a.Id == 1)} 
                            rightParseResult = Parse(binaryExpression.NodeType, binaryExpression.Right, adhesive, aliasDict); //调用自身
                            break;
                        default:
                            rightParseResult = Parse(binaryExpression.Right.NodeType, binaryExpression.Right, adhesive, aliasDict); //调用自身
                            break;
                    }

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

            #region  这边的 if 好像进不来了, 先注释
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
            var msg = $"Unknow Left:{binaryExpression.Left.GetType()}, Right:{binaryExpression.Right.GetType()},  NodeType:{binaryExpression.NodeType}";
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
            switch (expressionType)
            {
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.OrElse:
                    return "OR";
                default:
                    throw new NotSupportedException($"Unknown ExpressionType {expressionType}");
            }
        }
    }
}
