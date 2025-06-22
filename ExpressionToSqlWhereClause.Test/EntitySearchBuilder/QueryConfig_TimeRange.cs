using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_TimeRange
{
    [TestMethod]
    public void timeRange_start_neq_end()
    {
        //DataCreatedAtStart != DataCreatedAtEnd
        var searchModel = new Input_timeRange_Attr()
        {
            DataCreatedAtStart = DateTime.Parse("2021-8-7 23:00:00"),
            DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_timeRange));

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
    public void timeRange_start_eq_end()
    {
        //DataCreatedAtStart == DataCreatedAtEnd  ,会按 2021.8.8 一整天的时间号来解析

        var searchModel = new Input_timeRange_Attr()
        {
            DataCreatedAtStart = DateTime.Parse("2021-8-8"),
            DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_timeRange));

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
    public void timeRange_start_eq_end_WhereIf()
    {
        //DataCreatedAtStart == DataCreatedAtEnd  ,会按 2021.8.8 一整天的时间号来解析

        var searchModel = new Input_timeRange_Attr()
        {
            DataCreatedAtStart = DateTime.Parse("2021-8-8"),
            DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_timeRange));

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
    public void timeRange_Get时间精度()
    {
        //没有区间范围 (取当前时间精度: 如 当前 当前小时, 当前这一分钟, 当前这秒)
        var searchModel = new Input_timeRange2_Attr()
        {
            DataCreatedAt = DateTime.Parse("2021-8-8")
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));

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
}