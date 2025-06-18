using FilterStrategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause;

[TestClass]
public class GenSqlWhereDemo_FilterStrategy
{
    [TestMethod]
    public void Test_FilterStrategy()
    {
        var obj = new model_FilterStrategyInput()
        {
            GetSum = 5,
            GetSumFilter = "="
        };

        var expression =
            default(Expression<Func<model_FilterStrategy, bool>>)
            .WhereIf(true, a => a.IsDeleted == false)
            .WhereIfFilterStrategy(obj.GetSum > 0 && obj.GetSumFilter != null,
                                        a => a.GetSum, b => b.NumericFilterStrategy(obj.GetSum, obj.GetSumFilter))
            .WhereIf(true, a => a.Id > 0)
            ;

        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "IsDeleted = @IsDeleted And GetSum = @GetSum And Id > @Id");
        //var dict = new Dictionary<string, object>
        //{
        //    { "@IsDeleted", (object)false },
        //    { "@GetSum", (object)5},
        //    { "@Id",(object) 0},
        //};

        //CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    public class model_FilterStrategy
    {
        public bool IsDeleted { get; set; }
        public int Id { get; set; }
        public decimal GetSum { get; set; }
    }

    /// <summary>
    /// 获取收款input
    /// </summary>
    public class model_FilterStrategyInput
    {
        public decimal GetSum { get; set; }

        /// <summary>
        /// 筛选符号
        /// </summary>
        public string GetSumFilter { get; set; }
    }
}