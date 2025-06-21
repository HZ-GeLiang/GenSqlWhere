using ExpressionToSqlWhereClause.EntityConfig;

namespace ExpressionToSqlWhereClause.ExtensionMethods;

/// <summary>
/// 扩展方法
/// </summary>
public static class WhereLambdaExtensions
{
    /// <summary>
    /// 创建一个WhereLambda
    /// 第二个类型通过创建一个 空的委托
    /// </summary>
    /// <typeparam name="TEntity">数据库实体类型</typeparam>
    /// <typeparam name="TSearch">检索模型类型</typeparam>
    /// <param name="searchModel">检索模型实例</param>
    /// <param name="_">只是为了获得TEntity而存在的</param>
    /// <returns></returns>
    [Obsolete("推荐用defaut(T)的形式")]
    public static WhereLambda<TEntity, TSearch> CreateWhereLambda<TEntity, TSearch>(this TSearch searchModel, Action<TEntity> _)
        where TSearch : class
        where TEntity : class
    {
        //使用示例 , 使用的不方便
        //searchModel.CreateWhereLambda((Input_likeLeft _) => { });
        return new WhereLambda<TEntity, TSearch>(searchModel);
    }

    /// <summary>
    /// 创建一个WhereLambda  推荐
    /// </summary>
    /// <typeparam name="TEntity">数据库实体类型</typeparam>
    /// <typeparam name="TSearch">检索模型类型</typeparam>
    /// <param name="searchModel">检索模型实例</param>
    /// <param name="_">只是为了获得TEntity而存在的</param>
    /// <returns></returns>
    public static WhereLambda<TEntity, TSearch> CreateWhereLambda<TEntity, TSearch>(this TSearch searchModel, TEntity _)
        where TSearch : class
        where TEntity : class
    {
        //使用示例
        //searchModel.CreateWhereLambda((model_like)null);
        //searchModel.CreateWhereLambda(default(model_like));    推荐这种
        return new WhereLambda<TEntity, TSearch>(searchModel);
    }

    /// <summary>
    /// 创建一个WhereLambda
    /// 适合TEntity == TSearch,这种方式的, 没法用 timeRange
    /// </summary>
    /// <typeparam name="TSearch"></typeparam>
    /// <param name="searchModel"></param>
    /// <returns></returns>
    public static WhereLambda<TSearch, TSearch> CreateWhereLambda<TSearch>(this TSearch searchModel)
        where TSearch : class
    {
        return new WhereLambda<TSearch, TSearch>(searchModel);
    }

    /// <summary>
    /// 创建一个 WhereLambda 实例，用于构建查询条件。
    /// 两个类型都要传递
    /// 不怎么推荐
    /// </summary>
    /// <typeparam name="TEntity">数据库实体类型</typeparam>
    /// <typeparam name="TSearch">检索模型类型</typeparam>
    /// <param name="searchModel">检索模型实例</param>
    /// <returns>WhereLambda 实例</returns>
    public static WhereLambda<TEntity, TSearch> CreateWhereLambda<TEntity, TSearch>(this TSearch searchModel)
        where TSearch : class
        where TEntity : class
    {
        return new WhereLambda<TEntity, TSearch>(searchModel);
    }
}