using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityToSqlWhereClauseConfig
{
    public static class WhereLambdaExtensions
    {
        ////todo:转换为Where子句
        //public static (string WhereClause, Dictionary<string, object> Parameters) ToWhereClause<TEntity, TSearchModel>(this WhereLambda<TEntity, TSearchModel> whereLambda)
        //    where TEntity : class
        //    where TSearchModel : class
        //{
        //    return whereLambda.ToExpression().ToWhereClause();
        //}

        internal static List<Expression<Func<TEntity, bool>>> AddRange<TEntity, TSearchModel>(
            this List<Expression<Func<TEntity, bool>>> whereLambdas,
            TSearchModel searchModel, SearchType searchType, List<string> props)
        {
            List<Expression<Func<TEntity, bool>>> expressionList = searchType switch
            {
                SearchType.none => throw new Exceptions.EntityToSqlWhereCaluseConfigException($"未指定{nameof(searchType)}", new ArgumentException()),
                SearchType.like => WhereLambdaHelper.AddLike<TEntity, TSearchModel>(searchModel, props),
                SearchType.likeLeft => WhereLambdaHelper.AddLikeLeft<TEntity, TSearchModel>(searchModel, props),
                SearchType.likeRight => WhereLambdaHelper.AddLikeRight<TEntity, TSearchModel>(searchModel, props),
                SearchType.eq => WhereLambdaHelper.AddEqual<TEntity, TSearchModel>(searchModel, props),
                SearchType.neq => WhereLambdaHelper.AddNotEqual<TEntity, TSearchModel>(searchModel, props),
                SearchType.@in => WhereLambdaHelper.AddIn<TEntity, TSearchModel>(searchModel, props),
                SearchType.datetimeRange => WhereLambdaHelper.AddDateTimeRange<TEntity, TSearchModel>(searchModel, props),
                SearchType.numberRange => WhereLambdaHelper.AddNumberRange<TEntity, TSearchModel>(searchModel, props),
                SearchType.gt => WhereLambdaHelper.AddGt<TEntity, TSearchModel>(searchModel, props),
                SearchType.ge => WhereLambdaHelper.AddGe<TEntity, TSearchModel>(searchModel, props),
                SearchType.lt => WhereLambdaHelper.AddLt<TEntity, TSearchModel>(searchModel, props),
                SearchType.le => WhereLambdaHelper.AddLe<TEntity, TSearchModel>(searchModel, props),
                _ => throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null),
            };

            whereLambdas.AddRange(expressionList);

            return whereLambdas;
        }

        public static WhereLambda<TEntity, TSearchModel> CrateWhereLambda<TSearchModel, TEntity>(this TSearchModel searchModel, Action<TEntity> _)
        {
            return new WhereLambda<TEntity, TSearchModel>(searchModel);
        }
    }
}