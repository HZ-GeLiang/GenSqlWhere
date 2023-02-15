using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WithEFTest.ExtensionMethod;
using ExpressionToSqlWhereClause.Helper;
using System.Runtime.CompilerServices;
using ExpressionToSqlWhereClause.ExtensionMethod;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

//var s1 = "SELECT * FROM table1 WHERE (col1 = 'value1') AND ((col2 = 'value2') OR (col3 = 'value3'))";
//var new_s1 = "";
//new_s1 = SqlTrim.RemoveRedundantParentheses(s1);
//Console.WriteLine(new_s1);
//var s1_res = "SELECT * FROM table1 WHERE col1 = 'value1' AND (col2 = 'value2' OR col3 = 'value3')";
//Console.WriteLine(s1_res);

var v3 = new DateTime(2023, 1, 1);
var v4 = Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81");
var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
    .WhereIf(true, a => a.Id == 1)
    .WhereIf(true, a => a.Name == "abc")
    //.WhereIf(true, a => a.CreateAt > new DateTime(2023, 1, 1))
    //.WhereIf(true, a => a.Flag.Value == Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81"))
    .WhereIf(true, a => a.CreateAt > v3)
    .WhereIf(true, a => a.Flag.Value == v4)
    //.WhereIf(true, a => a.Flag == v4)
    ;

var where = WhereClauseHelper.ToFormattableString(expression.ToWhereClause());

string sql = $@"select * from ExpressionSqlTest where {where.whereClause} ";

var fmbsql = FormattableStringFactory.Create(sql, where.parameters);


using var db = new TestContext();

var list = db.ExpressionSqlTests.FromSqlInterpolated(fmbsql);
foreach (var item in list)
{
    Console.WriteLine(item);
}


public class SqlTrim
{
    public static string RemoveRedundantParentheses(string sql)
    {
        return "";
    }

}