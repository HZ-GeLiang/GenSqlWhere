using ExpressionToSqlWhereClause.Exceptions;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Helpers;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionToSqlWhereClause.EntitySearchBuilder;

/// <summary>
/// TSearch 转 TEntity 的表达式树的配置
/// </summary>
/// <typeparam name="TSearch">检索对象,一般为后端定义好后给前端传参的模型对象. 注: 不要使用多态,会报错的</typeparam>
/// <typeparam name="TEntity">一般是数据库实体对象</typeparam>
public class QueryConfig<TSearch, TEntity>
    where TEntity : class
    where TSearch : class
{
    //添加where的排序顺序: 目的是尽可能的让索引生效(也就是like必须是最后的,其他只能随意)
    private static readonly SearchType[] _addOrder = {
        SearchType.None,
        SearchType.Eq,
        SearchType.In,
        SearchType.TimeRange,
        SearchType.NumberRange,
        SearchType.Gt,
        SearchType.Ge,
        SearchType.Lt,
        SearchType.Le,
        SearchType.Neq,
        SearchType.LikeLeft,
        SearchType.LikeRight,
        SearchType.Like,
    };

    ///// <summary>
    ///// 构造器创建,需要 赋值 SearchModel 属性才能使用
    ///// </summary>
    //public WhereLambda() { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search">检索对象</param>
    public QueryConfig(TSearch search)
    {
        this.Search = search;
    }

    /// <summary>
    /// 检索对象
    /// </summary>
    public TSearch Search { get; set; }

    /// <summary>
    /// 属性的值是否有值
    /// </summary>
    public Dictionary<string, Expression<Func<TSearch, bool>>> WhereIf { get; set; } = new();

    /// <summary>
    /// 检索对象的配置
    /// </summary>
    private Dictionary<SearchType, List<string>> _dictSearhType = new Dictionary<SearchType, List<string>>();

    public List<string> this[SearchType searchType]
    {
        get
        {
            if (!_dictSearhType.ContainsKey(searchType))
            {
                _dictSearhType[searchType] = new List<string>();
            }
            return _dictSearhType[searchType];
        }
        set
        {
            //_dictSearhType[searchType] = value;
            var list = this[searchType];

            foreach (var propertyName in value)
            {
                AddSearchType(list, propertyName);
            }
        }
    }

    public bool AddSearchType(SearchType searchType, string propertyName)
    {
        var list = this[searchType];
        return AddSearchType(list, propertyName);
    }

    /// <summary>
    /// 不区分大小写
    /// </summary>
    /// <param name="list"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    private bool AddSearchType(List<string> list, string propertyName)
    {
        if (!list.Contains(propertyName, StringComparer.OrdinalIgnoreCase))
        {
            list.Add(propertyName);
            return true;
        }

        return false;
    }

    #region ToExpression

    /// <inheritdoc cref="ToExpressionCore{TEntity}(List{Expression{Func{TEntity, bool}}})"/>
    public Expression<Func<TEntity, bool>> ToExpression()
    {
        List<Expression<Func<TEntity, bool>>> whereLambdas = ToExpressionList(this);
        var expression = QueryConfig<TSearch, TEntity>.ToExpressionCore(whereLambdas);
        return expression;
    }

    /// <inheritdoc cref="ToExpressionCore{TEntity}(List{Expression{Func{TEntity, bool}}})"/>
    public static implicit operator Expression<Func<TEntity, bool>>(QueryConfig<TSearch, TEntity> that)
    {
        var whereLambdas = ToExpressionList(that);
        var expression = QueryConfig<TSearch, TEntity>.ToExpressionCore(whereLambdas);
        return expression;
    }

    /// <inheritdoc cref="ToExpressionCore{TEntity}(List{Expression{Func{TEntity, bool}}})"/>
    public Expression<Func<TEntity, bool>> ToExpression<TEntity>(List<Expression<Func<TEntity, bool>>> whereLambdas)
        where TEntity : class
    {
        return QueryConfig<TSearch, TEntity>.ToExpressionCore(whereLambdas);
    }

    /// <summary>
    /// 转表达式树
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="whereLambdas"></param>
    /// <returns></returns>
    private static Expression<Func<TEntity, bool>> ToExpressionCore<TEntity>(List<Expression<Func<TEntity, bool>>> whereLambdas)
        where TEntity : class
    {
        if (!whereLambdas.Any())
        {
            return null;
        }

        if (whereLambdas.Count == 1)
        {
            return whereLambdas[0];
        }

        Expression<Func<TEntity, bool>> exp = whereLambdas[0];
        for (int i = 1; i < whereLambdas.Count; i++)
        {
            exp = exp.And(whereLambdas[i]);
        }

        return exp;
    }

    #endregion

    #region ToExpressionList

    /// <summary>
    /// 获得表达式树的写法,可以给ef用(不含sql内置函数的那种)
    /// </summary>
    /// <returns></returns>
    public static List<Expression<Func<TEntity, bool>>> ToExpressionList(QueryConfig<TSearch, TEntity> that)
    {
        //该方法的 static 是为了给 implicit operator 方法使用的
        var searchConditionDict = GetSearchCondition(that._dictSearhType);
        return ToExpressionList(that, searchConditionDict);
    }

    /// <inheritdoc cref="ToExpressionList(QueryConfig{TSearch , TEntity})"/>
    public List<Expression<Func<TEntity, bool>>> ToExpressionList()
    {
        var searchConditionDict = GetSearchCondition(this._dictSearhType);
        return ToExpressionList(this, searchConditionDict);
    }

    /// <summary>
    /// 转 ToExpressionList
    /// </summary>
    /// <param name="that"></param>
    /// <param name="searchConditionDict"></param>
    /// <returns></returns>
    /// <exception cref="FrameException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static List<Expression<Func<TEntity, bool>>> ToExpressionList(QueryConfig<TSearch, TEntity> that, Dictionary<SearchType, List<string>> searchConditionDict)
    {
        var whereLambdas = new List<Expression<Func<TEntity, bool>>>();

        foreach (SearchType searchType in _addOrder)
        {
            if (searchType == SearchType.None ||
                searchConditionDict.ContainsKey(searchType) == false ||
                searchConditionDict[searchType] == null ||
                searchConditionDict[searchType].Count <= 0
               )
            {
                continue;
            }
            List<string> searchCondition = searchConditionDict[searchType];
            if (searchCondition == null || !searchCondition.Any())
            {
                continue;
            }

            List<Expression<Func<TEntity, bool>>> expressionList = searchType switch
            {
                SearchType.Like => QueryConfigHelper.AddLike<TSearch, TEntity>(that, searchCondition),
                SearchType.LikeLeft => QueryConfigHelper.AddLikeLeft<TSearch, TEntity>(that, searchCondition),
                SearchType.LikeRight => QueryConfigHelper.AddLikeRight<TSearch, TEntity>(that, searchCondition),
                SearchType.Eq => QueryConfigHelper.AddEqual<TSearch, TEntity>(that, searchCondition),
                SearchType.Neq => QueryConfigHelper.AddNotEqual<TSearch, TEntity>(that, searchCondition),
                SearchType.In => QueryConfigHelper.AddIn<TSearch, TEntity>(that, searchCondition),
                SearchType.TimeRange => QueryConfigHelper.AddTimeRange<TSearch, TEntity>(that, searchCondition),
                SearchType.NumberRange => QueryConfigHelper.AddNumberRange<TSearch, TEntity>(that, searchCondition),
                SearchType.Gt => QueryConfigHelper.AddGt<TSearch, TEntity>(that, searchCondition),
                SearchType.Ge => QueryConfigHelper.AddGe<TSearch, TEntity>(that, searchCondition),
                SearchType.Lt => QueryConfigHelper.AddLt<TSearch, TEntity>(that, searchCondition),
                SearchType.Le => QueryConfigHelper.AddLe<TSearch, TEntity>(that, searchCondition),
                SearchType.None => throw new FrameException($"未指定{nameof(searchType)}", new ArgumentException()),
                _ => throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null),
            };

            whereLambdas.AddRange(expressionList);
        }

        return whereLambdas;
    }

    /// <inheritdoc cref="ToExpressionList(Dictionary{SearchType, List{string}})"/>
    public List<Expression<Func<TEntity, bool>>> ToExpressionList(Dictionary<SearchType, List<string>> searchConditionDict)
    {
        return ToExpressionList(this, searchConditionDict);
    }

    /// <summary>
    /// 获得搜索条件.
    /// 注:
    /// 1. 会从模型标注的 SearchTypeAttribute 中加载一次
    /// 2. 手写 whereLambda[SearchType.xx] =... 的优先级 > SearchTypeAttribute
    /// </summary>
    /// <param name="searchTypeConfig">默认情况下传入的对象 whereLambda[SearchType.xx] 对象</param>
    /// <returns>(搜索类型, List{对应的属性} )</returns>
    public static Dictionary<SearchType, List<string>> GetSearchCondition(Dictionary<SearchType, List<string>> searchTypeConfig)
    {
        var props = ReflectionHelper.GetProperties(typeof(TSearch));
        foreach (PropertyInfo prop in props)
        {
            var attrs = ReflectionHelper.GetAttributeForProperty<SearchTypeAttribute>(prop, false);
            foreach (SearchTypeAttribute item in attrs)
            {
                if (!searchTypeConfig.ContainsKey(item.SearchType))
                {
                    searchTypeConfig.Add(item.SearchType, new List<string>());
                }

                if (!searchTypeConfig[item.SearchType].Contains(prop.Name))
                {
                    searchTypeConfig[item.SearchType].Add(prop.Name);
                }
            }
        }

        return searchTypeConfig;
    }

    /// <inheritdoc cref="GetSearchCondition(Dictionary{SearchType, List{string}})"/>
    public Dictionary<SearchType, List<string>> GetSearchConditionDict()
    {
        var searchConditionDict = GetSearchCondition(this._dictSearhType);
        return searchConditionDict;
    }

    /// <summary>
    /// implicit 转换
    /// </summary>
    /// <param name="that"></param>
    public static implicit operator List<Expression<Func<TEntity, bool>>>(QueryConfig<TSearch, TEntity> that)
    {
        var whereLambdas = ToExpressionList(that);
        return whereLambdas;
    }

    public List<Expression<Func<TEntity, bool>>> ToExpressionListForEF()
    {
        return ToExpressionList(this);
    }

    #endregion
}

///// <summary>
///// 适合TEntity ==  TSearch,
///// 这种方式的, 没法用 dataRange 和 timeRange,
///// 不建议使用这个
///// </summary>
///// <typeparam name="TSearch"></typeparam>
//public class WhereLambda<TSearch> : WhereLambda<TSearch, TSearch>
//    where TSearch : class
//{
//    public WhereLambda(TSearch search) : base(search)
//    {
//    }
//}