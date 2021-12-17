using ExpressionToSqlWhereClause.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExpressionToSqlWhereClause
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
        public static (string WhereClause, Dictionary<string, object> Parameters) ToWhereClause<T>(this Expression<Func<T, bool>> expression, Dictionary<string, string> alias = null, ISqlAdapter sqlAdapter = default)
            where T : class
        {
            if (expression == null)
            {
                //return (default, default);
                return (string.Empty, new Dictionary<string, object>(0));
            }

            #region 处理 alias
            if (alias == null)
            {
                alias = new Dictionary<string, string>();
            }
            var columnAttributeType = typeof(ColumnAttribute);
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                //优先级: 方法参数的 alias > Column
                if (!alias.ContainsKey(propertyInfo.Name))
                {
                    var attrs = ReflectionHelper.GetAttributeForProperty<ColumnAttribute>(propertyInfo, true);
                    if (attrs.Length == 1)
                    {
                        alias[propertyInfo.Name] = ((ColumnAttribute)attrs[0]).Name;
                    }
                }
            }
            #endregion

            var body = expression.Body;
            var parseResult = WhereClauseParser.Parse(body, alias, sqlAdapter);

            #region 处理 parseResult.WhereClause

            #region 去掉 关系条件 多余的 ()
            //能力有限: 只能去掉全是 and 语句的 ()
            if (!parseResult.WhereClause.Contains(SqlKeys.or))
            {
                if (parseResult.WhereClause.Contains("("))
                {
                    parseResult.WhereClause = parseResult.WhereClause.Replace("(", string.Empty);
                }
                if (parseResult.WhereClause.Contains(")"))
                {
                    parseResult.WhereClause = parseResult.WhereClause.Replace(")", string.Empty);
                }
            }
            #endregion

            #region 函数的[] 变成 ()

            if (parseResult.WhereClause.Contains("[") && parseResult.WhereClause.Contains("]"))
            {
                parseResult.WhereClause = parseResult.WhereClause.Replace("[", "(");
                parseResult.WhereClause = parseResult.WhereClause.Replace("]", ")");
            }

            #endregion

            #endregion

            #region 处理 parseResult.Parameters

            foreach (var key in parseResult.Parameters.Keys)
            {
                var parseValue = ConstantExtractor.ConstantExpressionValueToString(parseResult.Parameters[key], out var isIEnumerableObj);
                if (isIEnumerableObj)
                {
                    parseResult.Parameters[key] = parseValue;
                }
                else
                {
                    //为了支持 a.url == null 可以翻译为 url is null
                    if (parseResult.Parameters[key] == null)
                    {
                        parseResult.Parameters.Remove(key);
                    }
                    // todo: 还需要处理  "Url = @Url" 为 Url is null
                }
            }

            #endregion

            var result = (parseResult.WhereClause, parseResult.Parameters ?? new Dictionary<string, object>(0));
            return result;
        }

        /// <summary>
        /// 转换为Where子句(异步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public static Task<(string WhereClause, Dictionary<string, object> Parameters)> ToWhereClauseAsync<T>(this Expression<Func<T, bool>> expression, Dictionary<string, string> alias = null, ISqlAdapter sqlAdapter = default)
            where T : class
        {
            var result = Task.FromResult(expression.ToWhereClause(alias, sqlAdapter));
            return result;
        }
    }
}
