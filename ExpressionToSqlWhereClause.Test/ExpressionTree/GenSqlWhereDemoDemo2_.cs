using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EntityToSqlWhereClauseConfig;
using ExpressionToSqlWhere.Test;
using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlWhere.ExpressionTree
{
    //demo2,  在 input 模型上 标记 Attribute 的这种配置方式来创建sql (推荐)
    //注: demo1 和demo2 如果写重复, 那么都会生效
    public class Input_Demo_Attr
    {
        [SearchType(SearchType.@in)] public string Id { get; set; }
        [SearchType(SearchType.like)] public string Url { get; set; }
        [SearchType(SearchType.@in)] public string Sex { get; set; }
        [SearchType(SearchType.eq)] public bool IsDel { get; set; }
        [SearchType(SearchType.like)] public string Data_Remark { get; set; }
        [SearchType(SearchType.timeRange)] public DateTime? DataCreatedAtStart { get; set; }
        [SearchType(SearchType.timeRange)] public DateTime? DataCreatedAtEnd { get; set; }
        [SearchType(SearchType.timeRange)] public DateTime? DataUpdatedAtStart { get; set; }
        [SearchType(SearchType.timeRange)] public DateTime? DataUpdatedAtEnd { get; set; }
    }


    [TestClass]
    public class UseDemo2
    {
        [TestMethod]
        public void Use_attr()
        {
            var time = System.DateTime.Parse("2021-8-8");
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

            var whereLambda = searchModel.CrateWhereLambda((Model_People _) => { });

            Expression<Func<Model_People, bool>> exp = whereLambda.ToExpression();

            (string sql, Dictionary<string, object> param) = exp.ToWhereClause();

            Assert.AreEqual(sql, "((((((Id In (@Id)) And (Sex In (@Sex))) And ((IsDel = @IsDel))) And ((DataCreatedAt >= @DataCreatedAt))) And ((DataCreatedAt < @DataCreatedAt1))) And (Url Like @Url))");
        }

        [TestMethod]
        public void Test_like_Sef_Attr() //TEntity ==  TSearchModel
        {
            var searchModel = new Input_like_Attr()
            {
                Url = "123",
            };
            var whereLambda = searchModel.CrateWhereLambda();
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
        public void Test_like_self()
        {
            var searchModel = new model_like()
            {
                Url = "123",
            };
            var whereLambda = new WhereLambda<model_like>();
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

    }
}
