namespace EntityToSqlWhereClauseConfig.Exceptions
{
    public class EntityToSqlWhereCaluseConfigException : System.Exception
    {
        public string ErrorNo { get; } // “µŒÒ“Ï≥£±‡∫≈


        public EntityToSqlWhereCaluseConfigException(string errorNo, string message) : base(message)
        {
            ErrorNo = errorNo;
        }

        public EntityToSqlWhereCaluseConfigException(string errorNo, string message, System.Exception e) : base(message, e)
        {
            ErrorNo = errorNo;
        }

        public EntityToSqlWhereCaluseConfigException(string message) : base(message)
        {
        }

        public EntityToSqlWhereCaluseConfigException(string message, System.Exception e) : base(message, e)
        {
        }


    }
}
