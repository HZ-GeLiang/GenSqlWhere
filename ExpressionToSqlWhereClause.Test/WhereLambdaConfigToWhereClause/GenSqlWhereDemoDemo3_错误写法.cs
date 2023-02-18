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
        public void 重复配置使用方法_不同配置()
        {
            //todo: 重复配置的检测
            var searchModel = new Input_Demo3_不同配置
            {
                Id = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((model_Demo3 p) => { });
            // 已经在 input 模型上标注了
            whereLambda[SearchType.neq] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var exp = whereLambda.ToExpression();
            var searchCondition = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Id", 1);//Month()返回的是int ,所以1 是int类型的才对
            expectedParameters.Add("@Id1", 1);//Month()返回的是int ,所以1 是int类型的才对
            Assert.AreEqual(searchCondition.WhereClause, "Id = @Id And Id <> @Id1");
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void 重复配置使用方法_相同配置()
        {
            var searchModel = new Input_Demo3_相同配置
            {
                Id = 1
            };
            var whereLambda = searchModel.CrateWhereLambda((model_Demo3 p) => { });
            // 已经在 input 模型上标注了
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

        public class Input_Demo3_不同配置
        {
            [SearchType(SearchType.eq)]
            public int Id { get; set; }
        }
        public class Input_Demo3_相同配置
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