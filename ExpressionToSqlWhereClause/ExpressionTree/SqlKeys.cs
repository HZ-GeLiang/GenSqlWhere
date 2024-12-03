namespace ExpressionToSqlWhereClause.ExpressionTree;

/// <summary>
/// SQL查询中使用的关键字和符号常量类
/// </summary>
internal class SqlKeys
{
    #region 逻辑运算符
    /// <summary>
    /// 表达式树中的AND逻辑符号
    /// </summary>
    public const string LogicSymbolAnd = "And";

    /// <summary>
    /// 表达式树中的OR逻辑符号
    /// </summary>
    public const string LogicSymbolOr = "Or";
    #endregion

    #region SQL连接符
    /// <summary>
    /// SQL语句中的AND连接符
    /// </summary>
    public const string And = " And ";

    /// <summary>
    /// SQL语句中的OR连接符
    /// </summary>
    public const string Or = " Or ";
    #endregion

    #region 比较运算符
    /// <summary>
    /// 等于运算符
    /// </summary>
    public const string Equal = "=";

    /// <summary>
    /// 小于运算符
    /// </summary>
    public const string LessThan = "<";

    /// <summary>
    /// 大于运算符
    /// </summary>
    public const string GreaterThan = ">";

    /// <summary>
    /// 大于等于运算符
    /// </summary>
    public const string GreaterThanOrEqual = ">=";

    /// <summary>
    /// 小于等于运算符
    /// </summary>
    public const string LessThanOrEqual = "<=";

    /// <summary>
    /// 不等于运算符
    /// </summary>
    public const string NotEqual = "<>";

    /// <summary>
    /// IN运算符
    /// </summary>
    public const string @in = "In";
    #endregion

    #region 字符串比较运算符
    /// <summary>
    /// 等于符号
    /// </summary>
    public const string Equals_symbol = "= {0}";

    /// <summary>
    /// 等于值符号
    /// </summary>
    public const string Equals_valueSymbol = "{0}";

    /// <summary>
    /// 以...开始符号
    /// </summary>
    public const string StartsWith_symbol = "Like {0}";

    /// <summary>
    /// 以...开始值符号
    /// </summary>
    public const string StartsWith_valueSymbol = "{0}%";

    /// <summary>
    /// 以...结束符号
    /// </summary>
    public const string EndsWith_symbol = "Like {0}";

    /// <summary>
    /// 以...结束值符号
    /// </summary>
    public const string EndsWith_valueSymbol = "%{0}";

    /// <summary>
    /// 包含符号
    /// </summary>
    public const string Contains_symbol = "Like {0}";

    /// <summary>
    /// 包含值符号
    /// </summary>
    public const string Contains_valueSymbol = "%{0}%";
    #endregion
}