using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    public class Input_Demo_Attr
    {
        [SearchType(SearchType.@in)] public string Id { get; set; }
        [SearchType(SearchType.like)] public string Url { get; set; }
        [SearchType(SearchType.@in)] public string Sex { get; set; }
        [SearchType(SearchType.eq)] public bool IsDel { get; set; }
        [SearchType(SearchType.like)] public string Data_Remark { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataCreatedAtStart { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataCreatedAtEnd { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataUpdatedAtStart { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataUpdatedAtEnd { get; set; }
    }


    [TestClass]
    public class UseDemo_Attr
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

            var whereLambda = searchModel.CrateWhereLambda((People _) => { });

            Expression<Func<People, bool>> exp = whereLambda.ToExpression();

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
            var searchModel = new Input_like()
            {
                Url = "123",
            };
            var whereLambda = new WhereLambda<Input_like>();
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
