using System.Collections.Generic;
using ExpressionToSqlWhere.Test;
using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.EntityConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlWhere.ExpressionTree
{
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
            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Id", 1);//Month()返回的是int ,所以1 是int类型的才对
            expectedParameters.Add("@Id1", 1);//Month()返回的是int ,所以1 是int类型的才对
            Assert.AreEqual(whereClause, "Id = @Id And Id <> @Id1");
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            (string whereClause, Dictionary<string, object> parameters) = exp.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Id", 1);
            Assert.AreEqual(whereClause, "Id = @Id");
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }
    }
}