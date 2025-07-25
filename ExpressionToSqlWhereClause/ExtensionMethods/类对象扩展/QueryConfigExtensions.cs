﻿using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause;

/// <summary>
/// 扩展方法
/// </summary>
public static class QueryConfigExtensions
{
    /// <summary>
    /// 创建一个WhereLambda  推荐
    /// </summary>
    /// <typeparam name="TSearch">检索模型类型</typeparam>
    /// <typeparam name="TDbEntity">数据库实体类型</typeparam>
    /// <param name="searchModel">检索模型实例</param>
    /// <param name="_">只是为了获得TEntity而存在的</param>
    /// <returns></returns>
    public static QueryConfig<TSearch, TDbEntity> CreateQueryConfig<TSearch, TDbEntity>(this TSearch searchModel, TDbEntity _)
        where TSearch : class
        where TDbEntity : class
    {
        //使用示例
        //searchModel.QueryConfig((model_like)null);
        //searchModel.QueryConfig(default(model_like));    推荐这种
        return new QueryConfig<TSearch, TDbEntity>(searchModel);
    }

    ///// <summary>
    ///// 创建一个WhereLambda
    ///// 适合TEntity == TSearch,这种方式的, 没法用 timeRange
    ///// </summary>
    ///// <typeparam name="TSearch"></typeparam>
    ///// <param name="searchModel"></param>
    ///// <returns></returns>
    //public static QueryConfig<TSearch, TSearch> CreateQueryConfig<TSearch>(this TSearch searchModel)
    //    where TSearch : class
    //{
    //    return new QueryConfig<TSearch, TSearch>(searchModel);
    //}
}