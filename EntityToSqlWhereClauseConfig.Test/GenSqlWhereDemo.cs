using System;
using System.Collections.Generic;
using EntityToSqlWhereClauseConfig.Test.input;
using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    [TestClass]
    public class GenSqlWhereDemo
    {
        [TestMethod]
        public void Test_like()
        {
            var searchModel = new Input_like()
            {
                Url = "123",
            };
            var whereLambda = new WhereLambda<People, Input_like>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.like] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };


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
        public void Test_likeLeft()
        {
            var searchModel = new Input_likeLeft()
            {
                Url = "123",
            };
            var whereLambda = new WhereLambda<People, Input_likeLeft>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.likeLeft] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };


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
        public void Test_likeRight()
        {
            var searchModel = new Input_likeRight()
            {
                Url = "123",
            };
            var whereLambda = new WhereLambda<People, Input_likeRight>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.likeRight] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };

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
        public void Test_eq()
        {
            var searchModel = new Input_eq()
            {
                IsDel = true,//todo://计划:添加当其他值为xx时,当前值才生效
            };

            var whereLambda = new WhereLambda<People, Input_eq>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.eq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };

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
        public void Test_neq()
        {
            var searchModel = new Input_neq()
            {
                IsDel = true,//todo://计划:添加当其他值为xx时,当前值才生效
            };

            var whereLambda = new WhereLambda<People, Input_neq>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.neq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };

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
        public void Test_in()
        {
            var searchModel = new Input_in()
            {
                Id = "1,2",
                Sex = "1",
            };

            var whereLambda = new WhereLambda<People, Input_in>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.Sex),
            };


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
        public void Test_in2()
        {
            var searchModel = new Input_in2()
            {
                Id = 1
            };

            var whereLambda = new WhereLambda<People, Input_in2>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.Id),
            };

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
        public void Test_datetimeRange()
        {
            {
                //DataCreatedAtStart != DataCreatedAtEnd 
                var searchModel = new Input_datetimeRange_Attr()
                {
                    DataCreatedAtStart = DateTime.Parse("2021-8-7 23:00:00"),
                    DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
                };
                var whereLambda = new WhereLambda<Input_datetimeRange, Input_datetimeRange_Attr>();
                whereLambda.SearchModel = searchModel;

                whereLambda[SearchType.datetimeRange] = new List<string>
                {
                    nameof(searchModel.DataCreatedAtStart),
                    nameof(searchModel.DataCreatedAtEnd),
                    nameof(searchModel.DataUpdatedAtStart),
                    nameof(searchModel.DataUpdatedAtEnd),
                };


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

                var whereLambda = new WhereLambda<People, Input_datetimeRange2_Attr>();
                whereLambda.SearchModel = searchModel;

                whereLambda[SearchType.datetimeRange] = new List<string>
                {
                    nameof(searchModel.DataCreatedAt)
                };


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
        public void Test_numberRange()
        {
            {
                var searchModel = new Input_numberRange_Attr()
                {
                    IdLeft = 3,
                    IdRight = 9
                };
                var whereLambda = new WhereLambda<People, Input_numberRange_Attr>();
                whereLambda.SearchModel = searchModel;
                whereLambda[SearchType.numberRange] = new List<string>
                {
                    nameof(searchModel.IdLeft),
                    nameof(searchModel.IdRight),
                };

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
                var searchModel = new Input_numberRange2()
                {
                    Id = 5
                };
                var whereLambda = new WhereLambda<People, Input_numberRange2>();
                whereLambda.SearchModel = searchModel;
                whereLambda[SearchType.numberRange] = new List<string>
                {
                    nameof(searchModel.Id),
                };


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
        public void Test_gt()
        {
            var searchModel = new Input_gt()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = new WhereLambda<People, Input_gt>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.gt] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };

            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Id > @Id And DataCreatedAt > @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5 },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_ge()
        {
            var searchModel = new Input_ge()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = new WhereLambda<People, Input_ge>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.ge] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };

            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Id >= @Id And DataCreatedAt >= @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5 },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_lt()
        {
            var searchModel = new Input_lt()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = new WhereLambda<People, Input_lt>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.lt] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };


            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Id < @Id And DataCreatedAt < @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5 },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_le()
        {
            var searchModel = new Input_le()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = new WhereLambda<People, Input_le>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.le] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };

            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Id <= @Id And DataCreatedAt <= @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5 },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

    }
}