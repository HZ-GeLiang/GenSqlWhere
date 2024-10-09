using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.Helper;
using ExpressionToSqlWhereClause.Test.ExtensionMethod;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause
{
    public class Test_001
    {
        public string Message { get; set; }

        public int MessageType { get; set; } // MessageType 包含 Message
    }

    [TestClass]
    public class ExpressionDemo_属性名存在包含关系
    {
        [TestMethod]
        public void test()
        {
            var expression =
                default(Expression<Func<Test_001, bool>>)
                .WhereIf(true, a => a.Message == "abc")
                .WhereIf(true, a => a.MessageType == 1);

            var searchCondition = expression.ToWhereClause();

            var clause = WhereClauseHelper.GetNonParameterClause(searchCondition);

            Assert.AreEqual(clause, "Message = 'abc' And MessageType = 1");
        }

        [TestMethod]
        public void test02()
        {
            var expression =
                default(Expression<Func<Test_001, bool>>)
                .WhereIf(true, a => a.MessageType == 1)
                .WhereIf(true, a => a.Message == "abc");//对比上面, 顺序不一样, 问题修复后, 这里的单元测试应该是要通过的

            var searchCondition = expression.ToWhereClause();

            var clause = WhereClauseHelper.GetNonParameterClause(searchCondition);

            Assert.AreEqual(clause, "MessageType = 1 And Message = 'abc'");
        }

        [TestMethod]
        public void test03()
        {
            var expression =
                default(Expression<Func<Test_001, bool>>)
                .WhereIf(true, a => a.MessageType == 1)
                .WhereIf(true, a => a.Message == "abc");//对比上面, 顺序不一样, 问题修复后, 这里的单元测试应该是要通过的

            var searchCondition = expression.ToWhereClause();

            var clause = WhereClauseHelper.GetNumberParameterClause(searchCondition);

            Assert.AreEqual(clause, "MessageType = @0 And Message = @1");
        }
    }
}