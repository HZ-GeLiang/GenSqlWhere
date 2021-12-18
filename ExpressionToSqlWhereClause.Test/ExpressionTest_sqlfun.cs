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
    public class ExpressionTest_sqlfun
    {
        public void Test_Expression_compile()
        {
            Expression<Func<User_SqlFunc_Entity, bool>> expression =
                u => SqlFunc.DbFunctions.Month(u.CreateAt) == 5;
        }

        [TestMethod]
        public void SqlFunc_Month_In_多个月()
        {
            //EntityToSqlWherecClauseConfig 的 in
            var userFilter = new User_SqlFunc3() { CreateAtMonth = "5,6" };
            Expression<Func<User_SqlFunc_Entity, bool>> expression =
                u => SqlFunc.DbFunctions.MonthIn(u.CreateAt) == new List<int> { 5, 6 };//string 需要自己转成 List<int> , 这里略,直接写死
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@MonthIn", new List<int> { 5 ,6 });
            Assert.AreEqual("Month(CreateAt) In @MonthIn", whereClause);

            DictionaryAssert.AreEqual(expectedParameters, parameters);

        }

        [TestMethod]
        public void SqlFunc_Month_In_一个月()
        {
            //EntityToSqlWherecClauseConfig 的 in
            var userFilter = new User_SqlFunc3() { CreateAtMonth = "5" };
            Expression<Func<User_SqlFunc_Entity, bool>> expression =
                u => SqlFunc.DbFunctions.MonthIn(u.CreateAt) == new List<int> { 5 };//string 需要自己转成 List<int> , 这里略,直接写死
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@MonthIn", new List<int> { 5});
            Assert.AreEqual("Month(CreateAt) In @MonthIn", whereClause);

            DictionaryAssert.AreEqual(expectedParameters, parameters);

        }

        [TestMethod]
        public void SqlFunc_Month_Eq_多个月()
        {
            //EntityToSqlWherecClauseConfig 的 in
            var userFilter = new User_SqlFunc2() { CreateAtMonth = new List<int> { 5, 6 } };
            Expression<Func<User_SqlFunc_Entity, bool>> expression =
                u => (SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth[0] ||
                SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth[1]
                ); //如果有多个条件, 这个()不能去掉
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 5);
            expectedParameters.Add("@Month1", 6);
            //Assert.AreEqual("((Age < @Age))", whereClause);
            Assert.AreEqual("(((Month(CreateAt) = @Month)) Or ((Month(CreateAt) = @Month1)))", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void SqlFunc_Month_Eq_一个月()
        {
            {
                var userFilter = new User_SqlFunc() { Month = 1, CreateAtMonth = 5 };
                Expression<Func<User_SqlFunc_Entity, bool>> expression =
                    u => SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth &&
                         u.Month == userFilter.Month;
                (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
                Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
                expectedParameters.Add("@Month", 5);
                expectedParameters.Add("@Month1", 1);
                Assert.AreEqual("Month(CreateAt) = @Month And Month = @Month1", whereClause);
                DictionaryAssert.AreEqual(expectedParameters, parameters);
            }
            {
                var userFilter = new User_SqlFunc() { Month = 1, CreateAtMonth = 5 };
                Expression<Func<User_SqlFunc_Entity, bool>> expression =
                    u => u.Month == userFilter.Month &&
                         SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth;
                (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
                Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
                expectedParameters.Add("@Month", 1);
                expectedParameters.Add("@Month1", 5);
                Assert.AreEqual("Month = @Month And Month(CreateAt) = @Month1", whereClause);
                DictionaryAssert.AreEqual(expectedParameters, parameters);
            }

            {
                var userFilter = new User_SqlFunc() { CreateAtMonth = 5, DelAtMonth = 6 };
                Expression<Func<User_SqlFunc_Entity, bool>> expression =
                    u => SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth &&
                         SqlFunc.DbFunctions.Month(u.DelAt) == userFilter.DelAtMonth;
                (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
                Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
                expectedParameters.Add("@Month", 5);
                expectedParameters.Add("@Month1", 6);
                Assert.AreEqual("Month(CreateAt) = @Month And Month(DelAt) = @Month1", whereClause);
                DictionaryAssert.AreEqual(expectedParameters, parameters);
            }

            {
                var userFilter = new User_SqlFunc() { CreateAtMonth = 5 };
                Expression<Func<User_SqlFunc_Entity, bool>> expression =
                    u => SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth;
                (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
                Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
                expectedParameters.Add("@Month", 5);
                Assert.AreEqual("Month(CreateAt) = @Month", whereClause);
                DictionaryAssert.AreEqual(expectedParameters, parameters);
            }
        }
    }
}