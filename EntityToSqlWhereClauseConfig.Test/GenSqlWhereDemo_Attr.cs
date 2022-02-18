using System;
using System.Collections.Generic;
using EntityToSqlWhereClauseConfig.Test.input;
using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    [TestClass]
    public class GenSqlWhereDemo_Attr
    {
        [TestMethod]
        public void Test_like_Attr()
        {
            var searchModel = new Input_like_Attr()
            {
                Url = "123",
            };
            var whereLambda = searchModel.CrateWhereLambda((Input_like _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
            Assert.AreEqual(sql, "Url Like @Url");
            var dict = new Dictionary<string, object>
            {
                { "@Url", "%123%" }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_likeLeft_Attr()
        {
            var searchModel = new Input_likeLeft_Attr()
            {
                Url = "123",
            };
            var whereLambda = searchModel.CrateWhereLambda((Input_likeLeft _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
            Assert.AreEqual(sql, "Url Like @Url");
            var dict = new Dictionary<string, object>
            {
                { "@Url", "123%" }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_likeRight_Attr()
        {
            var searchModel = new Input_likeRight_Attr()
            {
                Url = "123",
            };
            var whereLambda = searchModel.CrateWhereLambda((Input_likeRight _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
            Assert.AreEqual(sql, "Url Like @Url");
            var dict = new Dictionary<string, object>
            {
                { "@Url", "%123" }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_eq_Attr()
        {
            var searchModel = new Input_eq_Attr()
            {
                IsDel = true,
            };

            var whereLambda = searchModel.CrateWhereLambda((Input_eq _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "IsDel = @IsDel");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", searchModel.IsDel }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_neq_Attr()
        {
            var searchModel = new Input_neq_Attr()
            {
                IsDel = true,//todo://计划:添加当其他值为xx时,当前值才生效
            };
         
            var whereLambda = searchModel.CrateWhereLambda((Input_neq _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
            Assert.AreEqual(sql, "IsDel <> @IsDel");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", searchModel.IsDel }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_in_Attr()
        {
            var searchModel = new Input_in_Attr()
            {
                Id = "1,2",
                Sex = "1",
            };
 
            var whereLambda = searchModel.CrateWhereLambda((Input_in _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
            Assert.AreEqual(sql, "((Id In (@Id)) And (Sex In (@Sex)))");
            var dict = new Dictionary<string, object>
            {
                { "@Id", searchModel.Id},
                { "@Sex", searchModel.Sex}
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_in2_Attr()
        {
            var searchModel = new Input_in2_Attr()
            {
                Id = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Input_in2 _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
            Assert.AreEqual(sql, "(Id In (@Id))");
            var dict = new Dictionary<string, object>
            {
                { "@Id", "1"},//in 可以有多个值,所以这个值就是stirng类型的
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_datetimeRange_Attr()
        {
            {
                var searchModel = new Input_datetimeRange_Attr()
                {
                    DataCreatedAtStart = DateTime.Parse("2021-8-7 23:00:00"),
                    DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
                };
                var whereLambda = searchModel.CrateWhereLambda((Input_datetimeRange _) => { });
                var expression = whereLambda.ToExpression();
                (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
                Assert.AreEqual(sql, "DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1");
                var dict = new Dictionary<string, object>
                {
                    { "@DataCreatedAt", searchModel.DataCreatedAtStart},
                    { "@DataCreatedAt1", DateTime.Parse("2021-8-9")},
                };

                DictionaryAssert.AreEqual(param, dict);
            }

            {
                //没有区间范围 (取当前时间精度: 如 当前 当前小时, 当前这一分钟, 当前这秒)
                var searchModel = new Input_datetimeRange2_Attr()
                {
                    DataCreatedAt = DateTime.Parse("2021-8-8")
                };

                var whereLambda = searchModel.CrateWhereLambda((Input_datetimeRange2_Attr _) => { });
                var expression = whereLambda.ToExpression();
                (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
                Assert.AreEqual(sql, "DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1");

                var dict = new Dictionary<string, object>
                {
                    { "@DataCreatedAt", DateTime.Parse("2021-8-8") },
                    { "@DataCreatedAt1", DateTime.Parse("2021-8-9") }
                };

                DictionaryAssert.AreEqual(param, dict);
            }
        }

        [TestMethod]
        public void Test_numberRange_Attr()
        {
            {
                var searchModel = new Input_numberRange_Attr()
                {
                    IdLeft = 3,
                    IdRight = 9
                };
                var whereLambda = searchModel.CrateWhereLambda((Input_numberRange _) => { });
                var expression = whereLambda.ToExpression();
                (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

                Assert.AreEqual(sql, "Id >= @Id And Id <= @Id1");
                var dict = new Dictionary<string, object>
                {
                    { "@Id",searchModel.IdLeft},
                    { "@Id1",searchModel.IdRight},
                };

                DictionaryAssert.AreEqual(param, dict);
            }

            {
                var searchModel = new Input_numberRange2_Attr()
                {
                    Id = 5
                };
                var whereLambda = searchModel.CrateWhereLambda((Input_numberRange2 _) => { });
                var expression = whereLambda.ToExpression();
                (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
                Assert.AreEqual(sql, "Id >= @Id And Id <= @Id1");
                var dict = new Dictionary<string, object>
                {
                    { "@Id",searchModel.Id},
                    { "@Id1",searchModel.Id},
                };

                DictionaryAssert.AreEqual(param, dict);
            }
        }

        [TestMethod]
        public void Test_gt_Attr()
        {
            var searchModel = new Input_gt_Attr()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = searchModel.CrateWhereLambda((Input_gt _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
            Assert.AreEqual(sql, "Id > @Id And DataCreatedAt > @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5l },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_ge_Attr()
        {
            var searchModel = new Input_ge_Attr()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };
            var whereLambda = searchModel.CrateWhereLambda((Input_ge _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
            Assert.AreEqual(sql, "Id >= @Id And DataCreatedAt >= @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5L },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_lt_Attr()
        {
            var searchModel = new Input_lt_Attr()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };
            var whereLambda = searchModel.CrateWhereLambda((Input_lt _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
            Assert.AreEqual(sql, "Id < @Id And DataCreatedAt < @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5l },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_le_Attr()
        {
            var searchModel = new Input_le_Attr()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };
            var whereLambda = searchModel.CrateWhereLambda((Input_le _) => { });
            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Id <= @Id And DataCreatedAt <= @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5l },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

    }
}