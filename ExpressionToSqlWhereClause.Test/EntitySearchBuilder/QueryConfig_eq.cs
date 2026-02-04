using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Infra.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_eq
{
    [TestMethod]
    public void eq_checkvalue()
    {
        {
            var searchModel = new Input_eq()
            {
                IsDel = false,
            };
            var expression = default(Expression<Func<Input_eq, bool>>).Where(a => a.IsDel == searchModel.IsDel);

            SearchCondition searchCondition = expression.ToWhereClause();
            var clause = searchCondition.WhereClause;
            var sqlParams = searchCondition.Parameters;

            var value = Convert.ToBoolean(sqlParams["@IsDel"]);
            Assert.AreEqual(clause, "IsDel = @IsDel");
            Assert.AreEqual(value, false);
        }
        {
            var searchModel = new Input_eq()
            {
                IsDel = true,
            };
            var expression = default(Expression<Func<Input_eq, bool>>).Where(a => a.IsDel == searchModel.IsDel);

            SearchCondition searchCondition = expression.ToWhereClause();
            var clause = searchCondition.WhereClause;
            var sqlParams = searchCondition.Parameters;

            var value = Convert.ToBoolean(sqlParams["@IsDel"]);
            Assert.AreEqual(clause, "IsDel = @IsDel");
            Assert.AreEqual(value, true);
        }
    }

    [TestMethod]
    public void eq()
    {
        var searchModel = new Input_eq()
        {
            IsDel = true,
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_eq));

        whereLambda[SearchType.Eq] = new List<string>
        {
            nameof(searchModel.IsDel),
        };

        {
            //获得 expression 的 写法1
            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", searchModel.IsDel }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        {
            //获得 expression 的 写法2

            //var expression = whereLambda.ToExpression();
            var searchConditionDict = whereLambda.GetSearchConditionDict();

            //手动的删除某个配置
            //if (searchConditionDict.ContainsKey(SearchType.Eq))
            //{
            //    searchConditionDict[SearchType.Eq].Remove(nameof(input.Id));
            //}

            var whereLambdas = whereLambda.ToExpressionList(searchConditionDict);
            var expression = whereLambda.ToExpression(whereLambdas);

            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", searchModel.IsDel }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }

    [TestMethod]
    public void eq2()
    {
        var searchModel = new Model_eq2()
        {
            Id = 1
        };

        var expression = default(Expression<Func<Input_eq2, bool>>)
         .WhereIf(true, a => a.Id == searchModel.Id);

        var searchCondition = expression.ToWhereClause();
    }

    [TestMethod]
    public void eq_WhereIf()
    {
        var searchModel = new InputModel_eq()
        {
            Id = 1
        };

        {
            var whereLambda = searchModel.CreateQueryConfig(default(Model_eq));

            whereLambda.WhereIf[nameof(searchModel.Id)] = a => a.Id > 0;// 添加满足 Id>0的 条件时 ,当前值才生效

            whereLambda[SearchType.Eq] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Id = @Id");
            var dict = new Dictionary<string, object>
            {
                { "@Id", searchModel.Id }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }

        {
            var whereLambda = searchModel.CreateQueryConfig(default(Model_eq));

            //whereLambda.WhereIf[nameof(searchModel.Id)] = a => a.Id > 10;// 添加满足 Id>0的 条件时 ,当前值才生效

            //或者这样写
            Expression<Func<InputModel_eq, bool>> exp = a => a.Id > 10;
            whereLambda.WhereIf[nameof(searchModel.Id)] = exp;// 添加满足 Id>0的 条件时 ,当前值才生效

            whereLambda[SearchType.Eq] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "");
            var dict = new Dictionary<string, object>
            {
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }

    [TestMethod]
    public void eq_WhereIf_parseExpressionStr()
    {
        var searchModel = new InputModel_eq()
        {
            Id = 1
        };

        {
            var whereLambda = searchModel.CreateQueryConfig(default(Model_eq));

            //或者这样写
            //Expression<Func<InputModel_eq, bool>> exp = a => a.Id > 10;
            //whereLambda.WhereIf[nameof(searchModel.Id)] = exp;// 添加满足 Id>0的 条件时 ,当前值才生效

            //在或者这样写

            string expressionStr = "Id > 0";
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(typeof(InputModel_eq), typeof(bool), expressionStr);
            //string expressionStr = "Id > 0 && Age>0";//如果有多个表达式, 可以这样写
            //LambdaExpression lambda = DynamicExpressionParser.ParseLambda(typeof(InputModel_eq), typeof(bool), expressionStr, new object[] { }); //这个也可以
            whereLambda.WhereIf[nameof(searchModel.Id)] = (Expression<Func<InputModel_eq, bool>>)lambda;

            //c#将字符串 a => a.Id > 10 && a.Name="A"  解析为表达式树, 使用 System.Linq.Dynamic.Core

            whereLambda[SearchType.Eq] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Id = @Id");
            var dict = new Dictionary<string, object>
            {
                { "@Id", searchModel.Id }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }
}