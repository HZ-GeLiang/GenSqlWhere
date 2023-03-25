using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause
{
    public class model_dynamic
    {
        public int Id { get; set; }
        public bool IsDel { get; set; }
        public DateTime DataCreatedAt { get; set; }

    }


    [TestClass]
    public class GenSqlWhereDemo_dynamic
    {
        [TestMethod]
        public void Test_le_dynamic()
        {
            var searchModel = new
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = searchModel.CrateWhereLambda((model_dynamic _) => { });

            whereLambda[SearchType.Le] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };

            var searchCondition = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Id <= @Id And DataCreatedAt <= @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5 },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        [TestMethod]
        public void Test_le_dynamic_什么条件都没配()
        {
            var searchModel = new
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = searchModel.CrateWhereLambda((model_dynamic _) => { });

            var searchCondition = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "");
            var dict = new Dictionary<string, object>();
            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        [TestMethod]
        public void Test_le_dynamic_bool和int()
        {
            //int to bool  取决于 Convert.ChangeType(propertyValue, bool );

            {
                var searchModel = new
                {
                    IsDel = 0,
                };

                var whereLambda = searchModel.CrateWhereLambda((model_dynamic _) => { });
                whereLambda[SearchType.Eq] = new List<string>
                {
                    nameof(searchModel.IsDel),
                };

                var searchCondition = whereLambda.ToExpression().ToWhereClause();

                Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel");
                var dict = new Dictionary<string, object>
                {
                    { "@IsDel", false },
                };
                CollectionAssert.AreEqual(searchCondition.Parameters, dict);
            }
            {
                var searchModel = new
                {
                    IsDel = 1,
                };

                var whereLambda = searchModel.CrateWhereLambda((model_dynamic _) => { });
                whereLambda[SearchType.Eq] = new List<string>
                {
                    nameof(searchModel.IsDel),
                };

                var searchCondition = whereLambda.ToExpression().ToWhereClause();

                Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel");
                var dict = new Dictionary<string, object>
                {
                    { "@IsDel", true },
                };
                CollectionAssert.AreEqual(searchCondition.Parameters, dict);
            }
        }
    }
}