using System;
using System.Collections.Generic;
using EntityToSqlWhereClauseConfig.SqlFunc;

using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    public class Input_sqlFun_Month_eq
    {
        [Month]
        [SearchType(SearchType.eq)]
        public int DataCreatedAt { get; set; } // ����� Entity ��������һ��
    }
    public class Input_sqlFun_Month_neq
    {
        [Month]
        [SearchType(SearchType.neq)]
        public int DataCreatedAt { get; set; } // ����� Entity ��������һ��
    }
    public class Input_sqlFun_Month_in
    {
        [Month]
        [SearchType(SearchType.@in)]
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
        [MonthIn]
        public int DataCreatedAt { get; set; } //���Ҫ����Ϊ List<int> 
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


    #endregion

    [TestClass]
    public class GenSqlWhereDemo_sqlFunc_Month
    {

        [TestMethod]
        public void Month_eq()
        {
            var searchModel = new Input_sqlFun_Month_eq() //���ﲻ������������
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
        public void Month_Neq()
        {
            var searchModel = new Input_sqlFun_Month_neq
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });

            var exp = whereLambda.ToExpression();

            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 1);//Month()���ص���int ,����1 ��int���͵ĲŶ�
            Assert.AreEqual(whereClause, "Month(DataCreatedAt) <> @Month");
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }
        [TestMethod]
        public void Month_In()
        {
            var searchModel = new Input_sqlFun_Month_in
            {
                DataCreatedAt = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((Model_sqlFun_Month p) => { });

            //SearchType:in,����������֧�ֵ���������:System.DateTime
            var exMsg = $"SearchType:{nameof(SearchType.@in)},����������֧�ֵ���������:{typeof(DateTime).FullName}";

            Assert.ThrowsException<Exceptions.EntityToSqlWhereCaluseConfigException>(() => whereLambda.ToExpression().ToWhereClause(), exMsg);

        }


        [TestMethod]
        public void MonthIn_eq()
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

            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@MonthIn", "1"); //sql �� in(����), �������ֻ��дһ��, ����,������string
            Assert.AreEqual("((Month(DataCreatedAt) In (@MonthIn)))", whereClause);

            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

    }
}