namespace ExpressionToSqlWhereClause.EntitySearchBuilder
{
    public enum FilterOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,


        /// <summary>
        /// 日期比较
        /// </summary>
        Between

    }
}
