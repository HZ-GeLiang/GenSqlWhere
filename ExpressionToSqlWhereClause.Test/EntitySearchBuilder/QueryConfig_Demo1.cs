using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Infra.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_Demo1
{
    [TestMethod]
    public void Use1()
    {
        var time = DateTime.Parse("2021-8-8");
        var searchModel = new Input_Demo()
        {
            //Id = 1,
            Id = "1,2",
            Sex = "1",
            IsDel = true,
            Url = "123",
            DataCreatedAtStart = time.AddDays(-1), //Todo:如果这里是 Addhour,默认的时间进度将计算错误
            DataCreatedAtEnd = time,
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));
        #region whereLambda 的配置

        whereLambda[SearchType.Like] = new List<string>
        {
            nameof(searchModel.Url),
            nameof(searchModel.Data_Remark),
        };

        whereLambda[SearchType.Eq] = new List<string>
        {
            nameof(searchModel.IsDel),
        };
        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.Id),
            nameof(searchModel.Sex),
        };

        whereLambda[SearchType.TimeRange] = new List<string>
        {
            nameof(searchModel.DataCreatedAtStart),
            nameof(searchModel.DataCreatedAtEnd),
            nameof(searchModel.DataUpdatedAtStart),
            nameof(searchModel.DataUpdatedAtEnd),
        };

        whereLambda[SearchType.NumberRange] = new List<string>
        {
        };

        #endregion

        //List<Expression<Func<Route, bool>>> listExp = whereLambda.ToExpressionList(); //可以给ef用
        Expression<Func<Model_People, bool>> exp = whereLambda.ToExpression();
        //等价
        //List<Expression<Func<Route, bool>>> listExp = whereLambda;
        //Expression<Func<Route, bool>> exp = whereLambda;

        var searchCondition = exp.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel And Id In (@Id1 , @Id2 ) And Sex In (@Sex1 ) And DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1 And Url Like @Url");

        var order_Parameters = searchCondition.Parameters.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);

        var dict = new Dictionary<string, object>
        {
            { "@DataCreatedAt", searchModel.DataCreatedAtStart },
            { "@DataCreatedAt1", searchModel.DataCreatedAtEnd?.AddDays(1) },
            { "@Id1", 1}, //Model_People.Id 是int 类型
            { "@Id2", 2}, //Model_People.Id 是int 类型
            { "@IsDel", searchModel.IsDel },
            { "@Sex1", (sbyte)1},
            { "@Url", $@"%{searchModel.Url}%" },
        };

        DictionaryAssert.AreEqual(order_Parameters, dict);
    }

    [TestMethod]
    public void Use2()
    {
        var searchModel = new Input_Demo()
        {
            IsDel = true,
            Url = "123",
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));

        whereLambda[SearchType.Eq] = new List<string>
        {
            nameof(searchModel.IsDel),
            nameof(searchModel.Url),
        };

        Expression<Func<Model_People, bool>> exp = whereLambda.ToExpression();

        {
            var searchCondition = exp.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel And Url = @Url");

            var clause = WhereClauseHelper.GetNumberParameterClause(searchCondition.WhereClause);
            Assert.AreEqual(clause, "IsDel = @0 And Url = @1");
        }
    }

    [TestMethod]
    public void UseDynamic()
    {
        //针对  People  的匿名类
        var searchModel = new /*People*/
        {
            //Id = 1,
            Id = "1,2",
            Sex = "1",
            IsDel = true,
            Url = "123",
            DataCreatedAtStart = DateTime.Parse("2021-8-8").AddHours(-1),
            DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People)); //这个 People 类型 和上面的匿名类型有关联的

        whereLambda[SearchType.Like] = new List<string>
        {
            nameof(searchModel.Url),
        };

        whereLambda[SearchType.Eq] = new List<string>
        {
            nameof(searchModel.IsDel),
        };
        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.Id),
            nameof(searchModel.Sex),
        };

        whereLambda[SearchType.TimeRange] = new List<string>
        {
            nameof(searchModel.DataCreatedAtStart),
            nameof(searchModel.DataCreatedAtEnd),
        };

        var searchCondition = whereLambda.ToExpression().ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel And Id In (@Id1 , @Id2 ) And Sex In (@Sex1 ) And DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1 And Url Like @Url");
    }

    [TestMethod]
    public void UseDynamic_Test2()
    {
        //针对  People  的匿名类
        var searchModel = new /*People*/
        {
            //IsDel = 0,
            Name = (string)null,
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));//这个 People 类型 和上面的匿名类型有关联的

        whereLambda[SearchType.Like] = new List<string>
        {
            "balabala"
        };
        Assert.ThrowsException<ArgumentException>(() => whereLambda.ToExpression().ToWhereClause(), "类'<>f__AnonymousType3`1[[System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]'中不存在名为'balabala'的属性");
    }

    [TestMethod]
    public void UseExampleForEf()
    {
        var time = DateTime.Parse("2021-8-8");
        var searchModel = new Input_Demo()
        {
            //Id = 1,
            Id = "1,2",
            Sex = "1",
            IsDel = true,
            Url = "123",
            DataCreatedAtStart = time.AddHours(-1),
            DataCreatedAtEnd = time,
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));

        whereLambda[SearchType.Like] = new List<string>
        {
            nameof(searchModel.Url),
            nameof(searchModel.Data_Remark),
        };

        whereLambda[SearchType.Eq] = new List<string>
        {
            nameof(searchModel.IsDel),
        };
        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.Id),
            nameof(searchModel.Sex),
        };

        whereLambda[SearchType.TimeRange] = new List<string>
        {
            nameof(searchModel.DataCreatedAtStart),
            nameof(searchModel.DataCreatedAtEnd),
            nameof(searchModel.DataUpdatedAtStart),
            nameof(searchModel.DataUpdatedAtEnd),
        };

        whereLambda[SearchType.NumberRange] = new List<string>
        {
        };

        var listExp = whereLambda.ToExpressionListForEF(); //可以给ef用
    }
}