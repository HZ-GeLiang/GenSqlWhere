using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.Test.ExtensionMethod;
using ExpressionToSqlWhereClause.Test.InputOrModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test
{


    [TestClass]
    public class ExpressionTest
    {
        [TestMethod]
        public void Test_string为null值()
        {
            //暂不支持翻译为 isnull
            Expression<Func<Student, bool>> expOr = a => a.Url == null;
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause();
            Assert.AreEqual(WhereClause, "(Url is null)");

            var para = new Dictionary<string, object>()
            {

            };
            CollectionAssert.AreEqual(Parameters, para);
        }


        [TestMethod]
        public void Test_boolean值的写法1()
        {
            Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel; });
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause();


            Assert.AreEqual(WhereClause, "((((Id = @Id)) Or ((Id = @Id1))) And (IsDel = @IsDel))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(Parameters, para);
        }

        [TestMethod]
        public void Test_boolean值的写法2()
        {
            Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; }); // 和  () => { return x => x.IsDel; } 不一样

            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause();

            Assert.AreEqual(WhereClause, "((((Id = @Id)) Or ((Id = @Id1))) And ((IsDel = @IsDel)))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(Parameters, para);

        }

        [TestMethod]
        public void 测试别名()
        {
            Expression<Func<Student_Alias, bool>> expOr = a => a.Id == 1 || a.Id == 2;

            var dict = new Dictionary<string, string>()
            {
                { "Id", "RouteId" }
            };
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause(alias: dict);

            Assert.AreEqual(WhereClause, "(((RouteId = @Id)) Or ((RouteId = @Id1)))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
            };
            CollectionAssert.AreEqual(Parameters, para);
        }

        [TestMethod]
        public void 测试别名2()
        {
            Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; }); // 和  () => { return x => x.IsDel; } 不一样

            var dict = new Dictionary<string, string>()
            {
                { "Id", "RouteId" }
            };
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause(alias: dict);

            Assert.AreEqual(WhereClause, "((((RouteId = @Id)) Or ((RouteId = @Id1))) And ((IsDel = @IsDel)))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(Parameters, para);
        }

        [TestMethod]
        public void 测试别名3()
        {
            Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; }); // 和  () => { return x => x.IsDel; } 不一样

            var dict = new Dictionary<string, string>()
            {
                { "Id", "RouteId" },
                { "IsDel", "b.IsDel" }
            };
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause(alias: dict);

            Assert.AreEqual(WhereClause, "((((RouteId = @Id)) Or ((RouteId = @Id1))) And ((b.IsDel = @IsDel)))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(Parameters, para);
        }

        [TestMethod]
        public void ValidateString()
        {
            Expression<Func<User, bool>> expression = u => u.Name != "aa";

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "aa");
            //Assert.AreEqual("(Name <> @Name)", sql);
            Assert.AreEqual("Name <> @Name", sql);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }


        [TestMethod]
        public void ValidateBool()
        {
            Expression<Func<User, bool>> expression = u => !u.Sex;

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", false);
            //Assert.AreEqual("(Sex != @Sex)", sql);
            Assert.AreEqual("Sex <> @Sex", sql);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }


        [TestMethod]
        public void ValidateBool3()
        {
            Expression<Func<User, bool>> expression = u => !(u.Sex == false);

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            //Assert.AreEqual("(Sex != @Sex)", sql);
            Assert.AreEqual("Sex <> @Sex", sql);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateBool4()
        {
            Expression<Func<User, bool>> expression = u => !(u.Sex == true);

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", false);
            //Assert.AreEqual("(Sex != @Sex)", sql);
            Assert.AreEqual("Sex <> @Sex", sql);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateBool2()
        {
            Expression<Func<User, bool>> expression = u => u.Sex && u.Name.StartsWith("Foo");
            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Name", "Foo%");
            //Assert.AreEqual("((Sex = @Sex) And (Name Like @Name))", sql);
            Assert.AreEqual("Sex = @Sex And Name Like @Name", sql);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);

        }

        [TestMethod]
        public void ValidateConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age >= 20;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            //Assert.AreEqual("((Age >= @Age))", whereClause);
            Assert.AreEqual("Age >= @Age", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateVariable()
        {
            int age = 20;
            Expression<Func<User, bool>> expression = u => u.Age >= age;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age >= @Age", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateAnd()
        {
            Expression<Func<User, bool>> expression = u => u.Sex && u.Age > 20;

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Sex = @Sex And Age > @Age", sql);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateOr()
        {
            Expression<Func<User, bool>> expression = u => u.Sex || u.Age > 20;

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("((Sex = @Sex) Or ((Age > @Age)))", sql);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateDuplicateField()
        {
            Expression<Func<User, bool>> expression = u => u.Age < 15 || u.Age > 20;

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 15);
            expectedParameters.Add("@Age1", 20);
            Assert.AreEqual("(((Age < @Age)) Or ((Age > @Age1)))", sql);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateMemberConstant()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Age = 20;
            Expression<Func<User, bool>> expression = u => u.Age < userFilter.Age;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            //Assert.AreEqual("((Age < @Age))", whereClause);
            Assert.AreEqual("Age < @Age", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }


        [TestMethod]
        public void ValidateDeepMemberConstant()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Internal.Age = 20;
            Expression<Func<User, bool>> expression = u => u.Age < userFilter.Internal.Age;
            //(string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(sqlAdapter: new ToLowerSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            //Assert.AreEqual("((Age < @Age))", whereClause);
            Assert.AreEqual("age < @Age", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateInstanceMethodConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < GetInt();
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            //Assert.AreEqual("((Age < @Age))", whereClause);
            Assert.AreEqual("Age < @Age", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateStaticMethodConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < UserFilter.GetInt(20);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            //Assert.AreEqual("((Age < @Age))", whereClause);
            Assert.AreEqual("Age < @Age", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }


        [TestMethod]
        public void ValidateMethodConstant2_SqlFunc2()
        {
            //EntityToSqlWherecClauseConfig 的 in
            var userFilter = new User_SqlFunc2() { CreateAtMonth = new List<int> { 5, 6 } };
            Expression<Func<User_SqlFunc_Entity, bool>> expression =
                u => (SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth[0] ||
                SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth[1]
                ); //如果有多个条件, 这个()不能去掉
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Month", 5);
            expectedParameters.Add("@Month1", 6);
            //Assert.AreEqual("((Age < @Age))", whereClause);
            Assert.AreEqual("(((Month(CreateAt) = @Month)) Or ((Month(CreateAt) = @Month1)))", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        public void Test_Expression_compile()
        {
            Expression<Func<User_SqlFunc_Entity, bool>> expression =
                u => SqlFunc.DbFunctions.Month(u.CreateAt) == 5;
        }

        [TestMethod]
        public void ValidateMethodConstant2_SqlFunc()
        {
            {
                var userFilter = new User_SqlFunc() { Month = 1, CreateAtMonth = 5 };
                Expression<Func<User_SqlFunc_Entity, bool>> expression =
                    u => SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth &&
                         u.Month == userFilter.Month;
                (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
                Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
                expectedParameters.Add("@Month", 5);
                expectedParameters.Add("@Month1", 1);
                Assert.AreEqual("Month(CreateAt) = @Month And Month = @Month1", whereClause);
                DictionaryAssert.AssertParameters(expectedParameters, parameters);
            }
            {
                var userFilter = new User_SqlFunc() { Month = 1, CreateAtMonth = 5 };
                Expression<Func<User_SqlFunc_Entity, bool>> expression =
                    u => u.Month == userFilter.Month &&
                         SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth;
                (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
                Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
                expectedParameters.Add("@Month", 1);
                expectedParameters.Add("@Month1", 5);
                Assert.AreEqual("Month = @Month And Month(CreateAt) = @Month1", whereClause);
                DictionaryAssert.AssertParameters(expectedParameters, parameters);
            }

            {
                var userFilter = new User_SqlFunc() { CreateAtMonth = 5, DelAtMonth = 6 };
                Expression<Func<User_SqlFunc_Entity, bool>> expression =
                    u => SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth &&
                         SqlFunc.DbFunctions.Month(u.DelAt) == userFilter.DelAtMonth;
                (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
                Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
                expectedParameters.Add("@Month", 5);
                expectedParameters.Add("@Month1", 6);
                Assert.AreEqual("Month(CreateAt) = @Month And Month(DelAt) = @Month1", whereClause);
                DictionaryAssert.AssertParameters(expectedParameters, parameters);
            }

            {
                var userFilter = new User_SqlFunc() { CreateAtMonth = 5 };
                Expression<Func<User_SqlFunc_Entity, bool>> expression =
                    u => SqlFunc.DbFunctions.Month(u.CreateAt) == userFilter.CreateAtMonth;
                (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
                Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
                expectedParameters.Add("@Month", 5);
                Assert.AreEqual("Month(CreateAt) = @Month", whereClause);
                DictionaryAssert.AssertParameters(expectedParameters, parameters);
            }
        }

        [TestMethod]
        public void ValidateMethodConstant2()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Internal.Age = 20;
            Expression<Func<User, bool>> expression = u => u.Age < GetInt(userFilter.Internal.Age);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateMethodChainConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < int.Parse(GetInt().ToString());
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateEqualMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.Equals(filter.Name.Substring(1));
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "ame");
            //Assert.AreEqual("(Name = @Name)", whereClause);
            Assert.AreEqual("Name = @Name", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateTernary()
        {
            string name = "Gary";
            Expression<Func<User, bool>> expression = u => u.Name == (name == null ? "Foo" : name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Gary");
            //Assert.AreEqual("((Name = @Name))", whereClause);
            Assert.AreEqual("Name = @Name", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateNotEqual()
        {
            Expression<Func<User, bool>> expression = u => u.Age != 20;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            //Assert.AreEqual("(Age <> @Age)", whereClause);
            Assert.AreEqual("Age <> @Age", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateStartsWithMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.StartsWith(filter.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Name%");
            //Assert.AreEqual("(Name Like @Name)", whereClause);
            Assert.AreEqual("Name Like @Name", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateStartWith2()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.StartsWith(filter.Name) || u.Name.Contains("Start");
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Name%");
            expectedParameters.Add("@Name1", "%Start%");
            Assert.AreEqual("((Name Like @Name) Or (Name Like @Name1))", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateEndsWithMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.EndsWith(filter.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "%Name");
            //Assert.AreEqual("(Name Like @Name)", whereClause);
            Assert.AreEqual("Name Like @Name", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateContainsMethod()
        {
            UserFilter filter = new UserFilter();
            filter.Name = "Name";
            Expression<Func<User, bool>> expression = u => u.Name.Contains(filter.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "%Name%");
            //Assert.AreEqual("(Name Like @Name)", whereClause);
            Assert.AreEqual("Name Like @Name", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateUnary()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
            {
                Expression<Func<User, bool>> expression = u => !u.Name.Contains("Name");
                var sql = expression.ToWhereClause(null, new TestSqlAdapter());
            });

        }

        [TestMethod]
        public void ValidateAll()
        {
            UserFilter filter = new UserFilter();
            filter.Internal.Age = 20;
            filter.Name = "Gary";
            Expression<Func<User, bool>> expression =
                u => ((u.Sex && u.Age > 18) || (u.Sex == false && u.Age > filter.Internal.Age))
                  && (u.Name == filter.Name || u.Name.Contains(filter.Name.Substring(1, 2)));
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Age", 18);
            expectedParameters.Add("@Sex1", false);
            expectedParameters.Add("@Age1", 20);
            expectedParameters.Add("@Name", "Gary");
            expectedParameters.Add("@Name1", "%ar%");
            Assert.AreEqual("((((Sex = @Sex) And ((Age > @Age))) Or (((Sex = @Sex1)) And ((Age > @Age1)))) And (((Name = @Name)) Or (Name Like @Name1)))", whereClause);

            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateArrayIn()
        {
            List<string> values = new List<string> { "a", "b" };
            Expression<Func<User, bool>> expression = u => values.Contains(u.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            //Assert.AreEqual("(Name in @Name)", whereClause);
            Assert.AreEqual("Name In @Name", whereClause);
            Assert.IsTrue(parameters["@Name"] is List<string>);
            Assert.AreEqual(string.Join(',', values), string.Join(',', (List<string>)parameters["@Name"]));
        }

        [TestMethod]
        public void ValidateEnumerableIn()
        {
            string[] values = new string[] { "a", "b" };
            Expression<Func<User, bool>> expression = u => values.Contains(u.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            //Assert.AreEqual("(Name in @Name)", whereClause);
            Assert.AreEqual("Name In @Name", whereClause);
            Assert.IsTrue(parameters["@Name"] is string[]);
            Assert.AreEqual(string.Join(',', values), string.Join(',', (string[])parameters["@Name"]));
        }

        [TestMethod]
        public void ValidateEnum()
        {
            Expression<Func<User, bool>> expression = u => u.Sex2 == Sex.Female;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            //Assert.AreEqual("((Sex2 = @Sex2))", whereClause);
            Assert.AreEqual("Sex2 = @Sex2", whereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex2", (int)Sex.Female);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateEnum2()
        {
            Sex[] sexes = new Sex[] { Sex.Female, Sex.Female };
            Expression<Func<User, bool>> expression = u => u.Sex2 == sexes[1];
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            //Assert.AreEqual("((Sex2 = @Sex2))", whereClause);
            Assert.AreEqual("Sex2 = @Sex2", whereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex2", (int)Sex.Female);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }

        private int GetInt()
        {
            return 20;
        }

        private int GetInt(int i)
        {
            return i;
        }

        [TestMethod]
        public void ValidateColumnAttr()
        {
            Expression<Func<User_columnAttr, bool>> expression = u => u.Age != 20;
            //优先级:方法参数的 alias > Column
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("UserAge <> @Age", whereClause);
            DictionaryAssert.AssertParameters(expectedParameters, parameters);
        }
    }
}
