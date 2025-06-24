using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSqlWhereClause.ExpressionTree
{
    /// <summary>
    /// 参数化sql的信息
    /// </summary>
    public class SqlClauseParametersInfo
    {
        public SqlClauseParametersInfo()
        { }

        /// <summary>
        /// 参数化的key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 参数化的sql子句的符号
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 参数化的值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 参数化的值(in的时候,分隔成多个),此时, 这个值就有值
        /// </summary>
        public List<SqlClauseListParametersItem> ValueCollection { get; set; }

        /// <summary>
        /// 参数化的sql子句
        /// </summary>
        public string SqlClause { get; set; }

        /// <summary>
        /// 是否是函数
        /// </summary>
        public bool IsDbFunction { get; set; }
    }
}