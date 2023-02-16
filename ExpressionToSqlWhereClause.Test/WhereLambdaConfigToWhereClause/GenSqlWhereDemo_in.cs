using System.Collections.Generic;
using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.WhereLambdaConfigToWhereClause
{
    public class model_in
    {
        //public int? Id { get; set; }
        public string Id { get; set; }
        public string Sex { get; set; }

    }
    public class model_in2
    {
        public int? Id { get; set; }

    }

    public class Input_in_Attr
    {
        [SearchType(SearchType.@in)] public string Id { get; set; }
        [SearchType(SearchType.@in)] public string Sex { get; set; }

    }
    public class Input_in2_Attr
    {
        [SearchType(SearchType.@in)] public int? Id { get; set; }

    }


    [TestClass]
    public class GenSqlWhereDemo_in
    {
        [TestMethod]
        public void Test_in()
        {
            var searchModel = new model_in()
            {
                Id = "1,2",
                Sex = "1",
            };

            var whereLambda = new WhereLambda<Model_People, model_in>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.Sex),
            };


            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Id In (@Id) And Sex In (@Sex)");
            var dict = new Dictionary<string, object>
            {
                { "@Id", searchModel.Id},
                { "@Sex", searchModel.Sex}
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_in2()
        {
            var searchModel = new model_in2()
            {
                Id = 1
            };

            var whereLambda = new WhereLambda<Model_People, model_in2>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Id In (@Id)");
            var dict = new Dictionary<string, object>
            {
                { "@Id", "1"},//in 可以有多个值,所以这个值就是stirng类型的
            };

            DictionaryAssert.AreEqual(param, dict);
        }
    }
}