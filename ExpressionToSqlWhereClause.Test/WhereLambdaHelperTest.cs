﻿using ExpressionToSqlWhereClause.Helpers;
using ExpressionToSqlWhereClause.Test.LambdaToWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToSqlWhereClause.Test;

[TestClass]
public class WhereLambdaHelperTest
{
    [TestMethod]
    public void test_GetNonParameterClause_02()
    {
        var expression =
            default(Expression<Func<Test_001, bool>>)
            .WhereIf(true, a => a.Message == "abc")
            .WhereIf(true, a => a.MessageType == 1);

        var searchCondition = expression.ToWhereClause();
        var clause = searchCondition.WhereClause;
        var param = searchCondition.Parameters;

        StringBuilder sb_clause = new StringBuilder();
        if (clause.Length > 0)
        {
            sb_clause.Append(" and " + clause);
        }

        StringBuilder sb_sql_count = new StringBuilder("SELECT count(*) FROM xxx ");
        StringBuilder sb_sql = new StringBuilder($"SELECT * FROM xxx ");

        if (sb_clause.Length > 0)
        {
            sb_clause.Remove(0, 4);
            sb_sql_count.Append(" where ").Append(sb_clause);
            sb_sql.Append(" where ").Append(sb_clause);
        }

        sb_sql.Append($" order by Id ");
        var limit_offset = 0;
        var limit_startLine = 10;
        sb_sql.Append($" LIMIT {limit_startLine} offset {limit_offset} ");

        var sql_count = sb_sql_count.ToString();
        var sql = sb_sql.ToString();

        var debug_sql_count = WhereClauseHelper.GetNonParameterClause(sql_count, param);
        var debug_sql = WhereClauseHelper.GetNonParameterClause(sql, param);

        Assert.AreEqual("SELECT count(*) FROM xxx  where  Message = 'abc' And MessageType = 1", debug_sql_count);
        Assert.AreEqual("SELECT * FROM xxx  where  Message = 'abc' And MessageType = 1 order by Id  LIMIT 10 offset 0", debug_sql);
    }

    [TestMethod]
    public void Test_timeRange_MergeParametersIntoSql()
    {
        {
            DateTime? d1 = (DateTime?)DateTime.Parse("2022-3-1 15:12:34");
            DateTime? d2 = (DateTime?)DateTime.Parse("2022-3-3 12:34:56");
            var expression = default(Expression<Func<Input_timeRange2_Attr, bool>>)
              .WhereIf(true, a => a.DataCreatedAt >= d1)
              .WhereIf(true, a => a.DataCreatedAt < d2);

            var searchCondition = expression.ToWhereClause();

            var formatDateTime = new Dictionary<string, string>() { { "@DataCreatedAt1", "yyyy-MM-dd" } };
            var caluse = WhereClauseHelper.GetNonParameterClause(searchCondition, formatDateTime);
            Assert.AreEqual(caluse, "DataCreatedAt >= '2022-03-01 15:12:34' And DataCreatedAt < '2022-03-03'");
        }

        {
            DateTime? d1 = (DateTime?)DateTime.Parse("2022-3-1 15:12:34");
            DateTime? d2 = (DateTime?)DateTime.Parse("2022-3-3 12:34:56");
            var expression = default(Expression<Func<Input_timeRange2_Attr, bool>>)
              .WhereIf(true, a => a.DataCreatedAt >= d1)
              .WhereIf(true, a => a.DataCreatedAt < d2);

            var searchCondition = expression.ToWhereClause();

            var formatDateTime = WhereClauseHelper.GetDefaultFormatDateTime(searchCondition.Parameters);
            var caluse = WhereClauseHelper.GetNonParameterClause(searchCondition, formatDateTime);
            Assert.AreEqual(caluse, "DataCreatedAt >= '2022-03-01 15:12:34' And DataCreatedAt < '2022-03-03 12:34:56'");
        }
    }

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