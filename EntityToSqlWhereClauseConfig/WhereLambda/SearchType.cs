// ReSharper disable once CheckNamespace
namespace EntityToSqlWhereClauseConfig
{
    public class SearchTypeAttribute : System.Attribute
    {
        public SearchType SearchType { get; private set; }

        public SearchTypeAttribute(SearchType searchType)
        {
            this.SearchType = searchType;
        }
    }

    /// <summary>
    /// 搜索类型
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
        /// 实际翻译成 in 还是 Equal , 根据 split() 后的个数而定 (sqlFunc除外)
        /// </summary>
        @in,

        /// <summary>
        /// 日期的区间(只接受日期),
        /// 结束时间会在当前精度+1(最小为秒, 然后秒的精度识别有问题), 然后使用 小于符号, 即: >=@xxx And < @xxx1
        /// 1:有区间范围 [xxxStart, xxxEnd] => 包含开始值,包含结束值
        /// 2:没有区间范围  xxx =>(取当前时间精度: 如 当前这天, 当前这一小时, 当前这一分钟, 当前这一秒)
        /// </summary>
        datetimeRange,

        /// <summary>
        /// 数值的范围(只接受数字)
        /// 1:有区间范围 [xxxLeft, xxxRight]=> 包含开始值,包含结束值
        /// 2:没有区间范围  xxx => 还是上面的规则, 即: >=@xxx And <= @xxx1
        /// </summary>
        numberRange,

        /// <summary>
        ///  greater than
        /// </summary>
        gt,


        /// <summary>
        /// 大于或等于(LE)
        /// </summary>
        ge,

        /// <summary>
        ///  less Than
        /// </summary>
        lt,

        /// <summary>
        /// 小于或等于(LE)
        /// </summary>
        le,

        /// <summary>
        /// not equal
        /// </summary>
        neq,

        /// <summary>
        /// 翻译成 like '%xxx'
        /// </summary>
        likeLeft,

        /// <summary>
        /// 翻译成 like 'xxx%'
        /// </summary>
        likeRight,

    }
}
