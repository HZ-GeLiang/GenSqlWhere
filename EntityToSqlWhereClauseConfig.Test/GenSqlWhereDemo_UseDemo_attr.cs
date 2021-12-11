using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EntityToSqlWhereClauseConfig.Test.input;
using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
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

            Assert.AreEqual(sql, "Id In @Id And Sex In @Sex And IsDel = @IsDel And DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1 And Url Like @Url");
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
