using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EntityToSqlWhereClauseConfig.ExtensionMethod;

namespace EntityToSqlWhereClauseConfig
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
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
            SearchType.datetimeRange,
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

        #region ToExpressionList


        public static implicit operator List<Expression<Func<TEntity, bool>>>(WhereLambda<TEntity, TSearchModel> that)
        {
            var whereLambdas = ToExpressionList(that.SearchModel, that._dictSearhType);
            return whereLambdas;
        }

        /// <summary>
        /// 获得表达式树的写法
        /// </summary>
        /// <param name="searchModel">input对象</param>
        /// <param name="searchCondition">input对象的搜索条件配置</param>
        /// <returns></returns>
        private static List<Expression<Func<TEntity, bool>>> ToExpressionList(TSearchModel searchModel, Dictionary<SearchType, List<string>> searchCondition)
        {
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
        #endregion

        #region ToExpression

        public Expression<Func<TEntity, bool>> ToExpression()
        {
            var whereLambdas = ToExpressionList(this.SearchModel, this._dictSearhType);
            return ToExpression(whereLambdas);
        }

        public static implicit operator Expression<Func<TEntity, bool>>(WhereLambda<TEntity, TSearchModel> that)
        {
            List<Expression<Func<TEntity, bool>>> whereLambdas =
                ToExpressionList(that.SearchModel, that._dictSearhType);
            return ToExpression(whereLambdas);
        }

        private static Expression<Func<TEntity, bool>> ToExpression(List<Expression<Func<TEntity, bool>>> whereLambdas)
        {
            Expression<Func<TEntity, bool>> exp = null;
            if (whereLambdas == null || whereLambdas.Count <= 0)
            {
                return exp;
            }
            exp = whereLambdas[0];
            if (whereLambdas.Count == 1)
            {
                return exp;
            }
            else
            {
                for (int i = 1; i < whereLambdas.Count; i++)
                {
                    exp = exp.And(whereLambdas[i]);
                }
                return exp;
            }
        }
        #endregion
    }


}