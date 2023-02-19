using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.Helper;
using ExpressionToSqlWhereClause.Test.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause
{

    [TestClass]
    public class ExpressionDemo
    {

        [TestMethod]
        public void 获得sql_不使用sql参数()
        {
            var input = new ShopInfoInput()
            {
                Production = "aa",
            };

            var expression = default(Expression<Func<ShopInfoInput, bool>>)
               .WhereIf(!string.IsNullOrEmpty(input.Production), a => a.Production.Contains(input.Production))
            ;

            var searchCondition = expression.ToWhereClause();
            WhereClauseHelper.MergeParametersIntoSql(searchCondition);
            Assert.AreEqual(searchCondition.WhereClause, "Production Like '%aa%'");




        }

        [TestMethod]
        public void 获得sql_不使用sql参数_使用EF的直接查询()
        {
            var input = new ShopInfoInput()
            {
                Production = "aa",
            };

            var expression = default(Expression<Func<ShopInfoInput, bool>>)
               .WhereIf(!string.IsNullOrEmpty(input.Production), a => a.Production.Contains(input.Production))
            ;

            var searchCondition = expression.ToWhereClause();
            WhereClauseHelper.ToFormattableString(searchCondition);
            Assert.AreEqual(searchCondition.WhereClause, "Production Like {0}");


            /*一个示例


                var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
                    .WhereIf(true, a => a.Id == 1)
                    .WhereIf(true, a => a.Name == "abc")
                    .WhereIf(true, a => a.CreateAt > new DateTime(2023, 1, 1))
                    .WhereIf(true, a => a.Flag.Value == Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81"))
                    ;

                var searchCondition = expression.ToFormattableWhereClause();

                string sql = $@"select * from ExpressionSqlTest where {searchCondition.WhereClause} ";

                //var formattableStr = FormattableStringFactory.Create(sql, searchCondition.FormattableParameters);
                var formattableStr = searchCondition.GetFormattableString(sql);

                using var db = new TestContext();

                var list = db.ExpressionSqlTests.FromSqlInterpolated(formattableStr);
                foreach (var item in list)
                {
                    Console.WriteLine(item);
                }

            */
        }

        [TestMethod]
        public void boolean值的写法1()
        {
            Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel; });
            var searchCondition = expOr.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "(Id = @Id Or Id = @Id1) And IsDel = @IsDel");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(searchCondition.Parameters, para);
        }

        [TestMethod]
        public void boolean值的写法2()
        {
            Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; });
            //                    和  () => { return x => x.IsDel; } 不一样

            var searchCondition = expOr.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "(Id = @Id Or Id = @Id1) And IsDel = @IsDel");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(searchCondition.Parameters, para);


            //这个是   WhereClauseHelper.ConvertParameters 使用示例
            var pms = WhereClauseHelper.ConvertParameters(searchCondition.Parameters, (key, val) =>
            {
                return new { ParameterName = key, Value = val };
            });
        }

        [TestMethod]
        public void ValidateString()
        {
            Expression<Func<User, bool>> expression = u => u.Name != "aa";
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "aa");
            Assert.AreEqual("Name <> @Name", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateBool()
        {
            Expression<Func<User, bool>> expression = u => !u.Sex;

            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", false);
            Assert.AreEqual("Sex <> @Sex", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateBool2()
        {
            Expression<Func<User, bool>> expression = u => u.Sex && u.Name.StartsWith("Foo");
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Name", "Foo%");
            Assert.AreEqual("Sex = @Sex And Name Like @Name", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);

        }

        [TestMethod]
        public void ValidateBool3()
        {
            Expression<Func<User, bool>> expression = u => !(u.Sex == false);

            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            Assert.AreEqual("Sex <> @Sex", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateBool4()
        {
            Expression<Func<User, bool>> expression = u => !(u.Sex == true);
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", false);
            Assert.AreEqual("Sex <> @Sex", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }


        [TestMethod]
        public void ValidateConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age >= 20;
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age >= @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateVariable()
        {
            int age = 20;
            Expression<Func<User, bool>> expression = u => u.Age >= age;
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age >= @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateAnd()
        {
            Expression<Func<User, bool>> expression = u => u.Sex && u.Age > 20;

            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Sex = @Sex And Age > @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateOr()
        {
            Expression<Func<User, bool>> expression = u => u.Sex || u.Age > 20;

            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Sex = @Sex Or Age > @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateDuplicateField()
        {
            Expression<Func<User, bool>> expression = u => u.Age < 15 || u.Age > 20;

            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 15);
            expectedParameters.Add("@Age1", 20);
            Assert.AreEqual("Age < @Age Or Age > @Age1", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateMemberConstant()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Age = 20;
            Expression<Func<User, bool>> expression = u => u.Age < userFilter.Age;
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }


        [TestMethod]
        public void ValidateDeepMemberConstant()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Internal.Age = 20;
            Expression<Func<User, bool>> expression = u => u.Age < userFilter.Internal.Age;
            var searchCondition = expression.ToWhereClause(sqlAdapter: new ToLowerSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@age", 20);
            Assert.AreEqual("age < @age", searchCondition.WhereClause);//因为 ToLowerSqlAdapter
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateInstanceMethodConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < GetInt();
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateStaticMethodConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < UserFilter.GetInt(20);
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateMethodConstant2()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Internal.Age = 20;
            Expression<Func<User, bool>> expression = u => u.Age < GetInt(userFilter.Internal.Age);
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateMethodChainConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < int.Parse(GetInt().ToString());
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateEqualMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.Equals(filter.Name.Substring(1));
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "ame");
            Assert.AreEqual("Name = @Name", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateTernary()
        {
            string name = "Gary";
            Expression<Func<User, bool>> expression = u => u.Name == (name == null ? "Foo" : name);
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Gary");
            Assert.AreEqual("Name = @Name", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateNotEqual()
        {
            Expression<Func<User, bool>> expression = u => u.Age != 20;
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age <> @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateStartsWithMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.StartsWith(filter.Name);
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Name%");
            Assert.AreEqual("Name Like @Name", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateStartWith2()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.StartsWith(filter.Name) || u.Name.Contains("Start");
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Name%");
            expectedParameters.Add("@Name1", "%Start%");
            Assert.AreEqual("Name Like @Name Or Name Like @Name1", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateEndsWithMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.EndsWith(filter.Name);
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "%Name");
            Assert.AreEqual("Name Like @Name", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateContainsMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.Contains(filter.Name);
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "%Name%");
            Assert.AreEqual("Name Like @Name", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        //一元
        public void ValidateUnary()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
            {
                Expression<Func<User, bool>> expression = u => !u.Name.Contains("Name");
                var sql = expression.ToWhereClause(null, new TestSqlAdapter());
            });

        }


        [TestMethod]
        public void case_复杂版的表达式解析1()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;
            filter.Name = "Gary";

            Expression<Func<User, bool>> expression =
            //  (========left======) || (==================right====================)
            u => u.Sex && u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age;

            var where = expression.ToWhereClause();

            Assert.AreEqual("(Sex = @Sex And Age > @Age) Or (Sex = @Sex1 And Age > @Age1)", where.WhereClause);
        }

        [TestMethod]
        public void case_复杂版的表达式解析2()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;
            filter.Name = "Gary";
            Expression<Func<User, bool>> expression =
            //  (==left) && (==================================right====================)
            //              (==left==)  ||(====================right====================)
            u => u.Sex && (u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age);

            var where = expression.ToWhereClause();

            Assert.AreEqual("Sex = @Sex And (Age > @Age Or (Sex = @Sex1 And Age > @Age1))", where.WhereClause);
        }


        [TestMethod]
        public void case_复杂版的表达式解析3()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;
            filter.Name = "Gary";

            Expression<Func<User, bool>> expression =
                u => u.Age > 10
                      && (u.Sex && u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age)
                      && (u.Name == filter.Name || u.Name.Contains(filter.Name.Substring(1, 2)))
                      ;
            var where = expression.ToWhereClause();

            Assert.AreEqual("Age > @Age And ((Sex = @Sex And Age > @Age1) Or (Sex = @Sex1 And Age > @Age2)) And (Name = @Name Or Name Like @Name1)", where.WhereClause);
        }

        //[TestMethod]
        public void case_复杂版的表达式解析4()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;
            filter.Name = "Gary";

            Expression<Func<User, bool>> expression =
                u => u.Age > 10
                      || (u.Sex && u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age)
                      || (u.Name == filter.Name || u.Name.Contains(filter.Name.Substring(1, 2)))
                      ;
            var where = expression.ToWhereClause();

            Assert.AreEqual("Age > @Age Or ((Sex = @Sex And Age > @Age1) Or (Sex = @Sex1 And Age > @Age2)) Or (Name = @Name Or Name Like @Name1)", where.WhereClause);
        }

        //[TestMethod]
        public void case_复杂版的表达式解析5()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;
            filter.Name = "Gary";

            Expression<Func<User, bool>> expression =
                u => (u.Age > 10)
                      || (
                            (u.Age > 10 && u.Age > 11)
                             && (u.Sex && u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age)
                             && (u.Name == filter.Name || u.Name.Contains(filter.Name.Substring(1, 2)))
                          )
                      ;
            var where = expression.ToWhereClause();

            Assert.AreEqual("太复杂的去掉()的成本太高, 不写了...", where.WhereClause);
        }


        [TestMethod]
        public void case_去掉括号_3个or()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;

            Expression<Func<User, bool>> expression =
                u => u.Age > 10
                      || u.Age > 11
                      || u.Age > 12
                      ;
            var where = expression.ToWhereClause();

            Assert.AreEqual("Age > @Age Or Age > @Age1 Or Age > @Age2", where.WhereClause);

        }


        [TestMethod]
        public void case_去掉括号_4个or()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;

            Expression<Func<User, bool>> expression =
                u => u.Age > 10
                      || u.Age > 11
                      || u.Age > 12
                      || u.Age > 13
                      ;
            var where = expression.ToWhereClause();

            Assert.AreEqual("Age > @Age Or Age > @Age1 Or Age > @Age2 Or Age > @Age3", where.WhereClause);
        }



        [TestMethod]
        public void ValidateAll()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;
            filter.Name = "Gary";
            Expression<Func<User, bool>> expression =
                u =>
                 (u.Sex && u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age)
                  && (u.Name == filter.Name || u.Name.Contains(filter.Name.Substring(1, 2)));
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 18);
            expectedParameters.Add("@Sex1", false);
            expectedParameters.Add("@Age1", 20);
            expectedParameters.Add("@Name", "Gary");
            expectedParameters.Add("@Name1", "%ar%");
            Assert.AreEqual("((Sex = @Sex And Age > @Age) Or (Sex = @Sex1 And Age > @Age1)) And (Name = @Name Or Name Like @Name1)", searchCondition.WhereClause);

            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }


        [TestMethod]
        public void ValidateArrayIn()
        {
            List<string> values = new List<string> { "a", "b" };
            Expression<Func<User, bool>> expression = u => values.Contains(u.Name);
            var searchCondition = expression.ToWhereClause(sqlAdapter: new TestSqlAdapter());
            Assert.AreEqual("Name In (@Name)", searchCondition.WhereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Name", "'a','b'" }
            };
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateEnumerableIn()
        {
            string[] values = new string[] { "a", "b" };
            Expression<Func<User, bool>> expression = u => values.Contains(u.Name);
            var searchCondition = expression.ToWhereClause(sqlAdapter: new TestSqlAdapter());
            Assert.AreEqual("Name In (@Name)", searchCondition.WhereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Name", "'a','b'" }
            };
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateEnum()
        {
            Expression<Func<User, bool>> expression = u => u.Sex2 == Sex.Female;
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Assert.AreEqual("Sex2 = @Sex2", searchCondition.WhereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Sex2", (int)Sex.Female }
            };
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void ValidateEnum2()
        {
            Sex[] sexes = new Sex[] { Sex.Female, Sex.Female };
            Expression<Func<User, bool>> expression = u => u.Sex2 == sexes[1];
            var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
            Assert.AreEqual("Sex2 = @Sex2", searchCondition.WhereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Sex2", (int)Sex.Female }
            };
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        private int GetInt() => 20;

        private int GetInt(int i) => i;

        [TestMethod]
        public void ValidateColumnAttr()
        {
            Expression<Func<User_columnAttr, bool>> expression = u => u.Age != 20;
            //优先级:方法参数的 alias > Column
            var searchCondition = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Age", 20 }
            };
            Assert.AreEqual("UserAge <> @Age", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void 测试范围是否有问题()
        {
            DateTime? d1 = new DateTime(2020, 1, 1);
            DateTime? d2 = new DateTime(2020, 1, 15);
            //                                             lambda     left       ||   right --我这里没有写()
            Expression<Func<UserCreateTime, bool>> expression = u => u.Age != 20 || u.CreateStart > d1 && u.CreateEnd < d2;

            var searchCondition = expression.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Age", 20 },
                { "@CreateStart", d1 },
                { "@CreateEnd", d2 },
            };
            Assert.AreEqual("Age <> @Age Or (CreateStart > @CreateStart And CreateEnd < @CreateEnd)", searchCondition.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }

        [TestMethod]
        public void 测试_表达式的生成_new一个DateTime()
        {
            var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
                .WhereIf(true, a => a.CreateAt > new DateTime(2023, 1, 1))
                ;
            var where = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@CreateAt", new DateTime(2023, 1, 1)}
            };

            Assert.AreEqual("CreateAt > @CreateAt", where.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, where.Parameters);
        }

        [TestMethod]
        public void 测试_表达式的生成_new一个Guid()
        {
            var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
                .WhereIf(true, a => a.Flag.Value == Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81"))
                ;

            var where = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Flag", Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81")}
            };

            Assert.AreEqual("Flag = @Flag", where.WhereClause);
            DictionaryAssert.AreEqual(expectedParameters, where.Parameters);

        }

        [TestMethod]
        public void 测试_解析nullable的sql()
        {
            var v4 = Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81");
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Flag", v4}
            };

            {
                var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
                    .WhereIf(true, a => a.Flag.Value == v4)
                    ;
                var where = expression.ToWhereClause();

                Assert.AreEqual("Flag = @Flag", where.WhereClause);
                DictionaryAssert.AreEqual(expectedParameters, where.Parameters);
            }
            {
                var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
                    .WhereIf(true, a => a.Flag == v4)
                    ;
                var where = expression.ToWhereClause();

                Assert.AreEqual("Flag = @Flag", where.WhereClause);
                DictionaryAssert.AreEqual(expectedParameters, where.Parameters);
            }
        }

    }


    public record class ExpressionSqlTest
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreateAt { get; set; }
        public Guid? Flag { get; set; }
    }


    public class UserFilter
    {
        public string Name { get; set; }

        public bool Sex { get; set; }

        public int Age { get; set; }

        public Internal Internal { get; set; } = new Internal();

        public static int GetInt(int i)
        {
            return i;
        }
    }
    public class Internal
    {
        public int Age { get; set; }
    }

    public class UserCreateTime
    {
        public int Age { get; set; }
        public DateTime? CreateStart { get; set; }
        public DateTime? CreateEnd { get; set; }

    }
    public class User_columnAttr
    {
        [Column("UserAge")]
        public int Age { get; set; }
    }
    public class User
    {
        public string Name { get; set; }

        public bool Sex { get; set; }

        public Sex Sex2 { get; set; }

        public int Age { get; set; }
    }
    public class Student
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public sbyte Sex { get; set; }
        public bool IsDel { get; set; }
        public string DataRemark { get; set; }
        public DateTime DataCreatedAt { get; set; }
        public DateTime DataUpdatedAt { get; set; }

    }

    public class Student_Alias
    {
        public int Id { get; set; }
    }

    public class Student_mssql_buildIn_name
    {
        public int Index { get; set; }
    }
    public enum Sex
    {
        Male = 1,
        Female = 2
    }

    public class ShopInfoInput
    {

        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        internal DateTime? Date { get; set; } //冗余的

        public string Production { get; set; }

        public int? Unit { get; set; }
        public string Unit_Name { get; set; }

        public string Remarks { get; set; }
        public long? CreateUserID { get; set; }


    }
}
