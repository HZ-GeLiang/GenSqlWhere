using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.Test.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause
{
    internal class Student
    {
        public string Name { get; set; }

        public int IsDeleted1 { get; set; }
        public int? IsDeleted2 { get; set; }
        public bool IsDeleted3 { get; set; }
        public bool? IsDeleted4 { get; set; }
        public short IsDeleted5 { get; set; }
        public short? IsDeleted6 { get; set; }
    }

    [TestClass]
    public class WhereLambdaHelperTest
    {
        [TestMethod]
        public void GetExpression_HasValue()
        {
            Expression<Func<Student, bool>> exp = a => (a.Name ?? "") != "";
            var exp2 = WhereLambdaHelper.GetExpression_HasValue<Student>("Name");

            Assert.AreEqual(true, exp.ToString() == exp2.ToString());

            //配合扩展方法的使用
            {
                IQueryable<Student> query = null;
                query?.WhereNoValue(a => a.Name);
            }
        }

        [TestMethod]
        public void GetExpression_NotDeleted()
        {
        }
    }
}