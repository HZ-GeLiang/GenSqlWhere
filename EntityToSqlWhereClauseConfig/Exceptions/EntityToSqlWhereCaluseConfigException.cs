using System;

namespace EntityToSqlWhereClauseConfig.Exceptions
{
    public class EntityToSqlWhereCaluseConfigException : Exception
    {
        public string ErrorNo { get; } // ҵ���쳣���


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