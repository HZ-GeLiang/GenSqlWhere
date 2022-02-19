using System;
using System.Collections.Generic;
using EntityToSqlWhereClauseConfig.SqlFunc;

using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    public class Input_sqlFun_Month
    {

        [Month]
        [SearchType(SearchType.eq)]
        public int DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }

    public class Model_sqlFun_Month
    {
        public DateTime DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }

    #region 待实现

    //todo: 提供demo

    public class Input_sqlFun_MonthIn1
    {
        [Month]
        public int DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }

    public class Input_sqlFun_MonthIn2
    {
        [MonthIn]
        public List<int> DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }

    public class Input_sqlFun_MonthIn3
    {
        [MonthIn]
        public string DataCreatedAt { get; set; } //最后要翻译为 List<int> 
    }
    public class Input_sqlFun_MonthIn4
    {
        [MonthIn]
        public int DataCreatedAt { get; set; } //最后要翻译为 List<int> 
    }
    public class Input_sqlFun_MonthIn5
    {
        [MonthIn]
        public List<string> DataCreatedAt { get; set; } //最后要翻译为 List<int> 
    }
    #endregion

    [TestClass]
    public class GenSqlWhereDemo_sqlFunc_Month
    {

        [TestMethod]
        public void Month_eq_一个月()
        {
            var searchModel = new Input_sqlFun_Month() //这里不能是匿名类型
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });

            var exp = whereLambda.ToExpression();

            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 1);//Month()返回的是int ,所以1 是int类型的才对
            Assert.AreEqual(whereClause, "Month(DataCreatedAt) = @Month");
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void Month_Neq_一个月()
        {
            var searchModel = new Input_sqlFun_Month
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });
            //whereLambda[SearchType.neq] = new List<string>
            //{
            //    nameof(searchModel.DataCreatedAt),
            //};

            var exp = whereLambda.ToExpression();

            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 1);//Month()返回的是int ,所以1 是int类型的才对
            Assert.AreEqual(whereClause, "Month(DataCreatedAt) <> @Month");
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }



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

    }
}