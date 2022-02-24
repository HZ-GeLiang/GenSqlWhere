﻿using System;
using System.Collections.Generic;
using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.EntityConfig;

namespace NugetPackageTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestProgram.Test();
            Console.WriteLine("Hello World!");
        }
    }

    public class People
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public sbyte Sex { get; set; }
        public bool IsDel { get; set; }
        public string DataRemark { get; set; }
        public DateTime DataCreatedAt { get; set; }
        public DateTime DataUpdatedAt { get; set; }

    }

    public class TestProgram
    {
        public static void Test()
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

            Console.WriteLine(sql == "Id <= @Id And DataCreatedAt <= @DataCreatedAt");
        }
    }
}
