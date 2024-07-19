using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause
{
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
                var whereLambda = new WhereLambda<Model_People, Input_numberRange_Attr>(searchModel);

                whereLambda[SearchType.NumberRange] = new List<string>
                {
                    nameof(searchModel.IdLeft),
                    nameof(searchModel.IdRight),
                };

                var expression = whereLambda.ToExpression();
                var searchCondition = expression.ToWhereClause();

                Assert.AreEqual(searchCondition.WhereClause, "Id >= @Id And Id <= @Id1");
                var dict = new Dictionary<string, object>
                {
                    { "@Id",searchModel.IdLeft},
                    { "@Id1",searchModel.IdRight},
                };

                DictionaryAssert.AreEqual(searchCondition.Parameters, dict);
            }

            {
                var searchModel = new Input_numberRange2()
                {
                    Id = 5
                };
                var whereLambda = new WhereLambda<Model_People, Input_numberRange2>(searchModel);

                whereLambda[SearchType.NumberRange] = new List<string>
                {
                    nameof(searchModel.Id),
                };

                var expression = whereLambda.ToExpression();
                var searchCondition = expression.ToWhereClause();

                Assert.AreEqual(searchCondition.WhereClause, "Id >= @Id And Id <= @Id1");
                var dict = new Dictionary<string, object>
                {
                    { "@Id",searchModel.Id},
                    { "@Id1",searchModel.Id},
                };

                DictionaryAssert.AreEqual(searchCondition.Parameters, dict);
            }
        }

        [TestMethod]
        public void Test_numberRange_whereif()
        {
            {
                var searchModel = new Input_numberRange2()
                {
                    Id = 5
                };
                var whereLambda = new WhereLambda<Model_People, Input_numberRange2>(searchModel);

                whereLambda[SearchType.NumberRange] = new List<string>
                {
                    nameof(searchModel.Id),
                };

                whereLambda.WhereIf[nameof(searchModel.Id)] = a => a.Id > 6;// 满足 条件时 生效

                var expression = whereLambda.ToExpression();
                var searchCondition = expression.ToWhereClause();

                Assert.AreEqual(searchCondition.WhereClause, "");
                var dict = new Dictionary<string, object>
                {
                };

                DictionaryAssert.AreEqual(searchCondition.Parameters, dict);
            }
        }
    }

    public class model_numberRange
    {
        public int? Id { get; set; }
    }

    public class Input_numberRange_Attr
    {
        [SearchType(SearchType.NumberRange)] public int? IdLeft { get; set; }
        [SearchType(SearchType.NumberRange)] public int? IdRight { get; set; }
    }

    public class Input_numberRange2
    {
        public int? Id { get; set; }
    }

    public class Input_numberRange2_Attr
    {
        [SearchType(SearchType.NumberRange)] public int? Id { get; set; }
    }
}
