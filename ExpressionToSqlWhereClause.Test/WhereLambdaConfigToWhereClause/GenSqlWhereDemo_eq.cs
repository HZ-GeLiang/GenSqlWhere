using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.Test.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.WhereLambdaConfigToWhereClause
{
    [TestClass]
    public class GenSqlWhereDemo_eq
    {
        [TestMethod]
        public void Test_eq()
        {
            var searchModel = new Model_eq()
            {
                IsDel = true,//todo://计划:添加当其他值为xx时,当前值才生效
            };

            var whereLambda = new WhereLambda<Input_eq, Model_eq>();
            whereLambda.Search = searchModel;

            whereLambda[SearchType.Eq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", searchModel.IsDel }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        [TestMethod]
        public void Test_eq2()
        {
            var searchModel = new Model_eq2()
            {
                Id = 1
            };

            var expression = default(Expression<Func<Input_eq2, bool>>)
             .WhereIf(true, a => a.Id == searchModel.Id)
             ;
            var searchCondition = expression.ToWhereClause();

        }

    }

    public class Input_eq
    {
        [SearchType(SearchType.Eq)] public bool IsDel { get; set; }
    }

    public class Input_eq2
    {
        [SearchType(SearchType.Eq)] public long? Id { get; set; }
    }


    public class Model_eq
    {
        public bool IsDel { get; set; }
    }

    public class Model_eq2
    {
        public int Id { get; set; }
    }
}