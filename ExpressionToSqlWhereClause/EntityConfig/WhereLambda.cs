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

        private Dictionary<SearchType, List<string>> _dictSearhType = new Dictionary<SearchType, List<string>>();

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
            var searchCondition = GetSearchCondition(this._dictSearhType);
            var whereLambdas = ToExpressionList(this.Search, searchCondition);
            var expression = ToExpression(whereLambdas);
            return expression;
        }

        public static implicit operator Expression<Func<TEntity, bool>>(WhereLambda<TEntity, TSearch> that)
        {
            var searchCondition = GetSearchCondition(that._dictSearhType);
            var whereLambdas = ToExpressionList(that.Search, searchCondition);
            var expression = ToExpression(whereLambdas);
            return expression;
        }


        internal static Expression<Func<TEntity, bool>> ToExpression<TEntity>(List<Expression<Func<TEntity, bool>>> whereLambdas)
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

        /// <summary>
        /// 从模型属性的 SearchTypeAttribute 获得 SearchCondition
        /// </summary>
        /// <param name="searchTypeConfig"></param>
        /// <returns></returns>
        internal static Dictionary<SearchType, List<string>> GetSearchCondition(Dictionary<SearchType, List<string>> searchTypeConfig)
        {
            var config = searchTypeConfig.DeepClone();
            var props = ReflectionHelper.GetProperties(typeof(TSearch));
            foreach (System.Reflection.PropertyInfo prop in props)
            {
                var attrs = ReflectionHelper.GetAttributeForProperty<SearchTypeAttribute>(prop, false);
                foreach (SearchTypeAttribute item in attrs)
                {
                    if (!config.ContainsKey(item.SearchType))
                    {
                        config.Add(item.SearchType, new List<string>());
                    }

                    //优先级比 手动写的要低
                    if (!config[item.SearchType].Contains(prop.Name))
                    {
                        config[item.SearchType].Add(prop.Name);
                    }
                }
            }

            return config;
        }

        #region ToExpressionList

        /// <summary>
        /// 获得表达式树的写法,可以给ef用(不含sql内置函数的那种)
        /// </summary>
        /// <param name="searchModel">input对象</param>
        /// <param name="searchCondition">input对象的搜索条件配置</param>
        /// <returns></returns>
        internal static List<Expression<Func<TEntity, bool>>> ToExpressionList(TSearch searchModel, Dictionary<SearchType, List<string>> searchCondition)
        {
            searchCondition = GetSearchCondition(searchCondition);

            var whereLambdas = new List<Expression<Func<TEntity, bool>>>();

            foreach (var searchType in _addOrder)
            {
                if (searchType == SearchType.None || !searchCondition.ContainsKey(searchType))
                {
                    //throw new ExpressionToSqlWhereClauseException($"参数{nameof(dict)}不包含{nameof(searchType)}值:{searchType}");
                    continue;
                }
                var list = searchCondition[searchType];
                if (list != null && list.Count > 0)
                {
                    whereLambdas.AddRange(searchModel, searchType, list);
                }
            }

            return whereLambdas;
        }

        public static implicit operator List<Expression<Func<TEntity, bool>>>(WhereLambda<TEntity, TSearch> that)
        {
            var searchCondition = GetSearchCondition(that._dictSearhType);
            var whereLambdas = ToExpressionList(that.Search, searchCondition);
            return whereLambdas;
        }

        public List<Expression<Func<TEntity, bool>>> ToExpressionListForEF()
        {
            return ToExpressionList(this.Search, this._dictSearhType);
        }

        #endregion
    }
}