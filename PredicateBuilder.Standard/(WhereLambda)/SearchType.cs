namespace PredicateBuilder.Standard
{
    public enum SearchType
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        Like = 1,

        /// <summary>
        /// 只会翻译成 Equal(=号) 当数据库的值包含,号时且用Equal查询时, 就只能用这个
        /// </summary>
        Equal = 2,

        /// <summary>
        /// 实际翻译成 in 还是 Equal , 根据 split() 后的个数而定
        /// </summary>
        In = 3,

        /// <summary>
        /// 日期的区间
        /// 包含开始值,不包含结束值
        /// [xxxStart, xxxEnd)
        /// </summary>
        DateTimeRange = 4,

        /// <summary>
        /// 数值的范围
        /// 包含开始值与结束值
        /// [xxxLeft, xxxRight]
        /// </summary>
        NumberRange = 5,
    }
}
