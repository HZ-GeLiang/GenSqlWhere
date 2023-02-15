using System.Collections.Generic;
using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.WhereLambdaConfigToWhereClause
{
    public class model_neq
    {
        public bool IsDel { get; set; }
    }

    public class Input_neq_Attr
    {
        [SearchType(SearchType.neq)] public bool IsDel { get; set; }
    }
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

            var whereLambda = new WhereLambda<Model_People, model_neq>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.neq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };

            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "((IsDel <> @IsDel))");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", searchModel.IsDel }
            };

            DictionaryAssert.AreEqual(param, dict);
        }
    }
}