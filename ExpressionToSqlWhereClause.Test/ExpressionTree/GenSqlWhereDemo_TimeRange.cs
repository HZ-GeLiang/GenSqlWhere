using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionToSqlWhere.Test;
using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlWhere.ExtensionMethod;

namespace SqlWhere.ExpressionTree
{

    public class model_timeRange
    {
        public DateTime? DataCreatedAt { get; set; }
        public DateTime? DataUpdatedAt { get; set; }
    }

    public class Input_timeRange_Attr
    {
        [SearchType(SearchType.timeRange)] public DateTime? DataCreatedAtStart { get; set; }
        [SearchType(SearchType.timeRange)] public DateTime? DataCreatedAtEnd { get; set; }
        [SearchType(SearchType.timeRange)] public DateTime? DataUpdatedAtStart { get; set; }
        [SearchType(SearchType.timeRange)] public DateTime? DataUpdatedAtEnd { get; set; }
    }

    public class Input_timeRange2_Attr
    {
        [SearchType(SearchType.timeRange)] public DateTime? DataCreatedAt { get; set; }
    }

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
            var whereLambda = new WhereLambda<model_timeRange, Input_timeRange_Attr>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.timeRange] = new List<string>
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

        [TestMethod]
        public void Test_timeRange_Get时间精度()
        {
            //没有区间范围 (取当前时间精度: 如 当前 当前小时, 当前这一分钟, 当前这秒)
            var searchModel = new Input_timeRange2_Attr()
            {
                DataCreatedAt = DateTime.Parse("2021-8-8")
            };

            var whereLambda = new WhereLambda<Model_People, Input_timeRange2_Attr>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.timeRange] = new List<string>
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

        [TestMethod]
        public void Test_timeRange_MergeParametersIntoSql()
        {
            {
                DateTime? d1 = ((DateTime?)DateTime.Parse("2022-3-1 15:12:34"));
                DateTime? d2 = ((DateTime?)DateTime.Parse("2022-3-3 12:34:56"));
                var expression = default(Expression<Func<Input_timeRange2_Attr, bool>>)
                  .WhereIf(true, a => a.DataCreatedAt >= d1)
                  .WhereIf(true, a => a.DataCreatedAt < d2)
                  ;

                (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

                var formatDateTime = new Dictionary<string, string>() { { "@DataCreatedAt1", "yyyy-MM-dd" } };
                sql = WhereClauseHelper.MergeParametersIntoSql(sql, param, formatDateTime);
                Assert.AreEqual(sql, "DataCreatedAt >= '2022-3-1 15:12:34' And DataCreatedAt < '2022-03-03'");

            }

            {
                DateTime? d1 = ((DateTime?)DateTime.Parse("2022-3-1 15:12:34"));
                DateTime? d2 = ((DateTime?)DateTime.Parse("2022-3-3 12:34:56"));
                var expression = default(Expression<Func<Input_timeRange2_Attr, bool>>)
                  .WhereIf(true, a => a.DataCreatedAt >= d1)
                  .WhereIf(true, a => a.DataCreatedAt < d2)
                  ;

                (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

                var formatDateTime = WhereClauseHelper.GetDefaultFormatDateTime(param);
                sql = WhereClauseHelper.MergeParametersIntoSql(sql, param, formatDateTime);
                Assert.AreEqual(sql, "DataCreatedAt >= '2022-3-1 15:12:34' And DataCreatedAt < '2022-3-3 12:34:56'");
            }
        }
    }
}