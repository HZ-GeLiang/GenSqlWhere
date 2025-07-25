﻿namespace ExpressionToSqlWhereClause.EntitySearchBuilder;

/// <summary>
/// 搜索类型配置
/// </summary>
public class SearchTypeAttribute : Attribute
{
    /// <summary>
    /// 搜索类型
    /// </summary>
    public SearchType SearchType { get; private set; }

    /// <inheritdoc/>
    public SearchTypeAttribute(SearchType searchType)
    {
        this.SearchType = searchType;
    }
}