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
        internal static List<Expression<Func<TEntity, bool>>> AddRange<TEntity, TSearchModel>(
            this List<Expression<Func<TEntity, bool>>> whereLambdas,
            TSearchModel searchModel,
            SearchType searchType,
            List<string> props
            )
        {
            List<Expression<Func<TEntity, bool>>> expressionList = searchType switch
            {
                SearchType.Like => WhereLambdaHelper.AddLike<TEntity, TSearchModel>(searchModel, props),
                SearchType.LikeLeft => WhereLambdaHelper.AddLikeLeft<TEntity, TSearchModel>(searchModel, props),
                SearchType.LikeRight => WhereLambdaHelper.AddLikeRight<TEntity, TSearchModel>(searchModel, props),
                SearchType.Eq => WhereLambdaHelper.AddEqual<TEntity, TSearchModel>(searchModel, props),
                SearchType.Neq => WhereLambdaHelper.AddNotEqual<TEntity, TSearchModel>(searchModel, props),
                SearchType.In => WhereLambdaHelper.AddIn<TEntity, TSearchModel>(searchModel, props),
                SearchType.TimeRange => WhereLambdaHelper.AddTimeRange<TEntity, TSearchModel>(searchModel, props),
                SearchType.NumberRange => WhereLambdaHelper.AddNumberRange<TEntity, TSearchModel>(searchModel, props),
                SearchType.Gt => WhereLambdaHelper.AddGt<TEntity, TSearchModel>(searchModel, props),
                SearchType.Ge => WhereLambdaHelper.AddGe<TEntity, TSearchModel>(searchModel, props),
                SearchType.Lt => WhereLambdaHelper.AddLt<TEntity, TSearchModel>(searchModel, props),
                SearchType.Le => WhereLambdaHelper.AddLe<TEntity, TSearchModel>(searchModel, props),
                SearchType.None => throw new ExpressionToSqlWhereClauseException($"未指定{nameof(searchType)}", new ArgumentException()),
                _ => throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null),
            };

            whereLambdas.AddRange(expressionList);

            return whereLambdas;
        }

        ///
        public static WhereLambda<TEntity, TSearchModel> CrateWhereLambda<TEntity, TSearchModel>(this TSearchModel searchModel, Action<TEntity> _)
            where TSearchModel : class
            where TEntity : class
        {
            return new WhereLambda<TEntity, TSearchModel>(searchModel);
        }

        /// <summary>
        /// 适合TEntity ==  TSearchModel,
        /// 这种方式的, 没法用 timeRange
        /// </summary>
        /// <typeparam name="TSearchModel"></typeparam>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public static WhereLambda<TSearchModel, TSearchModel> CrateWhereLambda<TSearchModel>(this TSearchModel searchModel) where TSearchModel : class
        {
            return new WhereLambda<TSearchModel, TSearchModel>(searchModel);
        }

    }
}