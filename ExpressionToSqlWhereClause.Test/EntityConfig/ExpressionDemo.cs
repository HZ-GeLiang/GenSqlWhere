using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.Helper.EntityConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlWhere.ExtensionMethod;

namespace SqlWhere.EntityConfig
{
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
               .WhereIf(input.Unit.HasValue, a => a.Unit == input.Unit)
               //    .WhereIf(input.Unit.HasValue, a => a.Unit == input.Unit.Value)//todo:支持Nullbale<>
               .WhereIf(!string.IsNullOrEmpty(input.Unit_Name), a => a.Unit_Name.Contains(input.Unit_Name))
               .WhereIf(!string.IsNullOrEmpty(input.Remarks), a => a.Remarks.Contains(input.Remarks))
               .WhereIf(input.CreateUserID.HasValue, a => a.CreateUserID == input.CreateUserID)
               .WhereIf(input.DateStart.HasValue, a => a.Date >= input.DateStart)
               .WhereIf(input.DateEnd.HasValue, a => a.Date <= input.DateEnd)
            ;

            //MergeParametersIntoSql 的 2种使用方式
            {
                var a = expression.ToWhereClause();
                var sql2 = WhereClauseHelper.MergeParametersIntoSql(a);
                Assert.AreEqual(sql2, "Production Like '%aa%'");
            }

            {
                (string sql, Dictionary<string, object> param) = expression.ToWhereClause();
                var sql2 = WhereClauseHelper.MergeParametersIntoSql(sql, param);
                Assert.AreEqual(sql2, "Production Like '%aa%'");
            }


        }

        [TestMethod]
        public void boolean值的写法1()
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
        public void boolean值的写法2()
        {
            Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; });
            //                    和  () => { return x => x.IsDel; } 不一样

            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause();

            Assert.AreEqual(WhereClause, "((((Id = @Id)) Or ((Id = @Id1))) And ((IsDel = @IsDel)))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(Parameters, para);


            //这个是   WhereClauseHelper.ConvertParameters 使用示例
            var pms = WhereClauseHelper.ConvertParameters(Parameters, (key, val) =>
            {
                return new { ParameterName = key, Value = val };
            });
        }

        [TestMethod]
        public void ValidateString()
        {
            Expression<Func<User, bool>> expression = u => u.Name != "aa";

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "aa");
            Assert.AreEqual("Name <> @Name", sql);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateBool()
        {
            Expression<Func<User, bool>> expression = u => !u.Sex;

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", false);
            Assert.AreEqual("Sex <> @Sex", sql);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateBool2()
        {
            Expression<Func<User, bool>> expression = u => u.Sex && u.Name.StartsWith("Foo");
            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            expectedParameters.Add("@Name", "Foo%");
            Assert.AreEqual("Sex = @Sex And Name Like @Name", sql);
            DictionaryAssert.AreEqual(expectedParameters, parameters);

        }
        [TestMethod]
        public void ValidateBool3()
        {
            Expression<Func<User, bool>> expression = u => !(u.Sex == false);

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", true);
            Assert.AreEqual("Sex <> @Sex", sql);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateBool4()
        {
            Expression<Func<User, bool>> expression = u => !(u.Sex == true);

            (string sql, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Sex", false);
            Assert.AreEqual("Sex <> @Sex", sql);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }


        [TestMethod]
        public void ValidateConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age >= 20;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age >= @Age", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            Assert.AreEqual("Age < @Age", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }


        [TestMethod]
        public void ValidateDeepMemberConstant()
        {
            UserFilter userFilter = new UserFilter();
            userFilter.Internal.Age = 20;
            Expression<Func<User, bool>> expression = u => u.Age < userFilter.Internal.Age;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(sqlAdapter: new ToLowerSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("age < @Age", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateInstanceMethodConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < GetInt();
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateStaticMethodConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < UserFilter.GetInt(20);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateMethodChainConstant()
        {
            Expression<Func<User, bool>> expression = u => u.Age < int.Parse(GetInt().ToString());
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age < @Age", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            Assert.AreEqual("Name = @Name", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateTernary()
        {
            string name = "Gary";
            Expression<Func<User, bool>> expression = u => u.Name == (name == null ? "Foo" : name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Name", "Gary");
            Assert.AreEqual("Name = @Name", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateNotEqual()
        {
            Expression<Func<User, bool>> expression = u => u.Age != 20;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>();
            expectedParameters.Add("@Age", 20);
            Assert.AreEqual("Age <> @Age", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            Assert.AreEqual("Name Like @Name", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            Assert.AreEqual("Name Like @Name", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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
            Assert.AreEqual("Name Like @Name", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
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

            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateArrayIn()
        {
            List<string> values = new List<string> { "a", "b" };
            Expression<Func<User, bool>> expression = u => values.Contains(u.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(sqlAdapter: new TestSqlAdapter());
            Assert.AreEqual("(Name In (@Name))", whereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Name", "a,b" }
            };
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateEnumerableIn()
        {
            string[] values = new string[] { "a", "b" };
            Expression<Func<User, bool>> expression = u => values.Contains(u.Name);
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(sqlAdapter: new TestSqlAdapter());
            Assert.AreEqual("(Name In (@Name))", whereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Name", "a,b" }
            };
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateEnum()
        {
            Expression<Func<User, bool>> expression = u => u.Sex2 == Sex.Female;
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Assert.AreEqual("Sex2 = @Sex2", whereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Sex2", (int)Sex.Female }
            };
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void ValidateEnum2()
        {
            Sex[] sexes = new Sex[] { Sex.Female, Sex.Female };
            Expression<Func<User, bool>> expression = u => u.Sex2 == sexes[1];
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause(null, new TestSqlAdapter());
            Assert.AreEqual("Sex2 = @Sex2", whereClause);
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Sex2", (int)Sex.Female }
            };
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        private int GetInt() => 20;

        private int GetInt(int i) => i;

        [TestMethod]
        public void ValidateColumnAttr()
        {
            Expression<Func<User_columnAttr, bool>> expression = u => u.Age != 20;
            //优先级:方法参数的 alias > Column
            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();
            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Age", 20 }
            };
            Assert.AreEqual("UserAge <> @Age", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }

        [TestMethod]
        public void 测试范围是否有问题()
        {
            DateTime? d1 = new DateTime(2020, 1, 1);
            DateTime? d2 = new DateTime(2020, 1, 15);
            //                                             lambda     left       ||   right
            Expression<Func<UserCreateTime, bool>> expression = u => u.Age != 20 || (u.CreateStart > d1 && u.CreateEnd < d2);

            (string whereClause, Dictionary<string, object> parameters) = expression.ToWhereClause();

            Dictionary<string, object> expectedParameters = new Dictionary<string, object>
            {
                { "@Age", 20 },
                { "@CreateStart", d1 },
                { "@CreateEnd", d2 },
            };
            Assert.AreEqual("(((Age <> @Age)) Or (((CreateStart > @CreateStart)) And ((CreateEnd < @CreateEnd))))", whereClause);
            DictionaryAssert.AreEqual(expectedParameters, parameters);
        }
    }
}
