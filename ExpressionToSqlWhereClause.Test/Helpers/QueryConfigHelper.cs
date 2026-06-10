using ExpressionToSqlWhereClause.Helpers;

namespace ExpressionToSqlWhereClause.Test.Helpers
{
    [TestClass]
    public class QueryConfigHelperTest
    {
        [TestMethod]
        public void GetExpression_HasValue()
        {
            Expression<Func<peo, bool>> exp = a => (a.Name ?? "") != "";
            var exp2 = QueryConfigHelper.GetExpression_HasValue<peo>("Name");
            Assert.AreEqual(true, exp.ToString() == exp2.ToString());
        }

        internal class peo
        {
            public string Name { get; set; }
        }
    }
}