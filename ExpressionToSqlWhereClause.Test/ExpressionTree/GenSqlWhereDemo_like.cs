using System.Collections.Generic;
using ExpressionToSqlWhere.Test;
using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.EntityConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlWhere.ExpressionTree
{
    public class model_like
    {
        public string Url { get; set; }
        public string Data_Remark { get; set; }
    }


    public class Input_like_Attr
    {
        [SearchType(SearchType.like)] public string Url { get; set; }
        [SearchType(SearchType.like)] public string Data_Remark { get; set; }
    }

    public class Input_likeLeft
    {
        public string Url { get; set; }
        public string Data_Remark { get; set; }
    }

    public class Input_likeLeft_Attr
    {
        [SearchType(SearchType.likeLeft)] public string Url { get; set; }
        [SearchType(SearchType.likeLeft)] public string Data_Remark { get; set; }
    }

    public class Input_likeRight
    {
        public string Url { get; set; }
        public string Data_Remark { get; set; }
    }

    public class Input_likeRight_Attr
    {
        [SearchType(SearchType.likeRight)] public string Url { get; set; }
        [SearchType(SearchType.likeRight)] public string Data_Remark { get; set; }
    }

    [TestClass]
    public class GenSqlWhereDemo_like
    {

        [TestMethod]
        public void Test_like()
        {
            var searchModel = new model_like()
            {
                Url = "123",
            };
            var whereLambda = new WhereLambda<Model_People, model_like>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.like] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };


            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Url Like @Url");
            var dict = new Dictionary<string, object>
            {
                { "@Url", "%123%" }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_likeLeft()
        {
            var searchModel = new Input_likeLeft()
            {
                Url = "123",
            };
            var whereLambda = new WhereLambda<Model_People, Input_likeLeft>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.likeLeft] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };


            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Url Like @Url");
            var dict = new Dictionary<string, object>
            {
                { "@Url", "123%" }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Test_likeRight()
        {
            var searchModel = new Input_likeRight()
            {
                Url = "123",
            };
            var whereLambda = new WhereLambda<Model_People, Input_likeRight>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.likeRight] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };

            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "Url Like @Url");
            var dict = new Dictionary<string, object>
            {
                { "@Url", "%123" }
            };

            DictionaryAssert.AreEqual(param, dict);
        }
    }
}