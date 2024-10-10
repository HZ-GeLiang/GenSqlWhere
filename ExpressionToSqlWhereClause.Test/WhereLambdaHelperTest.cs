using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Helpers;
using ExpressionToSqlWhereClause.Test.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause
{
    [TestClass]
    public class WhereLambdaHelperTest
    {
        [TestMethod]
        public void GetExpression_HasValue()
        {
            Expression<Func<Student, bool>> exp = a => (a.Name ?? "") != "";
            var exp2 = WhereLambdaHelper.GetExpression_HasValue<Student>("Name");

            Assert.AreEqual(true, exp.ToString() == exp2.ToString());

            //配合扩展方法的使用
            {
                IQueryable<Student> query = null;
                query?.WhereNoValue(a => a.Name);
            }
        }

        [TestMethod]
        public void GetNonParameterClause优化_list()
        {
            var list = new List<string>() { "1", "2" };
            var expression = default(Expression<Func<Test_001, bool>>).WhereIf(true, a => list.Contains(a.Message));

            SearchCondition searchCondition = expression.ToWhereClause();

            var clause = WhereClauseHelper.GetNonParameterClause(searchCondition);

            Assert.AreEqual(clause, "Message In ('1','2')");
        }

        [TestMethod]
        public void GetNonParameterClause优化_string()
        {
            var expression = default(Expression<Func<Test_001, bool>>).WhereIf(true, a => a.Message == "'123");

            var searchCondition = expression.ToWhereClause();

            var clause = WhereClauseHelper.GetNonParameterClause(searchCondition);

            Assert.AreEqual(clause, "Message = '''123'");
        }

        [TestMethod]
        public void GetNonParameterClause优化_bool()
        {
            {
                {
                    var expression = default(Expression<Func<Test_001, bool>>).WhereIf(true, a => a.IsDel == true);

                    var searchCondition = expression.ToWhereClause();

                    var clause = WhereClauseHelper.GetNonParameterClause(searchCondition);

                    Assert.AreEqual(clause, "IsDel = 1");
                }

                {
                    var expression = default(Expression<Func<Test_001, bool>>).WhereIf(true, a => a.IsDel == false);

                    var searchCondition = expression.ToWhereClause();

                    var clause = WhereClauseHelper.GetNonParameterClause(searchCondition);

                    Assert.AreEqual(clause, "IsDel = 0");
                }
            }

            {
                {
                    var expression = default(Expression<Func<Test_001, bool>>).WhereIf(true, a => a.IsDel2 == true);

                    var searchCondition = expression.ToWhereClause();

                    var clause = WhereClauseHelper.GetNonParameterClause(searchCondition);

                    Assert.AreEqual(clause, "IsDel2 = 1");
                }

                {
                    var expression = default(Expression<Func<Test_001, bool>>).WhereIf(true, a => a.IsDel2 == false);

                    var searchCondition = expression.ToWhereClause();

                    var clause = WhereClauseHelper.GetNonParameterClause(searchCondition);

                    Assert.AreEqual(clause, "IsDel2 = 0");
                }
            }
        }

        [TestMethod]
        public void 属性名存在包含关系()
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
        public void 属性名存在包含关系2()
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
        public void 属性名存在包含关系3()
        {
            var expression =
                default(Expression<Func<Test_001, bool>>)
                .WhereIf(true, a => a.MessageType == 1)
                .WhereIf(true, a => a.Message == "abc");//对比上面, 顺序不一样, 问题修复后, 这里的单元测试应该是要通过的

            var searchCondition = expression.ToWhereClause();

            var clause = WhereClauseHelper.GetNumberParameterClause(searchCondition);

            Assert.AreEqual(clause, "MessageType = @0 And Message = @1");
        }

        [TestMethod]
        public void GetExpression_NotDeleted()
        {
        }
    }

    internal class Test_001
    {
        public bool IsDel { get; set; }
        public bool? IsDel2 { get; set; }
        public string Message { get; set; }

        public int MessageType { get; set; } // MessageType 包含 Message
    }

    internal class Student
    {
        public string Name { get; set; }

        public int IsDeleted1 { get; set; }
        public int? IsDeleted2 { get; set; }
        public bool IsDeleted3 { get; set; }
        public bool? IsDeleted4 { get; set; }
        public short IsDeleted5 { get; set; }
        public short? IsDeleted6 { get; set; }
    }
}