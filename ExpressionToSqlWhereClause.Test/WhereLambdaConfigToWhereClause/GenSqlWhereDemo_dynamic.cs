using System;
using System.Collections.Generic;
using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.WhereLambdaConfigToWhereClause
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

            whereLambda[SearchType.le] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };

            (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(sql, "(((Id <= @Id)) And ((DataCreatedAt <= @DataCreatedAt)))");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5 },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
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

            (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(sql, "");
            var dict = new Dictionary<string, object>();
            DictionaryAssert.AreEqual(param, dict);
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
                whereLambda[SearchType.eq] = new List<string>
                {
                    nameof(searchModel.IsDel),
                };

                (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

                Assert.AreEqual(sql, "((IsDel = @IsDel))");
                var dict = new Dictionary<string, object>
                {
                    { "@IsDel", false },
                };
                DictionaryAssert.AreEqual(param, dict);
            }
            {
                var searchModel = new
                {
                    IsDel = 1,
                };

                var whereLambda = searchModel.CrateWhereLambda((model_dynamic _) => { });
                whereLambda[SearchType.eq] = new List<string>
                {
                    nameof(searchModel.IsDel),
                };

                (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

                Assert.AreEqual(sql, "((IsDel = @IsDel))");
                var dict = new Dictionary<string, object>
                {
                    { "@IsDel", true },
                };
                DictionaryAssert.AreEqual(param, dict);
            }
        }
    }
}