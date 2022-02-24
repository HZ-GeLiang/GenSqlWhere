namespace ExpressionToSqlWhereClause
{
    public class ExpressionToSqlWhereClauseException : System.Exception
    {
        public string ErrorNo { get; } // “µŒÒ“Ï≥£±‡∫≈


        public ExpressionToSqlWhereClauseException(string errorNo, string message) : base(message)
        {
            ErrorNo = errorNo;
        }

        public ExpressionToSqlWhereClauseException(string errorNo, string message, System.Exception e) : base(message, e)
        {
            ErrorNo = errorNo;
        }

        public ExpressionToSqlWhereClauseException(string message) : base(message)
        {
        }

        public ExpressionToSqlWhereClauseException(string message, System.Exception e) : base(message, e)
        {
        }


    }
}
