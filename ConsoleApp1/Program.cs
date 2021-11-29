using ExpressionToWhereClause;
using PredicateBuilder;
using System;
using System.Collections.Generic;
using ExpressionToWhereClause.Standard;
using PredicateBuilder.Standard;

namespace ConsoleApp1
{
    public partial class People
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public sbyte Sex { get; set; }
        public bool IsDel { get; set; }
        public string DataRemark { get; set; }
        public DateTime DataCreatedAt { get; set; }
        public DateTime DataUpdatedAt { get; set; }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var searchModel = new
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = searchModel.CrateWhereLambda((People _) => { });

            whereLambda[SearchType.le] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };

            (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

            var a = (sql == "Id <= @Id And DataCreatedAt <= @DataCreatedAt");

        }
    }
}
