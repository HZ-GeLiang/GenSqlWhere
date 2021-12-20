using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToSqlWhereClause
{
    internal class ParseResult
    {
        /// <summary>
        /// where子句
        /// </summary>
        public StringBuilder WhereClause { get; set; }
        public SqlClauseParametersInfo SqlClauseParametersInfo { get; set; }
        
        /// <summary>
        /// 调用方是否需要额外添加参数 , 这个值为true 时,同属性的 MemberInfo 不能为null
        /// </summary>
        public bool NeedAddPara { get; set; } = false;

        /// <summary>
        /// 额外添加参数时的辅助信息3: MemberExpression
        /// </summary>
        public MemberExpression MemberExpression { get; set; } = null;

        /// <summary>
        /// 额外添加参数时的辅助信息1: Expression的MemberInfo
        /// </summary>
        public MemberInfo MemberInfo { get; set; } = null;


    }
}
