using System;

namespace ExpressionToSqlWhereClause.Test
{
    public class Student
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public sbyte Sex { get; set; }
        public bool IsDel { get; set; }
        public string DataRemark { get; set; }
        public DateTime DataCreatedAt { get; set; }
        public DateTime DataUpdatedAt { get; set; }

    }

    public class Student_Alias
    {
        public int Id { get; set; }

    }
}
