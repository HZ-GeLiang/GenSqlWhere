using ExpressionToSqlWhereClause.Helpers;
using System.Linq.Expressions;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Expression<Func<peo, bool>> exp = a => (a.Name ?? "") != "";
            var exp2 = QueryConfigHelper.GetExpression_HasValue<peo>("Name");

            Console.WriteLine(exp.ToString() == exp2.ToString());

            Console.WriteLine("Hello, World!");
        }
    }

    internal class peo
    {
        public string Name { get; set; }
    }
}