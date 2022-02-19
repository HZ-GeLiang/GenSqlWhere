using System;
using System.Collections.Generic;

using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    public class Input_eq
    {
        [SearchType(SearchType.eq)] public bool IsDel { get; set; }
    }

    public class Model_eq
    {
        public bool IsDel { get; set; }
    }


    [TestClass]
    public class GenSqlWhereDemo_eq
    {
        [TestMethod]
        public void Test_eq()
        {
            var searchModel = new Model_eq()
            {
                IsDel = true,//todo://计划:添加当其他值为xx时,当前值才生效
            };

            var whereLambda = new WhereLambda<Input_eq, Model_eq>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.eq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };

            var expression = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = expression.ToWhereClause();

            Assert.AreEqual(sql, "IsDel = @IsDel");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", searchModel.IsDel }
            };

            DictionaryAssert.AreEqual(param, dict);
        }

    }
}