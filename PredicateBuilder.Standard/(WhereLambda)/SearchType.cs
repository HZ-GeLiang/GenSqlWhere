using System;

namespace PredicateBuilder.Standard
{
    /// <summary>
    /// 
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// 未配置
        /// </summary>
        none = 0,

        /// <summary>
        /// like %%
        /// </summary>
        like,

        /// <summary>
        /// 只会翻译成 Equal(=号) 当数据库的值包含,号时且用Equal查询时, 就只能用这个
        /// </summary>
        eq,

        /// <summary>
        /// 实际翻译成 in 还是 Equal , 根据 split() 后的个数而定
        /// </summary>
        @in,

        /// <summary>
        /// 日期的区间
        /// 包含开始值,不包含结束值
        /// [xxxStart, xxxEnd)
        /// </summary>
        datetimeRange,

        /// <summary>
        /// 数值的范围
        /// 包含开始值与结束值
        /// [xxxLeft, xxxRight]
        /// </summary>
        numberRange,

        /// <summary>
        ///  greater than
        /// </summary>
        gt,

        #region 还未实现
        //todo: 还未实现
        /// <summary>
        /// 大于或等于(LE)
        /// </summary>
        [Obsolete("还未实现")] ge,

        /// <summary>
        ///  less Than
        /// </summary>
        [Obsolete("还未实现")] lt,

        /// <summary>
        /// 小于或等于(LE)
        /// </summary>
        [Obsolete("还未实现")] le,

        /// <summary>
        /// not equal
        /// </summary>
        [Obsolete("还未实现")] neq,

        /// <summary>
        /// 翻译成 like '%xxx'
        /// </summary>
        [Obsolete("还未实现")] likeLeft,

        /// <summary>
        /// 翻译成 like 'xxx%'
        /// </summary>
        [Obsolete("还未实现")] likeRight,

        #endregion

    }
}
