using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Modlesl;
using Infra.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_Demo3_错误写法
{
    [TestMethod]
    public void 重复配置使用方法_不同配置()
    {
        //todo: 重复配置的检测
        var searchModel = new Input_Demo3_不同配置
        {
            Id = 1
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_Demo3));
        // 已经在 input 模型上标注了
        whereLambda[SearchType.Neq] = new List<string>
        {
            nameof(searchModel.Id),
        };

        var exp = whereLambda.ToExpression();
        var searchCondition = exp.ToWhereClause();

        Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
        expectedParameters.Add("@Id", 1);//Month()返回的是int ,所以1 是int类型的才对
        expectedParameters.Add("@Id1", 1);//Month()返回的是int ,所以1 是int类型的才对
        Assert.AreEqual(searchCondition.WhereClause, "Id = @Id And Id <> @Id1");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void 重复配置使用方法_相同配置()
    {
        var searchModel = new Input_Demo3_相同配置
        {
            Id = 1
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_Demo3));
        // 已经在 input 模型上标注了
        whereLambda[SearchType.Eq] = new List<string>
        {
            nameof(searchModel.Id),
        };

        var exp = whereLambda.ToExpression();
        var searchCondition = exp.ToWhereClause();

        Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
        expectedParameters.Add("@Id", 1);
        Assert.AreEqual(searchCondition.WhereClause, "Id = @Id");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }
}