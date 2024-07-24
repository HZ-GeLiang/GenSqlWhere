using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause
{
    [TestClass]
    public class GenSqlWhereDemo_neq
    {
        [TestMethod]
        public void Test_neq()
        {
            var searchModel = new model_neq()
            {
                IsDel = true,//todo://计划:添加当其他值为xx时,当前值才生效
            };

            var whereLambda = new WhereLambda<Model_People, model_neq>(searchModel);

            whereLambda[SearchType.Neq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "IsDel <> @IsDel");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", searchModel.IsDel }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }

    public class model_neq
    {
        public bool IsDel { get; set; }
    }

    public class Input_neq_Attr
    {
        [SearchType(SearchType.Neq)]
        public bool IsDel { get; set; }
    }
}