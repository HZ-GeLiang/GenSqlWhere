using Infra.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class GenSqlWhereDemo_Nulable
{
    [TestMethod]
    public void nullable_datetime()
    {
        /*
         bug:#01
         .Value的访问会有错误
         System.NullReferenceException:“Object reference not set to an instance of an object.”
         */
        var obj = new inputDto
        {
            ModifyDate = new DateTime(2020, 1, 1)
        };

        var expression =
            default(Expression<Func<v_User, bool>>)
            .WhereIf(obj.ModifyDate.HasValue, a => a.ModifyDate >= obj.ModifyDate.Value.Date && a.ModifyDate < obj.ModifyDate.Value.Date.AddDays(1));

        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "ModifyDate >= @ModifyDate And ModifyDate < @ModifyDate1");
        var dict = new Dictionary<string, object>
        {
            { "@ModifyDate",  new DateTime(2020, 1, 1) },
            { "@ModifyDate1",  new DateTime(2020, 1, 2) },
        };
        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    #region domain

    public class v_User
    {
        public DateTime? ModifyDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class inputDto
    {
        public DateTime? ModifyDate { get; set; }
    }

    #endregion
}