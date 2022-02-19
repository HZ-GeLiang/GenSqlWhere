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
        public int DataCreatedAt { get; set; } // ����� Entity ��������һ��
    }

    public class Model_sqlFun_Month
    {
        public DateTime DataCreatedAt { get; set; } // ����� Entity ��������һ��
    }

    #region ��ʵ��

    //todo: �ṩdemo

    public class Input_sqlFun_MonthIn1
    {
        [Month]
        public int DataCreatedAt { get; set; } // ����� Entity ��������һ��
    }

    public class Input_sqlFun_MonthIn2
    {
        [MonthIn]
        public List<int> DataCreatedAt { get; set; } // ����� Entity ��������һ��
    }

    public class Input_sqlFun_MonthIn3
    {
        [MonthIn]
        public string DataCreatedAt { get; set; } //���Ҫ����Ϊ List<int> 
    }
    public class Input_sqlFun_MonthIn4
    {
        [MonthIn]
        public int DataCreatedAt { get; set; } //���Ҫ����Ϊ List<int> 
    }
    public class Input_sqlFun_MonthIn5
    {
        [MonthIn]
        public List<string> DataCreatedAt { get; set; } //���Ҫ����Ϊ List<int> 
    }
    #endregion

    [TestClass]
    public class GenSqlWhereDemo_sqlFunc_Month
    {

        [TestMethod]
        public void Month_eq_һ����()
        {
            var searchModel = new Input_sqlFun_Month() //���ﲻ������������
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });

            var exp = whereLambda.ToExpression();

            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 1);//Month()���ص���int ,����1 ��int���͵ĲŶ�
            Assert.AreEqual(whereClause, "Month(DataCreatedAt) = @Month");
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void Month_Neq_һ����()
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
            expectedParameters.Add("@Month", 1);//Month()���ص���int ,����1 ��int���͵ĲŶ�
            Assert.AreEqual(whereClause, "Month(DataCreatedAt) <> @Month");
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }



        [TestMethod]
        public void Month_in_һ����_list����()
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
            expectedParameters.Add("@Month", 1);//Month()���ص���int ,����1 ��int���͵ĲŶ�
            Assert.AreEqual("(Month(DataCreatedAt) In (@Month))", whereClause);

            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }



        //todo:
        //[TestMethod]
        //public void Month_�����()
        //{
        //    //��֧��
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