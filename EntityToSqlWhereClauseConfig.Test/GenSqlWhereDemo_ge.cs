using System;
using System.Collections.Generic;

using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    public class Input_ge
    {
        public long? Id { get; set; }
        public DateTime? DataCreatedAt { get; set; }
    }

    public class Input_ge_Attr
    {
        [SearchType(SearchType.ge)] public long? Id { get; set; }
        [SearchType(SearchType.ge)] public DateTime? DataCreatedAt { get; set; }
    }

    [TestClass]
    public class GenSqlWhereDemo_ge
    {

        [TestMethod]
        public void Test_ge()
        {
            var searchModel = new Input_ge()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = new WhereLambda<People, Input_ge>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.ge] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };

            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Id >= @Id And DataCreatedAt >= @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5 },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

    }
}