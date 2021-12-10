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
                DataCreatedAt = DateTime.Now
            };

            var whereLambda = new WhereLambda<People, Input_sqlFun_Month>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.eq] = new List<string>
            {
                nameof(searchModel.DataCreatedAt),
            };

            (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

        }
    }
}