using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.Exceptions;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.SqlFunc.EntityConfig;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Infra.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_sqlFunc_Month
{
    [TestMethod]
    public void Month_eq()
    {
        var searchModel = new Input_sqlFun_Month_eq() //这里不能是匿名类型
        {
            DataCreatedAt = 1
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_sqlFun_Month));

        var exp = whereLambda.ToExpression();

        var searchCondition = exp.ToWhereClause();

        Dictionary<string, object> expectedParameters = new()
        {
            { "@Month", 1 }//Month()返回的是int ,所以1 是int类型的才对
        };
        Assert.AreEqual(searchCondition.WhereClause, "Month(DataCreatedAt) = @Month");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void Month_Neq()
    {
        var searchModel = new Input_sqlFun_Month_neq
        {
            DataCreatedAt = 1
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_sqlFun_Month));

        var exp = whereLambda.ToExpression();

        var searchCondition = exp.ToWhereClause();

        Dictionary<string, object> expectedParameters = new()
        {
            { "@Month", 1 }//Month()返回的是int ,所以1 是int类型的才对
        };
        Assert.AreEqual(searchCondition.WhereClause, "Month(DataCreatedAt) <> @Month");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void Month_In()
    {
        var searchModel = new Input_sqlFun_Month_in
        {
            DataCreatedAt = 1
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_sqlFun_Month));

        //SearchType:in,操作遇到不支持的属性类型:System.DateTime
        var exMsg = $"SearchType:{nameof(SearchType.In)},操作遇到不支持的属性类型:{typeof(DateTime).FullName}";

        Assert.ThrowsException<FrameException>(() => whereLambda.ToExpression().ToWhereClause(), exMsg);
    }

    [TestMethod]
    public void MonthIn_eq_1()
    {
        var searchModel = new Input_sqlFun_MonthIn1()
        {
            DataCreatedAt = 1
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_sqlFun_Month));
        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.DataCreatedAt),
        };
        var exp = whereLambda.ToExpression();

        var searchCondition = exp.ToWhereClause();

        Dictionary<string, object> expectedParameters = new()
        {
            { "@MonthIn", "1" } //sql 的 in(变量), 这个变量只能写一个, 所以,这里是string
        };
        Assert.AreEqual(searchCondition.WhereClause, "Month(DataCreatedAt) In (@MonthIn)");

        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void MonthIn_eq_2()
    {
        var searchModel = new Input_sqlFun_MonthIn2()
        {
            DataCreatedAt = "1,2,3"
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_sqlFun_Month));
        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.DataCreatedAt),
        };
        var exp = whereLambda.ToExpression();

        var searchCondition = exp.ToWhereClause();

        Dictionary<string, object> expectedParameters = new()
        {
            { "@MonthIn", "1,2,3" } //sql 的 in(变量), 这个变量只能写一个, 所以,这里是string
        };
        Assert.AreEqual(searchCondition.WhereClause, "Month(DataCreatedAt) In (@MonthIn)");

        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void MonthIn_eq_3()
    {
        var searchModel = new Input_sqlFun_MonthIn3()
        {
            DataCreatedAt = new List<int> { 1, 2, 3 }
        };
        var whereLambda = searchModel.CreateQueryConfig(default(Model_sqlFun_Month));
        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.DataCreatedAt),
        };
        var exp = whereLambda.ToExpression();

        var searchCondition = exp.ToWhereClause();

        Dictionary<string, object> expectedParameters = new()
        {
            { "@MonthIn", "1,2,3" } //sql 的 in(变量), 这个变量只能写一个, 所以,这里是string
        };
        Assert.AreEqual(searchCondition.WhereClause, "Month(DataCreatedAt) In (@MonthIn)");

        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    #region 待实现

    //todo: 提供demo

    public class Input_sqlFun_MonthIn1
    {
        [MonthIn]
        public int DataCreatedAt { get; set; } //最后要翻译为 List<int>
    }

    public class Input_sqlFun_MonthIn2
    {
        [MonthIn]
        public string DataCreatedAt { get; set; } //最后要翻译为 List<int>
    }

    public class Input_sqlFun_MonthIn3
    {
        [MonthIn]
        public List<int> DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }

    #endregion
}