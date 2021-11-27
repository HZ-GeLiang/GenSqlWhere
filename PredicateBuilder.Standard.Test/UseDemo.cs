using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressionToWhereClause.Standard;

namespace PredicateBuilder.Standard.Test
{
    [TestClass]
    public class UseDemo
    {
        [TestMethod]
        public void Use()
        {
            var time = System.DateTime.Parse("2021-8-8");
            var searchModel = new Input_Demo()
            {
                //Id = 1,
                Id = "1,2",
                Sex = "1",
                IsDel = true,
                Url = "123",
                DataCreatedAtStart = time.AddHours(-1),
                DataCreatedAtEnd = time,
            };

            var whereLambda = new WhereLambda<People, Input_Demo>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.like] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };

            whereLambda[SearchType.eq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };
            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.Sex),
            };

            whereLambda[SearchType.datetimeRange] = new List<string>
            {
                nameof(searchModel.DataCreatedAtStart),
                nameof(searchModel.DataCreatedAtEnd),
                nameof(searchModel.DataUpdatedAtStart),
                nameof(searchModel.DataUpdatedAtEnd),
            };

            whereLambda[SearchType.numberRange] = new List<string>
            {
            };

            //List<Expression<Func<Route, bool>>> listExp = whereLambda.ToExpressionList(); //可以给ef用
            Expression<Func<People, bool>> exp = whereLambda.ToExpression();
            //等价
            //List<Expression<Func<Route, bool>>> listExp = whereLambda;
            //Expression<Func<Route, bool>> exp = whereLambda;

            (string sql, Dictionary<string, object> param) = exp.ToWhereClause();

            Assert.AreEqual(sql, "Id In @Id And Sex In @Sex And IsDel = @IsDel And DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1 And Url Like @Url");
        }

        [TestMethod]
        public void UseDynamic()
        {
            var searchModel = new
            {
                //Id = 1,
                Id = "1,2",
                Sex = "1",
                IsDel = true,
                Url = "123",
                DataCreatedAtStart = DateTime.Parse("2021-8-8").AddHours(-1),
                DataCreatedAtEnd = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = searchModel.CrateWhereLambda((People p) => { });

            whereLambda[SearchType.like] = new List<string>
            {
                nameof(searchModel.Url),
            };

            whereLambda[SearchType.eq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };
            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.Sex),
            };

            whereLambda[SearchType.datetimeRange] = new List<string>
            {
                nameof(searchModel.DataCreatedAtStart),
                nameof(searchModel.DataCreatedAtEnd),
            };

            (string sql, Dictionary<string, object> param) = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(sql, "Id In @Id And Sex In @Sex And IsDel = @IsDel And DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1 And Url Like @Url");
        }
    }
}
