using System;
using System.Collections.Generic;

using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    public class model_lt
    {
        public long? Id { get; set; }
        public DateTime? DataCreatedAt { get; set; }
    }

    public class Input_lt_Attr
    {
        [SearchType(SearchType.lt)] public long? Id { get; set; }
        [SearchType(SearchType.lt)] public DateTime? DataCreatedAt { get; set; }
    }

    [TestClass]
    public class GenSqlWhereDemo_lt
    {
        [TestMethod]
        public void Test_lt()
        {
            var searchModel = new model_lt()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = new WhereLambda<Model_People, model_lt>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.lt] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };


            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Id < @Id And DataCreatedAt < @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5 },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

    

    }
}