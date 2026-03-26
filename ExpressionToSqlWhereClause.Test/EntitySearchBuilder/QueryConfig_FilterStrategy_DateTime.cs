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


}