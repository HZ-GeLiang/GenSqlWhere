using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.Test.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.WhereLambdaConfigToWhereClause
{
    [TestClass]
    public class GenSqlWhereDemo_eq
    {
        [TestMethod]
        public void Test_eq()
        {
            var searchModel = new Model_eq()
            {
                IsDel = true,
            };

            var whereLambda = new WhereLambda<Input_eq, Model_eq>
            {
                Search = searchModel
            };

            whereLambda[SearchType.Eq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", searchModel.IsDel }
            };

            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }


        [TestMethod]
        public void Test_eq2()
        {
            var searchModel = new Model_eq2()
            {
                Id = 1
            };

            var expression = default(Expression<Func<Input_eq2, bool>>)
             .WhereIf(true, a => a.Id == searchModel.Id)
             ;
            var searchCondition = expression.ToWhereClause();

        }

        [TestMethod]
        public void Test_eq_WhereIf()
        {
            var searchModel = new InputModel_eq()
            {
                Id = 1
            };

            {
                var whereLambda = new WhereLambda<Model_eq, InputModel_eq>
                {
                    Search = searchModel
                };
                whereLambda.WhereIf[nameof(searchModel.Id)] = a => a.Id > 0;// ???????????? Id>0??? ????????? ,??????????????????

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
                var whereLambda = new WhereLambda<Model_eq, InputModel_eq>
                {
                    Search = searchModel
                };
                whereLambda.WhereIf[nameof(searchModel.Id)] = a => a.Id > 10;// ???????????? Id>0??? ????????? ,??????????????????

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

    }

    public class Input_eq
    {
        [SearchType(SearchType.Eq)] public bool IsDel { get; set; }
    }

    public class Input_eq2
    {
        [SearchType(SearchType.Eq)] public long? Id { get; set; }
    }


    public class Model_eq
    {
        public bool IsDel { get; set; }
        public int Id { get; set; }
    }


    public class InputModel_eq
    {
        public int Id { get; set; }
    }
    public class Model_eq2
    {
        public int Id { get; set; }
    }
}