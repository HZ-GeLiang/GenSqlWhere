using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause
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
        [SearchType(SearchType.In)] public string Id { get; set; }
        [SearchType(SearchType.In)] public string Sex { get; set; }

    }
    public class Input_in2_Attr
    {
        [SearchType(SearchType.In)] public int? Id { get; set; }

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

            var whereLambda = new WhereLambda<Model_People, model_in>(searchModel);

            whereLambda[SearchType.In] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.Sex),
            };


            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id) And Sex In (@Sex)");
            var dict = new Dictionary<string, object>
            {
                { "@Id", searchModel.Id},
                { "@Sex", searchModel.Sex}
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        [TestMethod]
        public void Test_in2()
        {
            var searchModel = new model_in2()
            {
                Id = 1
            };

            var whereLambda = new WhereLambda<Model_People, model_in2>(searchModel);

            whereLambda[SearchType.In] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id)");
            var dict = new Dictionary<string, object>
            {
                { "@Id", "1"},//in 可以有多个值,所以这个值就是stirng类型的
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }
}
