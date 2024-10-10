using ExpressionToSqlWhereClause.EntityConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause;

//demo2,  在 input 模型上 标记 Attribute 的这种配置方式来创建sql (推荐)
//注: demo1 和demo2 如果写重复, 那么都会生效
public class Input_Demo_Attr
{
    [SearchType(SearchType.In)] public string Id { get; set; }
    [SearchType(SearchType.Like)] public string Url { get; set; }
    [SearchType(SearchType.In)] public string Sex { get; set; }
    [SearchType(SearchType.Eq)] public bool IsDel { get; set; }
    [SearchType(SearchType.Like)] public string Data_Remark { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAtStart { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAtEnd { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataUpdatedAtStart { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataUpdatedAtEnd { get; set; }
}

[TestClass]
public class UseDemo2
{
    [TestMethod]
    public void Use_attr()
    {
        var time = DateTime.Parse("2021-8-8");
        var searchModel = new Input_Demo_Attr()
        {
            //Id = 1,
            Id = "1,2",
            Sex = "1",
            IsDel = true,
            Url = "123",
            DataCreatedAtStart = time.AddHours(-1),
            DataCreatedAtEnd = time,
        };

        var whereLambda = searchModel.CreateWhereLambda((Model_People _) => { });

        Expression<Func<Model_People, bool>> exp = whereLambda.ToExpression();

        var searchCondition = exp.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id) And Sex In (@Sex) And IsDel = @IsDel And DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1 And Url Like @Url");
    }

    [TestMethod]
    public void Test_like_Sef_Attr() //TEntity ==  TSearch
    {
        var searchModel = new Input_like_Attr()
        {
            Url = "123",
        };
        var whereLambda = searchModel.CreateWhereLambda();
        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "Url Like @Url");
        var dict = new Dictionary<string, object>
        {
            { "@Url", "%123%" }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void Test_like_self()
    {
        var searchModel = new model_like()
        {
            Url = "123",
        };
        var whereLambda = new WhereLambda<model_like, model_like>(searchModel);

        whereLambda[SearchType.Like] = new List<string>
        {
            nameof(searchModel.Url),
            nameof(searchModel.Data_Remark),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Url Like @Url");
        var dict = new Dictionary<string, object>
        {
            { "@Url", "%123%" }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }
}