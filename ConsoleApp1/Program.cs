using ExpressionToSqlWhereClause.Helpers;
using System.Linq.Expressions;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<int?> list = new List<int?>() { null, 12, 3, 4 };
            foreach (dynamic item in list)
            {
                Console.WriteLine(item.GetType());
            }

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