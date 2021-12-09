using System;
using System.Collections.Generic;
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
                return (default, default);
            }

            var body = expression.Body;
            var parseResult = WhereClauseParser.Parse(body, alias, sqlAdapter);

            #region 处理 parseResult.WhereClause

            //去掉 关系条件的 ()
            //只能去掉全是 and 语句的 ()
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

            #region 处理 parseResult.Parameters

            foreach (var key in parseResult.Parameters.Keys)
            {
                var parseValue = ConstantExtractor.ConstantExpressionValueToString(parseResult.Parameters[key], out var isParse);
                if (isParse)
                {
                    parseResult.Parameters[key] = parseValue;
                }
            }

            #endregion

            var result = (parseResult.WhereClause, parseResult.Parameters);
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
