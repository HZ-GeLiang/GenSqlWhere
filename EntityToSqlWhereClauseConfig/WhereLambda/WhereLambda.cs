using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityToSqlWhereClauseConfig.ExtensionMethod;
using EntityToSqlWhereClauseConfig.Helper;

// ReSharper disable once CheckNamespace
namespace EntityToSqlWhereClauseConfig
{
    /// <summary>
    /// 适合TEntity ==  TSearchModel,
    /// 这种方式的, 没法用dataRange 和 timeRange
    /// </summary>
    /// <typeparam name="TSearchModel"></typeparam>
    public class WhereLambda<TSearchModel> : WhereLambda<TSearchModel, TSearchModel>
    {

    }


    /// <summary>
    /// SearchModel 转 Entity的表达式树的配置
    /// </summary>
    /// <typeparam name="TEntity">数据表实体类型</typeparam>
    /// <typeparam name="TSearchModel">不要使用多态,会报错的</typeparam>
    public class WhereLambda<TEntity, TSearchModel>
    {
        /// <summary>
        /// 构造器创建,需要 赋值 SearchModel 属性才能使用
        /// </summary>
        public WhereLambda() { }

        public WhereLambda(TSearchModel searchModel)
        {
            this.SearchModel = searchModel;
        }

        public TSearchModel SearchModel { get; set; }

        internal Dictionary<SearchType, List<string>> _dictSearhType = new Dictionary<SearchType, List<string>>();

        public List<string> this[SearchType searchType]
        {
            get { return _dictSearhType[searchType]; }
            set { _dictSearhType[searchType] = value; }
        }

        //添加where的排序顺序: 目的是尽可能的让索引生效(也就是like必须是最后的,其他只能随意)
        private static readonly SearchType[] _addOrder = {
            SearchType.none,
            SearchType.@in,
            SearchType.eq,
            SearchType.timeRange,
            SearchType.numberRange,
            SearchType.gt,
            SearchType.ge,
            SearchType.lt,
            SearchType.le,
            SearchType.neq,
            SearchType.likeRight,
            SearchType.likeLeft,
            SearchType.like,
        };

        #region ToExpression

        /// <summary>
        /// 转表达式树
        /// </summary>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> ToExpression()
        {
            var searchCondition = GetSearchCondition(this.SearchModel, this._dictSearhType);
            var whereLambdas = WhereLambda<TEntity, TSearchModel>.ToExpressionList(this.SearchModel, searchCondition);
            var expression = WhereLambda<TEntity, TSearchModel>.ToExpression(whereLambdas);
            return expression;
        }


        public static implicit operator Expression<Func<TEntity, bool>>(WhereLambda<TEntity, TSearchModel> that)
        {
            var searchCondition = GetSearchCondition(that.SearchModel, that._dictSearhType);
            var whereLambdas = WhereLambda<TEntity, TSearchModel>.ToExpressionList(that.SearchModel, searchCondition);
            var expression = WhereLambda<TEntity, TSearchModel>.ToExpression(whereLambdas);
            return expression;
        }

        internal static Expression<Func<TEntity, bool>> ToExpression(List<Expression<Func<TEntity, bool>>> whereLambdas)
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

        internal static Dictionary<SearchType, List<string>> GetSearchCondition(TSearchModel searchModel, Dictionary<SearchType, List<string>> searchTypeConfig)
        {
            var config = searchTypeConfig.Clone();
            var props = ReflectionHelper.GetProperties(typeof(TSearchModel));
            foreach (System.Reflection.PropertyInfo prop in props)
            {
                var attrs = ReflectionHelper.GetAttributeForProperty<SearchTypeAttribute>(prop, false);
                foreach (SearchTypeAttribute item in attrs)
                {
                    if (!config.ContainsKey(item.SearchType))
                    {
                        config.Add(item.SearchType, new List<string>());
                    }
                    if (!config[item.SearchType].Contains(prop.Name))
                    {
                        config[item.SearchType].Add(prop.Name);
                    }
                }
            }

            return config;
        }
        #endregion

        #region ToExpressionList

        public static implicit operator List<Expression<Func<TEntity, bool>>>(WhereLambda<TEntity, TSearchModel> that)
        {
            var searchCondition = GetSearchCondition(that.SearchModel, that._dictSearhType);
            var whereLambdas = ToExpressionList(that.SearchModel, searchCondition);
            return whereLambdas;
        }

        /// <summary>
        /// 获得表达式树的写法,可以给ef用(不含sql内置函数的那种)
        /// </summary>
        /// <param name="searchModel">input对象</param>
        /// <param name="searchCondition">input对象的搜索条件配置</param>
        /// <returns></returns>
        internal static List<Expression<Func<TEntity, bool>>> ToExpressionList(TSearchModel searchModel, Dictionary<SearchType, List<string>> searchCondition)
        {
            searchCondition = GetSearchCondition(searchModel, searchCondition);

            var whereLambdas = new List<Expression<Func<TEntity, bool>>>();

            foreach (var searchType in _addOrder)
            {
                if (searchType == SearchType.none || !searchCondition.ContainsKey(searchType))
                {
                    //throw new Exceptions.EntityToSqlWhereCaluseConfigException($"参数{nameof(dict)}不包含{nameof(searchType)}值:{searchType}");
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

        public List<Expression<Func<TEntity, bool>>> ToExpressionListForEf()
        {
            return ToExpressionList(this.SearchModel, this._dictSearhType);
        }


        #endregion
    }
}