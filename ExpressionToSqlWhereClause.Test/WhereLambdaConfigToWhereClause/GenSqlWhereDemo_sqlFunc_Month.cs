﻿using System;
using System.Collections.Generic;
using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.SqlFunc.EntityConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.WhereLambdaConfigToWhereClause
{

    [TestClass]
    public class GenSqlWhereDemo_sqlFunc_Month
    {

        [TestMethod]
        public void Month_eq()
        {
            var searchModel = new Input_sqlFun_Month_eq() //这里不能是匿名类型
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });

            var exp = whereLambda.ToExpression();

            var searchCondition = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 1);//Month()返回的是int ,所以1 是int类型的才对
            Assert.AreEqual(searchCondition.WhereClause, "Month(DataCreatedAt) = @Month");
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void Month_Neq()
        {
            var searchModel = new Input_sqlFun_Month_neq
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });

            var exp = whereLambda.ToExpression();

            var searchCondition = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 1);//Month()返回的是int ,所以1 是int类型的才对
            Assert.AreEqual(searchCondition.WhereClause, "Month(DataCreatedAt) <> @Month");
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }
        [TestMethod]
        public void Month_In()
        {
            var searchModel = new Input_sqlFun_Month_in
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });

            //SearchType:in,操作遇到不支持的属性类型:System.DateTime
            var exMsg = $"SearchType:{nameof(SearchType.@in)},操作遇到不支持的属性类型:{typeof(DateTime).FullName}";

            Assert.ThrowsException<ExpressionToSqlWhereClauseException>(() => whereLambda.ToExpression().ToWhereClause(), exMsg);

        }


        [TestMethod]
        public void MonthIn_eq_1()
        {
            var searchModel = new Input_sqlFun_MonthIn1()
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });
            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.DataCreatedAt),
            };
            var exp = whereLambda.ToExpression();

            var searchCondition = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@MonthIn", "1"); //sql 的 in(变量), 这个变量只能写一个, 所以,这里是string
            Assert.AreEqual(searchCondition.WhereClause, "Month(DataCreatedAt) In (@MonthIn)");

            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void MonthIn_eq_2()
        {
            var searchModel = new Input_sqlFun_MonthIn2()
            {
                DataCreatedAt = "1,2,3"
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });
            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.DataCreatedAt),
            };
            var exp = whereLambda.ToExpression();

            var searchCondition = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@MonthIn", "1,2,3"); //sql 的 in(变量), 这个变量只能写一个, 所以,这里是string
            Assert.AreEqual(searchCondition.WhereClause, "Month(DataCreatedAt) In (@MonthIn)");

            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void MonthIn_eq_3()
        {
            var searchModel = new Input_sqlFun_MonthIn3()
            {
                DataCreatedAt = new List<int> { 1, 2, 3 }
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });
            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.DataCreatedAt),
            };
            var exp = whereLambda.ToExpression();

            var searchCondition = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@MonthIn", "1,2,3"); //sql 的 in(变量), 这个变量只能写一个, 所以,这里是string
            Assert.AreEqual(searchCondition.WhereClause, "Month(DataCreatedAt) In (@MonthIn)");

            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

    }

    public class Input_sqlFun_Month_eq
    {
        [Month]
        [SearchType(SearchType.eq)]
        public int DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }
    public class Input_sqlFun_Month_neq
    {
        [Month]
        [SearchType(SearchType.neq)]
        public int DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }
    public class Input_sqlFun_Month_in
    {
        [Month]
        [SearchType(SearchType.@in)]
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
        [MonthIn]
        public int DataCreatedAt { get; set; } //最后要翻译为 List<int> 
    }

    public class Input_sqlFun_MonthIn2
    {
        [MonthIn]
        public string DataCreatedAt { get; set; } //最后要翻译为 List<int> 
    }

    public class Input_sqlFun_MonthIn3
    {
        [MonthIn]
        public List<int> DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }

    #endregion
}