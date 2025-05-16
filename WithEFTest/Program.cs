using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.ExtensionMethods;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using WithEFTest.ExtensionMethods;

using var _db = new TestContext();


var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
    .WhereIf(true, a => a.Id == 1)
    .WhereIf(true, a => a.Name == "abc")
    .WhereIf(true, a => a.CreateAt > new DateTime(2023, 1, 1))
    .WhereIf(true, a => a.Flag.Value == Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81"));

{
    SearchCondition searchCondition = expression.ToWhereClause();
    string? clause_FormattableString = WhereClauseHelper.GetFormattableStringClause(searchCondition);
    string sql_str = $@"select * from ExpressionSqlTest where {clause_FormattableString}";
    var pms = searchCondition.Parameters.Values.ToArray();
    FormattableString? sql = WhereClauseHelper.CreateFormattableString(sql_str, pms);
    var list = _db.ExpressionSqlTests.FromSqlInterpolated(sql);
    foreach (var item in list)
    {
        Console.WriteLine(item);
    }
}


{
    //分页
    SearchCondition searchCondition = expression.ToWhereClause();
    var clause = searchCondition.WhereClause;
    var param = searchCondition.Parameters;
    var sql_count_str = $"select count(*) from V_CRM_Contacts where {clause}";

    //var total = _db.ExecSqlReader<int>(sql_count_str, searchCondition.GetSqlParameter()).First(); //自定义的扩展方法

    var pageIndex = 1;
    var pageSize = 5;

    var sql_query_str = $"select * from V_CRM_Contacts where {clause} Order By IsDeparture, ModifyDate desc " +
        $@" OFFSET {(pageIndex - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

    //var list = _db.ExecSqlReader<V_CRM_Contacts>(sql_query_str, searchCondition.GetSqlParameter()).ToList(); //自定义的扩展方法
}
