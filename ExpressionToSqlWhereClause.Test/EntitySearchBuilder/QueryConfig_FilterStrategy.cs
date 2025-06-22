using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Infra.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_FilterStrategy
{
    [TestMethod]
    public void FilterStrategy_常规方式()
    {
        var input = new Model_FilterStrategyInput()
        {
            GetSum = 5,
            GetSumFilter = "="
        };

        var expression =
            default(Expression<Func<Model_FilterStrategy, bool>>)
            .WhereIf(true, a => a.IsDeleted == false)
            .WhereIfFilterStrategy(input.GetSum > 0 && input.GetSumFilter != null,
                                        a => a.GetSum, b => b.NumericFilterStrategy(input.GetSum, input.GetSumFilter))
            .WhereIf(true, a => a.Id > 0)
            ;

        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "IsDeleted = @IsDeleted And GetSum = @GetSum And Id > @Id");

        Assert.AreEqual(searchCondition.Parameters["@IsDeleted"], false);
        Assert.AreEqual(searchCondition.Parameters["@GetSum"], 5m);
        Assert.AreEqual(searchCondition.Parameters["@Id"], 0);
    }

    [TestMethod]
    public void FilterStrategy_结合配置()
    {
        var input = new Input_FilterStrategy()
        {
            Id = 0,
            GetSum = 5,
            GetSumFilter = ">"
        };

        QueryConfig<Input_FilterStrategy, Model_FilterStrategy> whereLambda = input.CreateQueryConfig(default(Model_FilterStrategy));
        whereLambda[SearchType.Gt] = new List<string>
        {
           nameof(Input_FilterStrategy.Id),
        };

        //注: QueryConfig 中的排序顺序 在这里不会被影响到.
        var expression =
            whereLambda.ToExpression()
            .WhereIf(true, a => a.IsDeleted == false)
            .WhereIfFilterStrategy(input.GetSum > 0 && input.GetSumFilter != null,
                                    a => a.GetSum, b => b.NumericFilterStrategy(input.GetSum, input.GetSumFilter));

        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id > @Id And IsDeleted = @IsDeleted And GetSum > @GetSum");

        Assert.AreEqual(searchCondition.Parameters["@Id"], 0);
        Assert.AreEqual(searchCondition.Parameters["@IsDeleted"], false);
        Assert.AreEqual(searchCondition.Parameters["@GetSum"], 5m);
    }
}