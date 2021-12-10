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

       
    }
}
