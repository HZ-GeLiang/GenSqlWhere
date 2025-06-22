namespace ExpressionToSqlWhereClause.EntitySearchBuilder;

/// <summary>
/// 搜索类型
/// </summary>
public enum SearchType
{
    /// <summary>
    /// 未配置
    /// </summary>
    None = 0,

    /// <summary>
    /// like %%
    /// </summary>
    Like,

    /// <summary>
    /// 只会翻译成 Equal(=号) 当数据库的值包含,号时且用Equal查询时, 就只能用这个
    /// </summary>
    Eq,

    /// <summary>
    /// 实际翻译成 in 还是 Equal, 根据 split() 后的个数而定(sqlFunc除外)
    /// </summary>
    In,

    /// <summary>
    /// 时间范围(只接受日期), day /hour /minute /sec
    /// 结束时间会在当前精度+1(最小为秒, 然后秒的精度识别有问题), 然后使用 小于符号, 即: >=@xxx And ＜ @xxx1
    /// 1:有区间范围 [xxxStart, xxxEnd] => 包含开始值,包含结束值
    /// 2:没有区间范围  xxx =>(取当前时间精度: 如 当前这天, 当前这一小时, 当前这一分钟, 当前这一秒)
    /// </summary>
    TimeRange,

    /// <summary>
    /// 数值的范围(只接受数字)
    /// 1:有区间范围 [xxxLeft, xxxRight]=> 包含开始值,包含结束值
    /// 2:没有区间范围  xxx => 还是上面的规则, 即: >=@xxx And &lt;= @xxx1
    /// </summary>
    NumberRange,

    /// <summary>
    ///  greater than
    /// </summary>
    Gt,

    /// <summary>
    /// 大于或等于(GE)
    /// </summary>
    Ge,

    /// <summary>
    ///  less Than
    /// </summary>
    Lt,

    /// <summary>
    /// 小于或等于(LE)
    /// </summary>
    Le,

    /// <summary>
    /// not equal
    /// </summary>
    Neq,

    /// <summary>
    /// 翻译成 like '%xxx'
    /// </summary>
    LikeLeft,

    /// <summary>
    /// 翻译成 like 'xxx%'
    /// </summary>
    LikeRight,
}