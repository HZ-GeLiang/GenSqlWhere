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
        /// <summary>
        /// 创建一个WhereLambda
        /// </summary>
        /// <typeparam name="TEntity">一般是数据库实体对象</typeparam>
        /// <typeparam name="TSearch">检索对象</typeparam>
        /// <param name="searchModel"></param>
        /// <param name="_">只是为了获得TEntity而存在的</param>
        /// <returns></returns>
        public static WhereLambda<TEntity, TSearch> CreateWhereLambda<TEntity, TSearch>(this TSearch searchModel, Action<TEntity> _)
            where TSearch : class
            where TEntity : class
        {
            return new WhereLambda<TEntity, TSearch>(searchModel);
        }

        /// <summary>
        /// 创建一个WhereLambda
        /// 适合TEntity == TSearch,这种方式的, 没法用 timeRange
        /// </summary>
        /// <typeparam name="TSearch"></typeparam>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public static WhereLambda<TSearch, TSearch> CreateWhereLambda<TSearch>(this TSearch searchModel) where TSearch : class
        {
            return new WhereLambda<TSearch, TSearch>(searchModel);
        }

    }
}