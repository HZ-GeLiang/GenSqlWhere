namespace ExpressionToSqlWhereClause.Exceptions
{
    public class FrameException : System.Exception
    {
        public string ErrorNo { get; } // 业务异常编号

        public FrameException(string errorNo, string message) : base(message)
        {
            ErrorNo = errorNo;
        }

        public FrameException(string errorNo, string message, System.Exception e) : base(message, e)
        {
            ErrorNo = errorNo;
        }

        public FrameException(string message) : base(message)
        {
        }

        public FrameException(string message, System.Exception e) : base(message, e)
        {
        }
    }
}