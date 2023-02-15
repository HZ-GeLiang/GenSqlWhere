using System.Collections.Generic;
using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.WhereLambdaConfigToWhereClause
{

    public class model_numberRange
    {
        public int? Id { get; set; }
    }

    public class Input_numberRange_Attr
    {
        [SearchType(SearchType.numberRange)] public int? IdLeft { get; set; }
        [SearchType(SearchType.numberRange)] public int? IdRight { get; set; }
    }



    public class Input_numberRange2
    {
        public int? Id { get; set; }
    }
    public class Input_numberRange2_Attr
    {
        [SearchType(SearchType.numberRange)] public int? Id { get; set; }

    }

    [TestClass]
    public class GenSqlWhereDemo_numberRange
    {
        [TestMethod]
        public void Test_numberRange()
        {
            {
                var searchModel = new Input_numberRange_Attr()
                {
                    IdLeft = 3,
                    IdRight = 9
                };
                var whereLambda = new WhereLambda<Model_People, Input_numberRange_Attr>();
                whereLambda.SearchModel = searchModel;
                whereLambda[SearchType.numberRange] = new List<string>
                {
                    nameof(searchModel.IdLeft),
                    nameof(searchModel.IdRight),
                };

                var expression = whereLambda.ToExpression();
                (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

                Assert.AreEqual(sql, "(((Id >= @Id)) And ((Id <= @Id1)))");
                var dict = new Dictionary<string, object>
                {
                    { "@Id",searchModel.IdLeft},
                    { "@Id1",searchModel.IdRight},
                };

                DictionaryAssert.AreEqual(param, dict);
            }

            {
                var searchModel = new Input_numberRange2()
                {
                    Id = 5
                };
                var whereLambda = new WhereLambda<Model_People, Input_numberRange2>();
                whereLambda.SearchModel = searchModel;
                whereLambda[SearchType.numberRange] = new List<string>
                {
                    nameof(searchModel.Id),
                };


                var expression = whereLambda.ToExpression();
                (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

                Assert.AreEqual(sql, "(((Id >= @Id)) And ((Id <= @Id1)))");
                var dict = new Dictionary<string, object>
                {
                    { "@Id",searchModel.Id},
                    { "@Id1",searchModel.Id},
                };

                DictionaryAssert.AreEqual(param, dict);
            }
        }
    }
}