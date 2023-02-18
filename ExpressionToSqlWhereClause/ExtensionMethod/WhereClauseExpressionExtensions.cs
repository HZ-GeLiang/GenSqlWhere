using ExpressionToSqlWhereClause.ExpressionTree;
using ExpressionToSqlWhereClause.ExpressionTree.Adapter;
using ExpressionToSqlWhereClause.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq.Expressions;
using System.Text.RegularExpressions;


namespace ExpressionToSqlWhereClause.ExtensionMethod
{
    public static class WhereClauseExpressionExtensions
    {
        /// <summary>
        /// 转换为Where子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <param name="alias">别名</param>
        /// <param name="sqlAdapter">适配器</param>
        /// <returns></returns>
        public static SearchCondition ToWhereClause<T>(
            this Expression<Func<T, bool>> expression,
            Dictionary<string, string> alias = null,
            ISqlAdapter sqlAdapter = default) where T : class
        {
            if (expression == null)
            {
                return SearchCondition.CreateDefault(); 
            }

            #region 处理 alias
            alias ??= new Dictionary<string, string>();

            foreach (var propertyInfo in ReflectionHelper.GetProperties<T>())
            {
                //优先级: 方法参数的 alias > Column
                if (alias.ContainsKey(propertyInfo.Name))
                {
                    continue;
                }
                var attrs = ReflectionHelper.GetAttributeForProperty<ColumnAttribute>(propertyInfo, true);
                if (attrs.Length == 1)
                {
                    alias[propertyInfo.Name] = attrs[0].Name;
                }
            }
            #endregion

            var body = expression.Body;
            var parseResult = WhereClauseParser.Parse(body, alias, sqlAdapter);

            #region 处理 parseResult 的  Adhesive  和 WhereClause

            foreach (var key in parseResult.Adhesive.GetParameterKeys())
            {
                var para = parseResult.Adhesive.GetParameter(key);
                var val = para.Value;

                // 处理 val == null 的情况
                if (val == null)
                {
                    if (para.Symbol == SqlKeys.Equal || para.Symbol == SqlKeys.NotEqual)
                    {
                        //为了支持 a.url == null , 此处需要翻译为 url is null
                        var sqlClauseOrigin = para.SqlClause;
                        var reg = "[ ].*[ ]@.*\\)$";
                        var strList = RegexHelper.GetStringByMatches(sqlClauseOrigin, reg);
                        if (strList.Count == 1)
                        {
                            string sqlClauseNew;
                            if (para.Symbol == SqlKeys.Equal)
                            {
                                sqlClauseNew = sqlClauseOrigin.Replace(strList[0], " Is Null)");
                            }
                            else if (para.Symbol == SqlKeys.NotEqual)
                            {
                                sqlClauseNew = sqlClauseOrigin.Replace(strList[0], " Is Not Null)");
                            }
                            else
                            {
                                throw new ExpressionToSqlWhereClauseException("逻辑错误,需要调整");
                            }

                            parseResult.WhereClause = parseResult.WhereClause.Replace(sqlClauseOrigin, sqlClauseNew);
                            parseResult.Adhesive.RemoveParameter(key);//移除参数

                            continue;
                        }
                        //else 没有处理 
                    }

                    //else 没有处理 
                }

                //翻译IEnumerable的值
                var parseValue = ConstantExtractor.ConstantExpressionValueToString(val, out var isIEnumerableObj);
                if (isIEnumerableObj)
                {
                    para.Value = parseValue;
                    continue;
                }
            }

            #endregion

            var result = new SearchCondition();
            result.WhereClauseRaw = parseResult.WhereClause;
            result.WhereClause = TrimParentheses(parseResult.WhereClause);
            result.Parameters = GetParameters(parseResult.Adhesive);

            return result;
        }


        private static string TrimParentheses(string WhereClause)
        {
            string str;
            if (ParenthesesTrimHelper.CanTrimAll(WhereClause)) //去掉 关系条件 全为 and 时的 () 
            {
                str = ParenthesesTrimHelper.TrimAll(WhereClause);//全是and 的
            }
            else
            {
                str = ParenthesesTrimHelper.ParseTrimAll(WhereClause);
            }

            return str;
        }

        private static Dictionary<string, object> GetParameters(WhereClauseAdhesive Adhesive)
        {
            var parameters = new Dictionary<string, object>(0);
            foreach (var key in Adhesive.GetParameterKeys())
            {
                parameters.Add(key, Adhesive.GetParameter(key).Value);
            }

            return parameters;
        }

    }

    public class SearchCondition
    {
        public string WhereClause { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// 未加工处理的 WhereClause
        /// </summary>
        public string WhereClauseRaw { get; set; }

        public static SearchCondition CreateDefault()
        {
            return new SearchCondition
            {
                WhereClauseRaw = string.Empty,
                WhereClause = string.Empty,
                Parameters = new Dictionary<string, object>(),
            };
        }

    }
}
