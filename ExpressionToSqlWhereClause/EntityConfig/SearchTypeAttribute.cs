using System;

namespace ExpressionToSqlWhereClause.EntityConfig
{
    /// <summary>
    /// 搜索类型配置
    /// </summary>
    public class SearchTypeAttribute : Attribute
    {
        /// <summary>
        /// 搜索类型
        /// </summary>
        public SearchType SearchType { get; private set; }

        ///
        public SearchTypeAttribute(SearchType searchType)
        {
            this.SearchType = searchType;
        }
    }
}