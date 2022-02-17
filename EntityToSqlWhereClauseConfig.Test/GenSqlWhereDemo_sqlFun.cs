using System;
using System.Collections.Generic;
using EntityToSqlWhereClauseConfig.Test.input;
using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    [TestClass]
    public class GenSqlWhereDemo_sqlFun
    {
        //todo:
        //[TestMethod]
        //public void Month_多个月()
        //{
        //    //不支持
        //    var searchModel = new Input_sqlFun_Month2()
        //    {
        //        DataCreatedAt = "1,2"
        //    };
        //    var whereLambda = searchModel.CrateWhereLambda((Input_sqlFun_Month2 p) => { });
        //    whereLambda[SearchType.@in] = new List<string>
        //    {
        //        nameof(searchModel.DataCreatedAt),
        //    };
        //    var exp = whereLambda.ToExpression();

        //    (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();
        //}


        [TestMethod]
        public void Month_in_一个月_list类型()
        {
            var searchModel = new Input_sqlFun_MonthIn1()
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Input_sqlFun_MonthIn1 p) => { });
            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.DataCreatedAt),
            };
            var exp = whereLambda.ToExpression();

            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 1);//Month()返回的是int ,所以1 是int类型的才对
            Assert.AreEqual("(Month(DataCreatedAt) In (@Month))", whereClause);
            
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }


        [TestMethod]
        public void Month_Neq_一个月()
        {
            var searchModel = new Input_sqlFun_Month()
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Input_sqlFun_Month p) => { });
            whereLambda[SearchType.neq] = new List<string>
            {
                nameof(searchModel.DataCreatedAt),
            };
            var exp = whereLambda.ToExpression();

            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 1);//Month()返回的是int ,所以1 是int类型的才对
            Assert.AreEqual("Month(DataCreatedAt) <> @Month", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void Month_eq_一个月()
        {
            var searchModel = new Input_sqlFun_Month()
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Input_sqlFun_Month p) => { });
            whereLambda[SearchType.eq] = new List<string>
            {
                nameof(searchModel.DataCreatedAt),
            };
            var exp = whereLambda.ToExpression();

            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 1);//Month()返回的是int ,所以1 是int类型的才对
            Assert.AreEqual("Month(DataCreatedAt) = @Month", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }
    }
}