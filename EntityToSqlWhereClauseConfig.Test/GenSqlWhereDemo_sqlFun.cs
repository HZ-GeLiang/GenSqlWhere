using System;
using System.Collections.Generic;
using EntityToSqlWhereClauseConfig.Test.input;
using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    [TestClass]
    public class GenSqlWhereDemo_sqlFun
    {

        [TestMethod]
        public void Test_sqlFun_Month()
        {
            var searchModel = new Input_sqlFun_Month()
            {
                DataCreatedAt = 1
            }; 
            var whereLambda = searchModel.CrateWhereLambda((People p) => { });
            whereLambda[SearchType.eq] = new List<string>
            {
                nameof(searchModel.DataCreatedAt),
            }; 
            var exp = whereLambda.ToExpression();

            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 1);
            Assert.AreEqual("Month(DataCreatedAt) = @Month", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }
    }
}