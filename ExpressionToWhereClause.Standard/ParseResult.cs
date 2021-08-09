using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause.Standard
{
    internal class ParseResult
    {
        /// <summary>
        /// where子句
        /// </summary>
        public StringBuilder WhereClause { get; set; }

        /// <summary>
        /// 调用方是否需要额外添加参数
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
