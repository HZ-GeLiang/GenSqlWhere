using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToSqlWhereClause
{
    /// <summary>
    /// Where子句解析器
    /// </summary>
    internal static class WhereClauseParser
    {

        public static (string WhereClause, Dictionary<string, object> Parameters) Parse(Expression body, Dictionary<string, string> aliasDict, ISqlAdapter sqlAdapter = default)
        {
            aliasDict ??= new Dictionary<string, string>(0);
            sqlAdapter ??= new DefaultSqlAdapter();
            var parameters = new Dictionary<string, object>(0);
            var adhesive = new WhereClauseAdhesive(sqlAdapter, parameters);

            if (body is BinaryExpression binaryExpression)
            {
                var whereClause = ParseBinaryExpression(adhesive, binaryExpression, aliasDict).ToString();
                return ($"({whereClause})", parameters);
            }
            else if (body is MethodCallExpression methodCallExpression)
            {
                var whereClause = ParseMethodCallExpression(adhesive, methodCallExpression);
                return ($"({whereClause})", parameters);
                //throw new NotSupportedException("暂不支持MethodCallExpression,修改程序");
            }
            else if (body is MemberExpression)
            {
                throw new NotSupportedException("暂不支持MemberExpression,修改程序");
            }
            else if (body is UnaryExpression unaryExpression)// UnaryExpression : Expression
            {
                var pageResult = Parse(body.NodeType, body, adhesive, aliasDict);
                if (pageResult.NeedAddPara)
                {
                    string parameterName = ConditionBuilder.EnsureParameter(pageResult.MemberInfo, adhesive);

                    if (body.NodeType == ExpressionType.Not)
                    {
                        //not 只支持bool 类型
                        if (unaryExpression.Type == typeof(bool))
                        {
                            if (unaryExpression.Operand is MemberExpression operandMemberExpression)
                            {
                                var val = ConstantExtractor.ParseConstant(unaryExpression.Operand);
                                adhesive.Parameters.Add($"@{parameterName}", !Convert.ToBoolean(val));
                                return ($"({pageResult.WhereClause})", adhesive.Parameters);
                            }
                            else if (unaryExpression.Operand.GetType().Name == "LogicalBinaryExpression")
                            {
                                var val = ConstantExtractor.ParseConstant(unaryExpression.Operand);
                                adhesive.Parameters.Add($"@{parameterName}", !Convert.ToBoolean(val));//!取反
                                return ($"({pageResult.WhereClause})", adhesive.Parameters);
                            }
                        }
                    }
                    throw new NotSupportedException("暂不支持UnaryExpression,修改程序");
                }
                else
                {
                    return ($"({pageResult.WhereClause})", adhesive.Parameters);
                }
            }
            else if (body is Expression)
            {
                throw new NotSupportedException("暂不支持Expression,修改程序");
            }
            else
            {
                throw new NotSupportedException("暂不支持,修改程序");
            }
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
            else if (expression is MethodCallExpression methodCallExpression)
            {
                var whereClause = ParseMethodCallExpression(adhesive, methodCallExpression);
                return new ParseResult() { WhereClause = whereClause };
            }
            else if (expression is MemberExpression memberExpression)
            {
                StringBuilder whereClause;
                if (memberExpression.Member is PropertyInfo pi && pi.PropertyType == typeof(bool))
                {
                    whereClause = ConditionBuilder.BuildCondition(memberExpression.Member, adhesive, ExpressionType.Equal);
                }
                else
                {
                    whereClause = ConditionBuilder.BuildCondition(memberExpression.Member, adhesive, comparison);
                }
                return new ParseResult() { WhereClause = whereClause, NeedAddPara = true, MemberExpression = memberExpression, MemberInfo = memberExpression.Member };
            }
            else if (expression is UnaryExpression unaryExpression)
            {
                if (unaryExpression.NodeType == ExpressionType.Not)
                {
                    if (unaryExpression.Operand is MemberExpression operandMemberExpression)
                    {
                        if (unaryExpression.Type == typeof(bool))
                        {
                            var whereClause = ConditionBuilder.BuildCondition(operandMemberExpression.Member, adhesive, comparison);
                            return new ParseResult()
                            {
                                WhereClause = whereClause,
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
                        var whereClause = ConditionBuilder.BuildCondition(member, adhesive, comparison);
                        return new ParseResult()
                        {
                            WhereClause = whereClause,
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
                        var whereClause = ConditionBuilder.BuildCondition(operandMemberExpression.Member, adhesive, comparison);
                        return new ParseResult()
                        {
                            WhereClause = whereClause,
                            NeedAddPara = true,
                            MemberExpression = null,
                            MemberInfo = operandMemberExpression.Member
                        };
                    }
                }
            }
            throw new NotSupportedException($"Unknow expression {expression.GetType()}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adhesive">粘合剂</param>
        /// <param name="methodCallExpression"></param>
        /// <returns></returns>
        private static StringBuilder ParseMethodCallExpression(WhereClauseAdhesive adhesive, MethodCallExpression methodCallExpression)
        {
            var method = methodCallExpression.Method;

            if (method.DeclaringType == typeof(string) && (method.Name == "Contains" || method.Name == "StartsWith" || method.Name == "EndsWith"))
            {
                //"Like" condition for string property, For example: u.Name.Contains("A")
                return ConditionBuilder.BuildLikeOrEqualCondition(methodCallExpression, adhesive);
            }
            else if (method.Name == "Equals")
            {
                //"Like" condition for string property, For example: u.Name.Contains("A")
                return ConditionBuilder.BuildLikeOrEqualCondition(methodCallExpression, adhesive);
            }
            else if (method.DeclaringType == typeof(System.Linq.Enumerable)
                     && methodCallExpression.Arguments?.Count == 2
                     && method.Name == "Contains")
            {
                //"In" condition, Support the `Contains` extension Method of IEnumerable<TSource> Type
                //For example: List<string> values = new List<string> { "foo", "bar"};
                //             values.Contains(u.Name)  
                MemberExpression memberExpression = methodCallExpression.Arguments[1] as MemberExpression;
                Expression valueExpression = methodCallExpression.Arguments[0];
                return ConditionBuilder.BuildInCondition(memberExpression, valueExpression, adhesive);
            }
            else if (method.DeclaringType.IsGenericType
                     && method.DeclaringType.GetGenericTypeDefinition() == typeof(List<>)
                     && methodCallExpression.Arguments?.Count == 1
                     && method.Name == "Contains")
            {
                //"In" Condition, Support the `Contains` Method of List<T> type
                //For example: string[] values = new string[]{ "foo", "bar"};
                //             values.Contains(u.Name)  
                MemberExpression memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
                Expression valueExpression = methodCallExpression.Object;
                return ConditionBuilder.BuildInCondition(memberExpression, valueExpression, adhesive);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private static StringBuilder ParseBinaryExpression(
                WhereClauseAdhesive adhesive,
                BinaryExpression binaryExpression,
                Dictionary<string, string> aliasDict = default
            )
        {
            //在这里处理: 往adhesive的参数添加值 
            void AddParamteter3(ParseResult _parseResult, WhereClauseAdhesive _adhesive, object _value)
            {
                string parameterName = ConditionBuilder.EnsureParameter(_parseResult.MemberInfo, _adhesive);
                _adhesive.Parameters.Add($"@{parameterName}", _value);
            }

            void AddParamteter2(ParseResult _parseResult, WhereClauseAdhesive _adhesive)
            {
                string parameterName = ConditionBuilder.EnsureParameter(_parseResult.MemberInfo, _adhesive);
                var _value = ConstantExtractor.ParseConstant(_parseResult.MemberExpression);
                _adhesive.Parameters.Add($"@{parameterName}", _value);
            }

            //处理别名
            StringBuilder ReplaceAlias(ParseResult parseResult)
            {
                string alias = null;

                if (aliasDict != null && aliasDict.Count > 0 && parseResult.MemberInfo != null)
                {
                    var aliasKey = parseResult.MemberInfo.Name;
                    if (aliasDict.ContainsKey(aliasKey))
                    {
                        alias = aliasDict[aliasKey];
                    }
                }

                if (alias == null)
                {
                    return parseResult.WhereClause;
                }

                var index = parseResult.WhereClause.IndexOf(' ');
                if (index==-1)
                {
                    return parseResult.WhereClause;
                }
                parseResult.WhereClause.Remove(0, index);
                parseResult.WhereClause.Insert(0, alias);
                return parseResult.WhereClause;

            }

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

                //处理left
                var leftParseResult = Parse(binaryExpression.NodeType, binaryExpression.Left, adhesive, aliasDict); //调用自身

                //record: 生成的sql 有 () 问题  , 在最外层返回结果时, 在只有and 的时候把 () 全部替换掉了.
                var leftClause = $"({ReplaceAlias(leftParseResult)})";
                sqlBuilder.Append(leftClause);

                if (leftParseResult.NeedAddPara)
                {
                    //if (binaryExpression.NodeType == ExpressionType.Convert)
                    //{
                    //    var val = ConstantExtractor.ParseConstant(binaryExpression);
                    //}
                    //else if (IsDataComparator(binaryExpression.NodeType))
                    if (IsDataComparator(binaryExpression.NodeType))
                    {
                        //Basic case, For example: u.Age > 18
                        var val = ConstantExtractor.ParseConstant(binaryExpression.Right);
                        AddParamteter3(leftParseResult, adhesive, val);
                    }
                    else
                    {
                        AddParamteter2(leftParseResult, adhesive);
                    }
                }

                //处理right
                var parseRight = false;
                if (binaryExpression.NodeType == ExpressionType.OrElse) // {((a.Id == 1) OrElse (a.Id == 2))}
                {
                    sqlBuilder.Append(SqlKeys.or);
                    parseRight = true;
                }
                else if (binaryExpression.NodeType == ExpressionType.AndAlso) //{(((a.Id == 1) OrElse (a.Id == 2)) AndAlso a.IsDel == true )}
                {
                    sqlBuilder.Append(SqlKeys.and);
                    parseRight = true;
                }

                if (parseRight)
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
                        AddParamteter2(rightParseResult, adhesive);
                    }
                }

                return sqlBuilder;
            }
            else if (binaryExpression.Left is UnaryExpression convertExpression
                     && convertExpression.NodeType == ExpressionType.Convert
                     && convertExpression.Operand.Type.IsEnum
                     && convertExpression.Operand is MemberExpression enumMemberExpression
                     && IsDataComparator(binaryExpression.NodeType))
            {
                //Support the enum Property, For example: u.UserType == UserType.Admin
                return ConditionBuilder.BuildCondition(enumMemberExpression.Member, adhesive, binaryExpression.NodeType,
                    ConstantExtractor.ParseConstant(binaryExpression.Right));
            }
            else if (binaryExpression.Left is MemberExpression memberExpression && IsDataComparator(binaryExpression.NodeType))
            {
                //Basic case, For example: u.Age > 18
                return ConditionBuilder.BuildCondition(memberExpression.Member, adhesive, binaryExpression.NodeType,
                    ConstantExtractor.ParseConstant(binaryExpression.Right));
            }
            else
            {
                var msg = $"Unknow Left:{binaryExpression.Left.GetType()}, Right:{binaryExpression.Right.GetType()},  NodeType:{binaryExpression.NodeType}";
                throw new NotSupportedException(msg);
            }
        }

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
