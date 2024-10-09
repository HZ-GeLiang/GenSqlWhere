using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.Helper;
using ExpressionToSqlWhereClause.Test.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause
{
    [TestClass]
    public class GenSqlWhereDemo_TimeRange
    {
        [TestMethod]
        public void Test_timeRange_start_neq_end()
        {
            //DataCreatedAtStart != DataCreatedAtEnd
            var searchModel = new Input_timeRange_Attr()
            {
                DataCreatedAtStart = DateTime.Parse("2021-8-7 23:00:00"),
                DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
            };
            var whereLambda = new WhereLambda<model_timeRange, Input_timeRange_Attr>(searchModel);

            whereLambda[SearchType.TimeRange] = new List<string>
            {
                nameof(searchModel.DataCreatedAtStart),
                nameof(searchModel.DataCreatedAtEnd),
                nameof(searchModel.DataUpdatedAtStart),
                nameof(searchModel.DataUpdatedAtEnd),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1");
            Dictionary<string, object> dict = new()
            {
                { "@DataCreatedAt", searchModel.DataCreatedAtStart},
                { "@DataCreatedAt1", DateTime.Parse("2021-8-9")},
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        [TestMethod]
        public void Test_timeRange_start_eq_end()
        {
            //DataCreatedAtStart == DataCreatedAtEnd  ,会按 2021.8.8 一整天的时间号来解析

            var searchModel = new Input_timeRange_Attr()
            {
                DataCreatedAtStart = DateTime.Parse("2021-8-8"),
                DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
            };
            var whereLambda = new WhereLambda<model_timeRange, Input_timeRange_Attr>(searchModel);

            whereLambda[SearchType.TimeRange] = new List<string>
            {
                nameof(searchModel.DataCreatedAtStart),
                nameof(searchModel.DataCreatedAtEnd),
                nameof(searchModel.DataUpdatedAtStart),
                nameof(searchModel.DataUpdatedAtEnd),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1");
            Dictionary<string, object> dict = new()
            {
                { "@DataCreatedAt", searchModel.DataCreatedAtStart},
                { "@DataCreatedAt1", DateTime.Parse("2021-8-9")},
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        [TestMethod]
        public void Test_timeRange_start_eq_end_WhereIf()
        {
            //DataCreatedAtStart == DataCreatedAtEnd  ,会按 2021.8.8 一整天的时间号来解析

            var searchModel = new Input_timeRange_Attr()
            {
                DataCreatedAtStart = DateTime.Parse("2021-8-8"),
                DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
            };
            var whereLambda = new WhereLambda<model_timeRange, Input_timeRange_Attr>(searchModel);

            whereLambda[SearchType.TimeRange] = new List<string>
            {
                nameof(searchModel.DataCreatedAtStart),
                nameof(searchModel.DataCreatedAtEnd),
                nameof(searchModel.DataUpdatedAtStart),
                nameof(searchModel.DataUpdatedAtEnd),
            };

            whereLambda.WhereIf[nameof(searchModel.DataCreatedAtStart)] =
                    a => a.DataCreatedAtStart > new DateTime(2022, 8, 9);//

            //  因为上面的条件没生效, 所以翻译引擎会当  只有 DataCreatedAtEnd 有值来处理.
            //所以, 这里要把  DataCreatedAtEnd 当没有值来处理
            whereLambda.WhereIf[nameof(searchModel.DataCreatedAtEnd)] =
                    a => a.DataCreatedAtStart > new DateTime(2022, 8, 9);

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "");
            Dictionary<string, object> dict = new();

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        [TestMethod]
        public void Test_timeRange_Get时间精度()
        {
            //没有区间范围 (取当前时间精度: 如 当前 当前小时, 当前这一分钟, 当前这秒)
            var searchModel = new Input_timeRange2_Attr()
            {
                DataCreatedAt = DateTime.Parse("2021-8-8")
            };

            var whereLambda = new WhereLambda<Model_People, Input_timeRange2_Attr>(searchModel);

            whereLambda[SearchType.TimeRange] = new List<string>
            {
                nameof(searchModel.DataCreatedAt)
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1");

            var dict = new Dictionary<string, object>
            {
                { "@DataCreatedAt", DateTime.Parse("2021-8-8") },
                { "@DataCreatedAt1", DateTime.Parse("2021-8-9") }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        [TestMethod]
        public void Test_timeRange_MergeParametersIntoSql()
        {
            {
                DateTime? d1 = (DateTime?)DateTime.Parse("2022-3-1 15:12:34");
                DateTime? d2 = (DateTime?)DateTime.Parse("2022-3-3 12:34:56");
                var expression = default(Expression<Func<Input_timeRange2_Attr, bool>>)
                  .WhereIf(true, a => a.DataCreatedAt >= d1)
                  .WhereIf(true, a => a.DataCreatedAt < d2);

                var searchCondition = expression.ToWhereClause();

                var formatDateTime = new Dictionary<string, string>() { { "@DataCreatedAt1", "yyyy-MM-dd" } };
                var caluse = WhereClauseHelper.GetNonParameterClause(searchCondition.WhereClause, searchCondition.Parameters, formatDateTime);
                Assert.AreEqual(caluse, "DataCreatedAt >= '2022-03-01 15:12:34' And DataCreatedAt < '2022-03-03'");
            }

            {
                DateTime? d1 = (DateTime?)DateTime.Parse("2022-3-1 15:12:34");
                DateTime? d2 = (DateTime?)DateTime.Parse("2022-3-3 12:34:56");
                var expression = default(Expression<Func<Input_timeRange2_Attr, bool>>)
                  .WhereIf(true, a => a.DataCreatedAt >= d1)
                  .WhereIf(true, a => a.DataCreatedAt < d2);

                var searchCondition = expression.ToWhereClause();

                var formatDateTime = WhereClauseHelper.GetDefaultFormatDateTime(searchCondition.Parameters);
                var caluse = WhereClauseHelper.GetNonParameterClause(searchCondition, formatDateTime);
                Assert.AreEqual(caluse, "DataCreatedAt >= '2022-03-01 15:12:34' And DataCreatedAt < '2022-03-03 12:34:56'");
            }
        }
    }

    public class model_timeRange
    {
        public DateTime? DataCreatedAt { get; set; }
        public DateTime? DataUpdatedAt { get; set; }
    }

    public class Input_timeRange_Attr
    {
        [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAtStart { get; set; }
        [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAtEnd { get; set; }
        [SearchType(SearchType.TimeRange)] public DateTime? DataUpdatedAtStart { get; set; }
        [SearchType(SearchType.TimeRange)] public DateTime? DataUpdatedAtEnd { get; set; }
    }

    public class Input_timeRange2_Attr
    {
        [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAt { get; set; }
    }
}