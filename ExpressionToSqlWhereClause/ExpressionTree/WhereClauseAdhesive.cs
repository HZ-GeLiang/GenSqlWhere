using ExpressionToSqlWhereClause.ExpressionTree.Adapter;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.ExpressionTree
{
    /// <summary>
    /// 参数化sql的信息
    /// </summary>
    public class SqlClauseParametersInfo
    {
        /// <summary>
        /// 参数化的key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 参数化的sql子句的符号
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 参数化的值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 参数化的sql子句
        /// </summary>
        public string SqlClause { get; set; }

        /// <summary>
        /// 是否是函数
        /// </summary>
        public bool IsDbFunction { get; set; }
    }

    /// <summary>
    /// Where子句胶粘剂
    /// </summary>
    public class WhereClauseAdhesive
    {
        public WhereClauseAdhesive(ISqlAdapter sqlAdapter)
        {
            this.SqlAdapter = sqlAdapter ?? new DefaultSqlAdapter();
        }

        /// <summary>
        /// 参数化查询的值, 参数名-参数值
        /// </summary>
        private Dictionary<string, SqlClauseParametersInfo> _parameters = new();

        public Dictionary<string, SqlClauseParametersInfo>.KeyCollection GetParameterKeys() => _parameters.Keys;

        public SqlClauseParametersInfo GetParameter(string key)
        {
            if (!this._parameters.ContainsKey(key))
            {
                this._parameters.Add(key, new() { Key = key });
            }
            return this._parameters[key];
        }

        public bool ContainsParameter(string key) => _parameters.ContainsKey(key);

        public void RemoveParameter(string key)
        {
            if (this._parameters.ContainsKey(key))
            {
                this._parameters.Remove(key);
            }
        }

        /// <summary>
        /// sql 适配器,
        /// 目前是用来处理 属性名翻译为sql时的处理,
        /// 如mysql 可以翻译为 `Name`  , mssql 可以翻译为 [Name]  等等,
        /// 我这里的 处理目前是什么都没做, 直接翻译为 Name
        /// </summary>
        public ISqlAdapter SqlAdapter { get; }
    }
}
