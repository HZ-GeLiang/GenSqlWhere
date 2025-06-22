using ExpressionToSqlWhereClause.ExtensionMethods;
using Infra.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause;

[TestClass]
public class IssusDemo
{
    class TypeConvert
    {
        public byte Status { get; set; } = 0;
    }

    class TypeConvert_input
    {
        public int? Status { get; set; } = 0;
    }

    [TestMethod]
    public void 类型问题()
    {
        {
            byte a1 = 1;
            int? a2 = 1;
            Console.WriteLine(a1 == a2); //true
            Console.WriteLine(a2 == a1); //true
        }

        var input = new TypeConvert_input() { Status = 1 };

        var expression =
            default(Expression<Func<TypeConvert, bool>>)
            .WhereIf(input.Status.HasValue, a => a.Status == input.Status);

        var searchCondition = expression.ToWhereClause(); //注:sql参数的类型还是 int 的, 并没有转换为 byte
        Assert.AreEqual(searchCondition.WhereClause, "Status = @Status");
    }
}