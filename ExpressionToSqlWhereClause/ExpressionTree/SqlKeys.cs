namespace ExpressionToSqlWhereClause.ExpressionTree
{
    internal class SqlKeys
    {
        //逻辑
        public const string LogicSymbolAnd = "And";

        public const string LogicSymbolOr = "Or";

        //关系
        public const string And = " And ";

        public const string Or = " Or ";

        //比较
        public const string Equal = "=";

        public const string LessThan = "<";
        public const string GreaterThan = ">";
        public const string GreaterThanOrEqual = ">=";
        public const string LessThanOrEqual = "<=";
        public const string NotEqual = "<>";
        public const string @in = "In";

        // string 的 比较
        public const string Equals_symbol = "= {0}";

        public const string Equals_valueSymbol = "{0}";

        public const string StartsWith_symbol = "Like {0}";
        public const string StartsWith_valueSymbol = "{0}%";

        public const string EndsWith_symbol = "Like {0}";
        public const string EndsWith_valueSymbol = "%{0}";

        public const string Contains_symbol = "Like {0}";
        public const string Contains_valueSymbol = "%{0}%";
    }
}