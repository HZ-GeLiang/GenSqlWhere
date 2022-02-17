using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EntityToSqlWhereClauseConfig.Helper;
using EntityToSqlWhereClauseConfig.Test.input;
using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
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
                DataCreatedAtStart = time.AddDays(-1), //Todo:如果这里是 Addhour,默认的时间进度将计算错误
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

            Assert.AreEqual(sql, "((((((Id In (@Id)) And (Sex In (@Sex))) And ((IsDel = @IsDel))) And ((DataCreatedAt >= @DataCreatedAt))) And ((DataCreatedAt < @DataCreatedAt1))) And (Url Like @Url))");


            var dict = new Dictionary<string, object>
            {
                { "@Id",   "1,2"},
                { "@Sex", searchModel.Sex },
                { "@IsDel", searchModel.IsDel },
                { "@DataCreatedAt", searchModel.DataCreatedAtStart },
                { "@DataCreatedAt1", searchModel.DataCreatedAtEnd?.AddDays(1) },
                { "@Url", $@"%{searchModel.Url}%" },
            };

            DictionaryAssert.AreEqual(param, dict);
        }

        [TestMethod]
        public void Use2()
        {
            var searchModel = new Input_Demo()
            {
                IsDel = true,
                Url = "123",
            };

            var whereLambda = new WhereLambda<People, Input_Demo>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.eq] = new List<string>
            {
                nameof(searchModel.IsDel),
                nameof(searchModel.Url),
            };

            Expression<Func<People, bool>> exp = whereLambda.ToExpression();
            (string sql, Dictionary<string, object> param) = exp.ToWhereClause();

            Assert.AreEqual(sql, "IsDel = @IsDel And Url = @Url");
            sql = WhereClauseHelper.ParamNameToNumber(sql);
            Assert.AreEqual(sql, "IsDel = @0 And Url = @1");

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

            Assert.AreEqual(sql, "((((((Id In (@Id)) And (Sex In (@Sex))) And ((IsDel = @IsDel))) And ((DataCreatedAt >= @DataCreatedAt))) And ((DataCreatedAt < @DataCreatedAt1))) And (Url Like @Url))");
        }

        [TestMethod]
        public void UseDynamic_Test2()
        {
            var searchModel = new
            {
                //IsDel = 0,
                Name = (string)null,
            };

            var whereLambda = searchModel.CrateWhereLambda((People p) => { });

            whereLambda[SearchType.like] = new List<string>
            {
                "balabala"
            };
            Assert.ThrowsException<ArgumentException>(() => whereLambda.ToExpression().ToWhereClause(), "类'<>f__AnonymousType3`1[[System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]'中不存在名为'balabala'的属性");

        }

        [TestMethod]
        public void UseExampleForEf()
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

            var listExp = whereLambda.ToExpressionListForEf(); //可以给ef用

        }

    }
}
