namespace ExpressionToSqlWhereClause.Consts;

internal class ExpressionFullNameSpaceConst
{
    /// <summary>
    /// 类型
    /// </summary>
    public const string TypedParameter = "System.Linq.Expressions.TypedParameterExpression";

    /// <summary>
    /// 属性
    /// 实际的类型为: System.Linq.Expressions.Expression { System.Linq.Expressions.PropertyExpression}
    /// 应该是内部类,直接写,编译过不了
    /// </summary>
    public const string Property = "System.Linq.Expressions.PropertyExpression";

    /// <summary>
    /// 字段
    /// 实际的类型为:System.Linq.Expressions.Expression { System.Linq.Expressions.FieldExpression}
    /// 应该是内部类,直接写,编译过不了
    /// </summary>
    public const string Field = "System.Linq.Expressions.FieldExpression";

    /// <summary>
    /// 比如??运算符
    /// </summary>
    public const string SimpleBinary = "System.Linq.Expressions.SimpleBinaryExpression";
}