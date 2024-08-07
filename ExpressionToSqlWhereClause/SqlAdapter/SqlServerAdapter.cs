﻿using ExpressionToSqlWhereClause.ExpressionTree.Adapter;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause
{
    public class SqlServerAdapter : ISqlAdapter
    {
        /// <inheritdoc/>
        public virtual string FormatColumnName(string name)
        {
            return $"[{name}]";
        }
    }
}