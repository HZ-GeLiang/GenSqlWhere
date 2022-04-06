using System;
using System.Collections.Generic;
using ExpressionToSqlWhere.Test;
using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.EntityConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlWhere.ExpressionTree
{

    public class model_expection1
    {
        public int Id { get; set; }
    }
    public class model_expection2
    {
        public int? Id { get; set; }
    }

    [TestClass]
    public class GenSqlWhereDemo_exception
    {

        [TestMethod]
        public void Test_expection1()
        {
            {
                //string的null 转 int
                var searchModel = new
                {
                    Id = (string)null,
                };

                var whereLambda = searchModel.CrateWhereLambda((model_expection1 _) => { });

                whereLambda[SearchType.eq] = new List<string>
                {
                    nameof(searchModel.Id),
                };

                (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

                Assert.AreEqual(sql, String.Empty);

                DictionaryAssert.AreEqual(param, new Dictionary<string, object>());
            }

            {
                //解决  "" 转 值类型(如int) 抛异常的问题
                var searchModel = new
                {
                    Id = string.Empty,
                };

                var whereLambda = searchModel.CrateWhereLambda((model_expection1 _) => { });

                whereLambda[SearchType.eq] = new List<string>
                {
                    nameof(searchModel.Id),
                };

                (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

                Assert.AreEqual(sql, "");

                DictionaryAssert.AreEqual(param, new Dictionary<string, object>());

            }

            {
                //解决  "" 转 值类型(如int) 抛异常的问题
                var searchModel = new
                {
                    Id = string.Empty,
                };

                var whereLambda = searchModel.CrateWhereLambda((model_expection2 _) => { });

                whereLambda[SearchType.eq] = new List<string>
                {
                    nameof(searchModel.Id),
                };

                (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

                Assert.AreEqual(sql, "");

                DictionaryAssert.AreEqual(param, new Dictionary<string, object>());
            }
        }

        [TestMethod]
        public void Test_sqlShouldBeEmpty()
        {
            var searchModel = new model_lt()
            {

            };

            var whereLambda = new WhereLambda<Model_People, model_lt>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.lt] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };


            (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(sql, String.Empty);
            DictionaryAssert.AreEqual(param, new Dictionary<string, object>(0));

        }
    }
}