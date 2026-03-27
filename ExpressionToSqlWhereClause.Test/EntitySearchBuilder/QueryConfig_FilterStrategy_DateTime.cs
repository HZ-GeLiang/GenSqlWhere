using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Infra.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_FilterStrategy_DateTime
{
    [TestMethod]
    public void DateTimeFilterStrategy()
    {
        var time = new DateTime(2026, 1, 1, 10, 10, 10);
        var input = new Model_FilterStrategyInput() { CreateDate = time };

        {
            input.CreateDateFilter = "=";
            var searchCondition = GetSearchCondition(input);
            Assert.AreEqual(searchCondition.WhereClause, "IsDeleted = @IsDeleted And CreateDate = @CreateDate And Id > @Id");
            Assert.AreEqual(searchCondition.Parameters["@IsDeleted"], false);
            Assert.AreEqual(searchCondition.Parameters["@CreateDate"], time);
            Assert.AreEqual(searchCondition.Parameters["@Id"], 0);
        }

        {
            input.CreateDateFilter = "<>";
            var searchCondition = GetSearchCondition(input);
            Assert.AreEqual(searchCondition.WhereClause, "IsDeleted = @IsDeleted And CreateDate <> @CreateDate And Id > @Id");
            Assert.AreEqual(searchCondition.Parameters["@IsDeleted"], false);
            Assert.AreEqual(searchCondition.Parameters["@CreateDate"], time);
            Assert.AreEqual(searchCondition.Parameters["@Id"], 0);
        }

        {
            input.CreateDateFilter = ">";
            var searchCondition = GetSearchCondition(input);
            Assert.AreEqual(searchCondition.WhereClause, "IsDeleted = @IsDeleted And CreateDate > @CreateDate And Id > @Id");
            Assert.AreEqual(searchCondition.Parameters["@IsDeleted"], false);
            Assert.AreEqual(searchCondition.Parameters["@CreateDate"], time);
            Assert.AreEqual(searchCondition.Parameters["@Id"], 0);
        }

        {
            input.CreateDateFilter = ">=";
            var searchCondition = GetSearchCondition(input);
            Assert.AreEqual(searchCondition.WhereClause, "IsDeleted = @IsDeleted And CreateDate >= @CreateDate And Id > @Id");
            Assert.AreEqual(searchCondition.Parameters["@IsDeleted"], false);
            Assert.AreEqual(searchCondition.Parameters["@CreateDate"], time);
            Assert.AreEqual(searchCondition.Parameters["@Id"], 0);
        }


        {
            input.CreateDateFilter = "<";
            var searchCondition = GetSearchCondition(input);
            Assert.AreEqual(searchCondition.WhereClause, "IsDeleted = @IsDeleted And CreateDate < @CreateDate And Id > @Id");
            Assert.AreEqual(searchCondition.Parameters["@IsDeleted"], false);
            Assert.AreEqual(searchCondition.Parameters["@CreateDate"], time);
            Assert.AreEqual(searchCondition.Parameters["@Id"], 0);
        }

        {
            input.CreateDateFilter = "<=";
            var searchCondition = GetSearchCondition(input);
            Assert.AreEqual(searchCondition.WhereClause, "IsDeleted = @IsDeleted And CreateDate <= @CreateDate And Id > @Id");
            Assert.AreEqual(searchCondition.Parameters["@IsDeleted"], false);
            Assert.AreEqual(searchCondition.Parameters["@CreateDate"], time);
            Assert.AreEqual(searchCondition.Parameters["@Id"], 0);
        }

        {
            input.CreateDateFilter = "between";
            input.CreateDate2 = time.AddHours(1);
            var searchCondition = GetSearchCondition(input);
            Assert.AreEqual(searchCondition.WhereClause, "IsDeleted = @IsDeleted And CreateDate >= @CreateDate And CreateDate <= @CreateDate1 And Id > @Id");
            Assert.AreEqual(searchCondition.Parameters["@IsDeleted"], false);
            Assert.AreEqual(searchCondition.Parameters["@CreateDate"], time);
            Assert.AreEqual(searchCondition.Parameters["@CreateDate1"], input.CreateDate2);
            Assert.AreEqual(searchCondition.Parameters["@Id"], 0);
        }

    }

    private SearchCondition GetSearchCondition(Model_FilterStrategyInput input)
    {
        Expression<Func<Model_FilterStrategy, bool>> expression =
           default(Expression<Func<Model_FilterStrategy, bool>>)
           .WhereIf(true, a => a.IsDeleted == false)
           .WhereIfFilterStrategy(
               input.CreateDate != null && input.CreateDateFilter != null && input.CreateDate2 == null,
                a => a.CreateDate, b => b.DateTimeFilterStrategy(input.CreateDate, input.CreateDateFilter))
           .WhereIfFilterStrategy(
               input.CreateDate != null && input.CreateDateFilter != null && input.CreateDate2 != null,
                a => a.CreateDate, b => b.DateTimeFilterStrategy(input.CreateDate, input.CreateDateFilter, input.CreateDate2))
           .WhereIf(true, a => a.Id > 0)
           ;

        SearchCondition searchCondition = expression.ToWhereClause();
        return searchCondition;

    }

    [TestMethod]
    public void DateTimeFilterStrategy_EF示例()
    {

        /*
         * 1. 给视图对象配置时间精度(若类型为DateTime)
 public class v_User_Config : IEntityTypeConfiguration<v_User_Account>
 {
     public void Configure(EntityTypeBuilder<v_User_Account> builder)
     {
         builder.Property(x => x.CreateDate).HasPrecision(3);
     }
 }

 */

        /*
         * 2.设置日期的Kind属性 为Unspecified
            Kind        EF 处理
            Local       转成 datetimeoffset(+08:00)    '2026-03-21T00:00:00.0000000+08:00'
            Utc         转成 +00:00                    '2026-03-21T00:00:00.0000000Z'
            Unspecified 当普通 datetime                '2026-03-21T00:00:00.0000000'
        */
        var fiveDayAgo = DateTime.Now.Date.AddDays(-1 * 5);
        fiveDayAgo = DateTime.SpecifyKind(fiveDayAgo, DateTimeKind.Unspecified);
        var daysFilter = "=";
        var exp = default(Expression<Func<Model_FilterStrategy, bool>>)
            .DateTimeFilterStrategy(fiveDayAgo, daysFilter)
            .ApplyFilter(a => a.CreateDate);

        //var query = query.WhereIf(exp != null, exp);

    }

}