using ExpressionToSqlWhereClause.Exceptions;
using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.EntityConfig
{
    /// <summary>
    /// 适合TEntity ==  TSearch,
    /// 这种方式的, 没法用 dataRange 和 timeRange
    /// </summary>
    /// <typeparam name="TSearch"></typeparam>
    public class WhereLambda<TSearch> : WhereLambda<TSearch, TSearch>
        where TSearch : class
    {

    }

    /// <summary>
    /// TSearch 转 TEntity 的表达式树的配置
    /// </summary>
    /// <typeparam name="TEntity">数据表实体类型</typeparam>
    /// <typeparam name="TSearch">检索对象 ,一般是 xxInput 对象, 注: 不要使用多态,会报错的</typeparam>
    public class WhereLambda<TEntity, TSearch>
        where TEntity : class
        where TSearch : class
    {
        //添加where的排序顺序: 目的是尽可能的让索引生效(也就是like必须是最后的,其他只能随意)
        private static readonly SearchType[] _addOrder = {
            SearchType.None,
            SearchType.In,
            SearchType.Eq,
            SearchType.TimeRange,
            SearchType.NumberRange,
            SearchType.Gt,
            SearchType.Ge,
            SearchType.Lt,
            SearchType.Le,
            SearchType.Neq,
            SearchType.LikeRight,
            SearchType.LikeLeft,
            SearchType.Like,
        };

        /// <summary>
        /// 构造器创建,需要 赋值 SearchModel 属性才能使用
        /// </summary>
        public WhereLambda() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search">检索对象</param>
        public WhereLambda(TSearch search)
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
        public Dictionary<string, Expression<Func<TSearch, bool>>> WhereIf { get; set; } = new Dictionary<string, Expression<Func<TSearch, bool>>>();


        /// <summary>
        /// 检索对象的配置
        /// </summary>
        private Dictionary<SearchType, List<string>> _dictSearhType = new();

        public List<string> this[SearchType searchType]
        {
            get { return _dictSearhType[searchType]; }
            set { _dictSearhType[searchType] = value; }
        }

        #region ToExpression

        /// <summary>
        /// 转表达式树
        /// </summary>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> ToExpression()
        {
            var whereLambdas = ToExpressionList(this);
            var expression = ToExpression(whereLambdas);
            return expression;
        }

        ///
        public static implicit operator Expression<Func<TEntity, bool>>(WhereLambda<TEntity, TSearch> that)
        {
            var whereLambdas = ToExpressionList(that);
            var expression = ToExpression(whereLambdas);
            return expression;
        }

        ///
        internal static Expression<Func<TEntity, bool>> ToExpression<TEntity>(List<Expression<Func<TEntity, bool>>> whereLambdas)
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

        internal Dictionary<SearchType, List<string>> SearchCondition { get; set; } = null;


        #region ToExpressionList

        /// <summary>
        /// 获得表达式树的写法,可以给ef用(不含sql内置函数的那种)
        /// </summary> 
        /// <returns></returns>
        internal static List<Expression<Func<TEntity, bool>>> ToExpressionList(WhereLambda<TEntity, TSearch> that)
        {
            that.SearchCondition = GetSearchCondition(that._dictSearhType);

            var whereLambdas = new List<Expression<Func<TEntity, bool>>>();

            foreach (SearchType searchType in _addOrder)
            {
                if (searchType == SearchType.None ||
                    that.SearchCondition.ContainsKey(searchType) == false ||
                    that.SearchCondition[searchType] == null ||
                    that.SearchCondition[searchType].Count <= 0
                   )
                {
                    continue;
                }

                List<Expression<Func<TEntity, bool>>> expressionList = searchType switch
                {
                    SearchType.Like => WhereLambdaHelper.AddLike<TEntity, TSearch>(that, searchType),
                    SearchType.LikeLeft => WhereLambdaHelper.AddLikeLeft<TEntity, TSearch>(that, searchType),
                    SearchType.LikeRight => WhereLambdaHelper.AddLikeRight<TEntity, TSearch>(that, searchType),
                    SearchType.Eq => WhereLambdaHelper.AddEqual<TEntity, TSearch>(that, searchType),
                    SearchType.Neq => WhereLambdaHelper.AddNotEqual<TEntity, TSearch>(that, searchType),
                    SearchType.In => WhereLambdaHelper.AddIn<TEntity, TSearch>(that, searchType),
                    SearchType.TimeRange => WhereLambdaHelper.AddTimeRange<TEntity, TSearch>(that, searchType),
                    SearchType.NumberRange => WhereLambdaHelper.AddNumberRange<TEntity, TSearch>(that, searchType),
                    SearchType.Gt => WhereLambdaHelper.AddGt<TEntity, TSearch>(that, searchType),
                    SearchType.Ge => WhereLambdaHelper.AddGe<TEntity, TSearch>(that, searchType),
                    SearchType.Lt => WhereLambdaHelper.AddLt<TEntity, TSearch>(that, searchType),
                    SearchType.Le => WhereLambdaHelper.AddLe<TEntity, TSearch>(that, searchType),
                    SearchType.None => throw new FrameException($"未指定{nameof(searchType)}", new ArgumentException()),
                    _ => throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null),
                };

                whereLambdas.AddRange(expressionList);
            }

            return whereLambdas;
        }


        /// <summary>
        /// 从模型属性的 SearchTypeAttribute 获得 SearchCondition, 然后追加到 searchTypeConfig
        /// </summary>
        /// <param name="searchTypeConfig"></param>
        /// <returns></returns>
        private static Dictionary<SearchType, List<string>> GetSearchCondition(Dictionary<SearchType, List<string>> searchTypeConfig)
        {
            var props = ReflectionHelper.GetProperties(typeof(TSearch));
            foreach (System.Reflection.PropertyInfo prop in props)
            {
                var attrs = ReflectionHelper.GetAttributeForProperty<SearchTypeAttribute>(prop, false);
                foreach (SearchTypeAttribute item in attrs)
                {
                    if (!searchTypeConfig.ContainsKey(item.SearchType))
                    {
                        searchTypeConfig.Add(item.SearchType, new List<string>());
                    }

                    //优先级比 手动写的要低
                    if (!searchTypeConfig[item.SearchType].Contains(prop.Name))
                    {
                        searchTypeConfig[item.SearchType].Add(prop.Name);
                    }
                }
            }

            return searchTypeConfig;
        }


        public static implicit operator List<Expression<Func<TEntity, bool>>>(WhereLambda<TEntity, TSearch> that)
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
}