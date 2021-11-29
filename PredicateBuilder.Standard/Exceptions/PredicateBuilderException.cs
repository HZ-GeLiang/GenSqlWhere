namespace PredicateBuilder.Exceptions
{
    public class PredicateBuilderException : System.Exception
    {
        public string ErrorNo { get; } // 业务异常编号


        public PredicateBuilderException(string errorNo, string message) : base(message)
        {
            ErrorNo = errorNo;
        }

        public PredicateBuilderException(string errorNo, string message, System.Exception e) : base(message, e)
        {
            ErrorNo = errorNo;
        }

        public PredicateBuilderException(string message) : base(message)
        {
        }

        public PredicateBuilderException(string message, System.Exception e) : base(message, e)
        {
        }


    }
}
