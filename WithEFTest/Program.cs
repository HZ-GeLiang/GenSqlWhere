using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WithEFTest.ExtensionMethod;

var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
    .WhereIf(true, a => a.Id == 1)
    .WhereIf(true, a => a.Name == "abc")
    .WhereIf(true, a => a.CreateAt > new DateTime(2023, 1, 1))
    .WhereIf(true, a => a.Flag.Value == Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81"))
    ;

var searchCondition = expression.ToFormattableWhereClause();

string sql = $@"select * from ExpressionSqlTest where {searchCondition.WhereClause} ";

//var formattableStr = FormattableStringFactory.Create(sql, searchCondition.FormattableParameters);
var formattableStr = searchCondition.GetFormattableString(sql);

using var db = new TestContext();

var list = db.ExpressionSqlTests.FromSqlInterpolated(formattableStr);
foreach (var item in list)
{
    Console.WriteLine(item);
}
