using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WithEFTest.ExtensionMethods;

var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
    .WhereIf(true, a => a.Id == 1)
    .WhereIf(true, a => a.Name == "abc")
    .WhereIf(true, a => a.CreateAt > new DateTime(2023, 1, 1))
    .WhereIf(true, a => a.Flag.Value == Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81"));

SearchCondition searchCondition = expression.ToWhereClause();

var whereClause = WhereClauseHelper.GetFormattableStringClause(searchCondition);

string sql = $@"select * from ExpressionSqlTest where {whereClause} ";

FormattableString? formattableStr = WhereClauseHelper.CreateFormattableString(sql, searchCondition.Parameters.Values.ToArray());

using var db = new TestContext();

var list = db.ExpressionSqlTests.FromSqlInterpolated(formattableStr);
foreach (var item in list)
{
    Console.WriteLine(item);
}