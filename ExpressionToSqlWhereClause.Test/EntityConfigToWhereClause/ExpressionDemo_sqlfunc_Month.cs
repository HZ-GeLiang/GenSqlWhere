﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.SqlFunc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause
{


    [TestClass]
    public class ExpressionDemo_sqlfunc_Month
    {
        public void Test_Expression_compile()
        {
            Expression<Func<User_SqlFunc_Entity, bool>> expression =
                u => DbFunctions.Month(u.CreateAt) == 5;
        }

        [TestMethod]
        public void SqlFunc_Month_eq()
        {
            Expression<Func<User_SqlFunc_Entity, bool>> expression = u => DbFunctions.Month(u.CreateAt) == 5;
            var searchCondition = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Month", 5 }
            };
            Assert.AreEqual("Month(CreateAt) = @Month", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }


        [TestMethod]
        public void SqlFunc_MonthIn_eq()
        {
            //EntityToSqlWherecClauseConfig 的 in 
            Expression<Func<User_SqlFunc_Entity, bool>> expression =
                u => DbFunctions.MonthIn(u.CreateAt) == new List<int> { 5, 6 };//string 需要自己转成 List<int> , 这里略,直接写死
            var searchCondition = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@MonthIn", "5,6" }
            };
            Assert.AreEqual("Month(CreateAt) In (@MonthIn)", searchCondition.WhereClause);

            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);

        }

    }

    public class User_SqlFunc_Entity
    {
        public DateTime CreateAt { get; set; }
        public DateTime DelAt { get; set; }
        public int Month { get; set; }
    }

}