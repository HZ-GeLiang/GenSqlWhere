using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.EntityConfig
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class WhereLambdaExtensions
    {
       

        ///
        public static WhereLambda<TEntity, TSearch> CrateWhereLambda<TEntity, TSearch>(this TSearch searchModel, Action<TEntity> _)
            where TSearch : class
            where TEntity : class
        {
            return new WhereLambda<TEntity, TSearch>(searchModel);
        }

        /// <summary>
        /// 适合TEntity ==  TSearch,
        /// 这种方式的, 没法用 timeRange
        /// </summary>
        /// <typeparam name="TSearch"></typeparam>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public static WhereLambda<TSearch, TSearch> CrateWhereLambda<TSearch>(this TSearch searchModel) where TSearch : class
        {
            return new WhereLambda<TSearch, TSearch>(searchModel);
        }

    }
}