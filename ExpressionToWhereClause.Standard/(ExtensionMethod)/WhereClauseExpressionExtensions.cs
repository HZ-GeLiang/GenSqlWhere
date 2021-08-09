﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExpressionToWhereClause.Standard
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

            foreach (var key in parseResult.Parameters.Keys)
            {
                var parseValue = ConstantExtractor.ConstantExpressionValueToString(parseResult.Parameters[key], out var isParse);
                if (isParse)
                {
                    parseResult.Parameters[key] = parseValue;
                }
            }

            return (parseResult.WhereClause, parseResult.Parameters);
        }

        /// <summary>
        /// 转换为Where子句(异步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public static Task<(string WhereClause, Dictionary<string, object> Parameters)> ToWhereClauseAsync<T>(this Expression<Func<T, bool>> expression, Dictionary<string, string> alias = null, ISqlAdapter sqlAdapter = default)
            where T : class
        {
            return Task.FromResult(expression.ToWhereClause(alias, sqlAdapter));
        }
    }
}