using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.ExtensionMethods;

/// <summary>
/// 扩展方法
/// </summary>
public static class QueryConfigExtensions
{
    /// <summary>
    /// 创建一个WhereLambda  推荐
    /// </summary>
    /// <typeparam name="TEntity">数据库实体类型</typeparam>
    /// <typeparam name="TSearch">检索模型类型</typeparam>
    /// <param name="searchModel">检索模型实例</param>
    /// <param name="_">只是为了获得TEntity而存在的</param>
    /// <returns></returns>
    public static QueryConfig<TEntity, TSearch> CreateQueryConfig<TEntity, TSearch>(this TSearch searchModel, TEntity _)
        where TSearch : class
        where TEntity : class
    {
        //使用示例
        //searchModel.QueryConfig((model_like)null);
        //searchModel.QueryConfig(default(model_like));    推荐这种
        return new QueryConfig<TEntity, TSearch>(searchModel);
    }

    /// <summary>
    /// 创建一个WhereLambda
    /// 适合TEntity == TSearch,这种方式的, 没法用 timeRange
    /// </summary>
    /// <typeparam name="TSearch"></typeparam>
    /// <param name="searchModel"></param>
    /// <returns></returns>
    public static QueryConfig<TSearch, TSearch> CreateQueryConfig<TSearch>(this TSearch searchModel)
        where TSearch : class
    {
        return new QueryConfig<TSearch, TSearch>(searchModel);
    }
}