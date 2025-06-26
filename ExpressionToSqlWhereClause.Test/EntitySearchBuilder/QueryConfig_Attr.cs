using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Modlesl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_Attr
{
    [TestMethod]
    public void like_Attr()
    {
        var searchModel = new Input_like_Attr()
        {
            Url = "123",
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_like));

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
    public void likeLeft_Attr()
    {
        var searchModel = new Input_likeLeft_Attr()
        {
            Url = "123",
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Input_likeLeft));
        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "Url Like @Url");
        var dict = new Dictionary<string, object>
        {
            { "@Url", "123%" }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void likeRight_Attr()
    {
        var searchModel = new Input_likeRight_Attr()
        {
            Url = "123",
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Input_likeRight));
        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "Url Like @Url");
        var dict = new Dictionary<string, object>
        {
            { "@Url", "%123" }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void eq_Attr()
    {
        var searchModel = new Input_eq()
        {
            IsDel = true,
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_eq));
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
    public void neq_Attr()
    {
        var searchModel = new Input_neq_Attr()
        {
            IsDel = true,//todo://计划:添加当其他值为xx时,当前值才生效
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_neq));
        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "IsDel <> @IsDel");
        var dict = new Dictionary<string, object>
        {
            { "@IsDel", searchModel.IsDel }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void in_Attr_string_type()
    {
        var searchModel = new Input_in_Attr()
        {
            Id = "1,2",
            Sex = "1",
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_in));
        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id1 , @Id2 ) And Sex In (@Sex1 )");
        var dict = new Dictionary<string, object>
        {
            { "@Id1", "1"}, //因为 typeof(Model_in.Id) 是stirng 类型的
            { "@Id2", "2"}, //因为 typeof(Model_in.Id) 是stirng 类型的
            { "@Sex1", "1"} //因为 typeof(Model_in.Sex) 是stirng 类型的
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void in2_Attr_nullable_type()
    {
        var searchModel = new Input_in2_Attr()
        {
            Id = 1
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_in2));
        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id1 )");
        var dict = new Dictionary<string, object>
        {
            //{ "@Id", "1"},//in 可以有多个值,所以这个值就是stirng类型的   //  这个变量被移除了, 因为 WhereClause 没有这个变量了
            { "@Id1", (int) 1 }, //因为实体模型是int? 如果nullable 有值,  那么类型肯定是 int
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        Assert.AreEqual(searchCondition.Parameters["@Id1"].GetType(), typeof(int));
    }

    [TestMethod]
    public void datetimeRange_Attr()
    {
        {
            var searchModel = new Input_timeRange_Attr()
            {
                DataCreatedAtStart = DateTime.Parse("2021-8-7 23:00:00"),
                DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
            };
            var whereLambda = searchModel.CreateQueryConfig(default(Model_timeRange));
            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1");
            var dict = new Dictionary<string, object>
            {
                { "@DataCreatedAt", searchModel.DataCreatedAtStart},
                { "@DataCreatedAt1", DateTime.Parse("2021-8-9")},
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        {
            //没有区间范围 (取当前时间精度: 如 当前 当前小时, 当前这一分钟, 当前这秒)
            var searchModel = new Input_timeRange2_Attr()
            {
                DataCreatedAt = DateTime.Parse("2021-8-8")
            };

            var whereLambda = searchModel.CreateQueryConfig(default(Input_timeRange2_Attr));
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

    [TestMethod]
    public void numberRange_Attr()
    {
        {
            var searchModel = new Input_numberRange_Attr()
            {
                IdLeft = 3,
                IdRight = 9
            };
            var whereLambda = searchModel.CreateQueryConfig(default(Model_numberRange));
            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Id >= @Id And Id <= @Id1");
            var dict = new Dictionary<string, object>
            {
                { "@Id",searchModel.IdLeft},
                { "@Id1",searchModel.IdRight},
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        {
            var searchModel = new Input_numberRange2_Attr()
            {
                Id = 5
            };
            var whereLambda = searchModel.CreateQueryConfig(default(Input_numberRange2));
            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "Id >= @Id And Id <= @Id1");
            var dict = new Dictionary<string, object>
            {
                { "@Id",searchModel.Id},
                { "@Id1",searchModel.Id},
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }

    [TestMethod]
    public void gt_Attr()
    {
        var searchModel = new Input_gt_Attr()
        {
            Id = 5,
            DataCreatedAt = DateTime.Parse("2021-8-8"),
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_gt));
        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "Id > @Id And DataCreatedAt > @DataCreatedAt");
        var dict = new Dictionary<string, object>
        {
            { "@Id", 5l },//取 domain 的类型
            { "@DataCreatedAt", searchModel.DataCreatedAt }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void ge_Attr()
    {
        var searchModel = new Input_ge_Attr()
        {
            Id = 5,
            DataCreatedAt = DateTime.Parse("2021-8-8"),
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_ge));
        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "Id >= @Id And DataCreatedAt >= @DataCreatedAt");
        var dict = new Dictionary<string, object>
        {
            { "@Id", 5L },//取 domain 的类型
            { "@DataCreatedAt", searchModel.DataCreatedAt }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void lt_Attr()
    {
        var searchModel = new Input_lt_Attr()
        {
            Id = 5,
            DataCreatedAt = DateTime.Parse("2021-8-8"),
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_lt));
        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "Id < @Id And DataCreatedAt < @DataCreatedAt");
        var dict = new Dictionary<string, object>
        {
            { "@Id", 5l },//取 domain 的类型
            { "@DataCreatedAt", searchModel.DataCreatedAt }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void le_Attr()
    {
        var searchModel = new Input_le_Attr()
        {
            Id = 5,
            DataCreatedAt = DateTime.Parse("2021-8-8"),
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_le));
        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id <= @Id And DataCreatedAt <= @DataCreatedAt");
        var dict = new Dictionary<string, object>
        {
            { "@Id", 5l },//取 domain 的类型
            { "@DataCreatedAt", searchModel.DataCreatedAt }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }
}