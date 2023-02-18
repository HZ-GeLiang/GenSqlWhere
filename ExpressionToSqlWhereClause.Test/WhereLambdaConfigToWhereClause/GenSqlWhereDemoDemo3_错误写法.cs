using System.Collections.Generic;
using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.WhereLambdaConfigToWhereClause
{
    [TestClass]
    public class UseDemo3
    {

        [TestMethod]
        public void �ظ�����ʹ�÷���_��ͬ����()
        {
            //todo: �ظ����õļ��
            var searchModel = new Input_Demo3_��ͬ����
            {
                Id = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((model_Demo3 p) => { });
            // �Ѿ��� input ģ���ϱ�ע��
            whereLambda[SearchType.neq] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var exp = whereLambda.ToExpression();
            var searchCondition = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Id", 1);//Month()���ص���int ,����1 ��int���͵ĲŶ�
            expectedParameters.Add("@Id1", 1);//Month()���ص���int ,����1 ��int���͵ĲŶ�
            Assert.AreEqual(searchCondition.WhereClause, "Id = @Id And Id <> @Id1");
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void �ظ�����ʹ�÷���_��ͬ����()
        {
            var searchModel = new Input_Demo3_��ͬ����
            {
                Id = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((model_Demo3 p) => { });
            // �Ѿ��� input ģ���ϱ�ע��
            whereLambda[SearchType.eq] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var exp = whereLambda.ToExpression();
            var searchCondition = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Id", 1);
            Assert.AreEqual(searchCondition.WhereClause, "Id = @Id");
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        public class Input_Demo3_��ͬ����
        {
            [SearchType(SearchType.eq)]
            public int Id { get; set; }
        }
        public class Input_Demo3_��ͬ����
        {
            [SearchType(SearchType.eq)]
            public int Id { get; set; }
        }
        public class model_Demo3
        {
            [SearchType(SearchType.eq)]
            public int Id { get; set; }
        }
    }
}