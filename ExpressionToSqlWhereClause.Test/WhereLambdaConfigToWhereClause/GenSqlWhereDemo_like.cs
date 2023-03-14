using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.Test.WhereLambdaConfigToWhereClause
{

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
            whereLambda.Search = searchModel;

            whereLambda[SearchType.Like] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Url Like @Url");
            var dict = new Dictionary<string, object>
            {
                { "@Url", "%123%" }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        [TestMethod]
        public void Test_likeLeft()
        {
            var searchModel = new Input_likeLeft()
            {
                Url = "123",
            };
            var whereLambda = new WhereLambda<Model_People, Input_likeLeft>();
            whereLambda.Search = searchModel;

            whereLambda[SearchType.LikeLeft] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };


            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Url Like @Url");
            var dict = new Dictionary<string, object>
            {
                { "@Url", "123%" }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        [TestMethod]
        public void Test_likeRight()
        {
            var searchModel = new Input_likeRight()
            {
                Url = "123",
            };
            var whereLambda = new WhereLambda<Model_People, Input_likeRight>();
            whereLambda.Search = searchModel;

            whereLambda[SearchType.LikeRight] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Url Like @Url");
            var dict = new Dictionary<string, object>
            {
                { "@Url", "%123" }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }


    public class model_like
    {
        public string Url { get; set; }
        public string Data_Remark { get; set; }
    }


    public class Input_like_Attr
    {
        [SearchType(SearchType.Like)] public string Url { get; set; }
        [SearchType(SearchType.Like)] public string Data_Remark { get; set; }
    }

    public class Input_likeLeft
    {
        public string Url { get; set; }
        public string Data_Remark { get; set; }
    }

    public class Input_likeLeft_Attr
    {
        [SearchType(SearchType.LikeLeft)] public string Url { get; set; }
        [SearchType(SearchType.LikeLeft)] public string Data_Remark { get; set; }
    }

    public class Input_likeRight
    {
        public string Url { get; set; }
        public string Data_Remark { get; set; }
    }

    public class Input_likeRight_Attr
    {
        [SearchType(SearchType.LikeRight)] public string Url { get; set; }
        [SearchType(SearchType.LikeRight)] public string Data_Remark { get; set; }
    }
}