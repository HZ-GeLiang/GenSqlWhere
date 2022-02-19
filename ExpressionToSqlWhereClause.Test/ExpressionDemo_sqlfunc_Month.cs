using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.Test.ExtensionMethod;
using ExpressionToSqlWhereClause.Test.InputOrModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test
{
    [TestClass]
    public class ExpressionDemo_sqlfunc_Month
    {
        public void Test_Expression_compile()
        {
            Expression<Func<User_SqlFunc_Entity, bool>> expression =
                u => SqlFunc.DbFunctions.Month(u.CreateAt) == 5;
        }

        [TestMethod]
        public void SqlFunc_Month_eq()
        { 
            Expression<Func<User_SqlFunc_Entity, bool>> expression = u => SqlFunc.DbFunctions.Month(u.CreateAt) == 5;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Month", 5 }
            };
            Assert.AreEqual("Month(CreateAt) = @Month", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }


        [TestMethod]
        public void SqlFunc_MonthIn_eq()
        {
            //EntityToSqlWherecClauseConfig 的 in 
            Expression<Func<User_SqlFunc_Entity, bool>> expression =
                u => SqlFunc.DbFunctions.MonthIn(u.CreateAt) == new List<int> { 5, 6 };//string 需要自己转成 List<int> , 这里略,直接写死
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@MonthIn", "5,6" }
            };
            Assert.AreEqual("((Month(CreateAt) In (@MonthIn)))", whereClause);

            DictionaryAssert.AreEqual(expectedParameters, parameters);

        }

    }
}