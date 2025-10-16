using ExpressionToSqlWhereClaus.Test;
using Infra.ExtensionMethods;
using Infra.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause;

[TestClass]
public class ExpressionDemo
{
    [TestMethod]
    public void 一元表达式_AndAlsoTest()
    {
        {
            //新增支持;一元表达式
            var expression =
            default(Expression<Func<AndAlsoInput, bool>>)
            .WhereIf(true, a => !a.IsDept && a.IsAudit)
            //.WhereIf(true, a => a.IsDel != true)
            ;
            var searchCondition = expression.ToWhereClause();
            var sql = searchCondition.WhereClause;

            Assert.AreEqual(sql, "IsDept <> @IsDept And IsAudit = @IsAudit");
            Assert.AreEqual(searchCondition.Parameters["@IsDept"], true);
            Assert.AreEqual(searchCondition.Parameters["@IsAudit"], true);
        }

        {
            //目前支持
            var expression =
              default(Expression<Func<AndAlsoInput, bool>>)
              .WhereIf(true, a => a.IsDept == false && a.IsAudit == true);
            var searchCondition = expression.ToWhereClause();
            var sql = searchCondition.WhereClause;

            Assert.AreEqual(sql, "IsDept = @IsDept And IsAudit = @IsAudit");
            Assert.AreEqual(searchCondition.Parameters["@IsDept"], false);
            Assert.AreEqual(searchCondition.Parameters["@IsAudit"], true);
        }
    }

    [TestMethod]
    public void 常规条件Or一个特定的操作()
    {
        var input = new Sample250828Input()
        {
            Id = 1,
            Production = "aa",
            Unit_Name = "bb",
            Remarks = "cc",
        };

        //常规条件
        var expression = default(Expression<Func<Sample250828Input, bool>>)
           .WhereIf(!string.IsNullOrEmpty(input.Production), a => a.Production.Contains(input.Production))
           .WhereIf(!string.IsNullOrEmpty(input.Unit_Name), a => a.Unit_Name.Contains(input.Unit_Name))
           .WhereIf(!string.IsNullOrEmpty(input.Remarks), a => a.Remarks.Contains(input.Remarks))
           ;

        //一个特定的操作
        if (input.Id > 0)
        {
            expression = expression.Or(a => a.Id == 1);
        }

        var searchCondition = expression.ToWhereClause();
        var clause = WhereClauseHelper.GetNonParameterClause(searchCondition);

        Assert.AreEqual(clause, "(Production Like '%aa%' And Unit_Name Like '%bb%' And Remarks Like '%cc%') Or Id = 1");
    }

    [TestMethod]
    public void IsNull的生成()
    {
        {
            //增加 nfx 下的EF, string.IsNullOrEmpty() 可以出入Null值
            var expression =
                default(Expression<Func<ShopInfoInput, bool>>)
                .WhereIf(true, a => string.IsNullOrEmpty(a.Production));
            Assert.ThrowsException<Exception>(() => expression.ToWhereClause());
            try
            {
                expression.ToWhereClause();
            }
            catch (Exception ex)
            {
                var exMsg = "Please use (a.Production ?? \"\") != \"\" replace string.IsNullOrEmpty(a.Production).";
                Assert.AreEqual(ex.Message, exMsg);
            }
        }

        //==
        {
            var expression =
                default(Expression<Func<ShopInfoInput, bool>>)
                .WhereIf(true, a => (a.Production ?? "") == "");

            var searchCondition = expression.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "IsNull(Production, N'') = ''");
        }

        {
            var expression =
                default(Expression<Func<ShopInfoInput, bool>>)
                .WhereIf(true, a => (a.Unit ?? 0) == 0);

            var searchCondition = expression.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "IsNull(Unit, 0) = 0");
        }

        //!=
        {
            var expression =
                default(Expression<Func<ShopInfoInput, bool>>)
                .WhereIf(true, a => (a.Production ?? "") != "");

            var searchCondition = expression.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "IsNull(Production, N'') <> ''");
        }

        {
            var expression =
                default(Expression<Func<ShopInfoInput, bool>>)
                .WhereIf(true, a => (a.Unit ?? 0) != 0);

            var searchCondition = expression.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "IsNull(Unit, 0) <> 0");
        }

        //多个条件-and
        {
            var expression =
                default(Expression<Func<ShopInfoInput, bool>>)
                .WhereIf(true, a => (a.Production ?? "") == "")
                .WhereIf(true, a => (a.Unit ?? 0) == 0);

            var searchCondition = expression.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "IsNull(Production, N'') = '' And IsNull(Unit, 0) = 0");
        }

        //多个条件-or
        {
            var expression =
                default(Expression<Func<ShopInfoInput, bool>>)
                .WhereIf(true, a => (a.Production ?? "") == "")
                .OrIf(true, a => (a.Unit ?? 0) == 0);

            var searchCondition = expression.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "IsNull(Production, N'') = '' Or IsNull(Unit, 0) = 0");
        }
    }

    [TestMethod]
    public void 获得sql_不使用sql参数()
    {
        var input = new ShopInfoInput()
        {
            Production = "aa",
        };

        var expression = default(Expression<Func<ShopInfoInput, bool>>)
           .WhereIf(!string.IsNullOrEmpty(input.Production), a => a.Production.Contains(input.Production));

        var searchCondition = expression.ToWhereClause();
        var clause = WhereClauseHelper.GetNonParameterClause(searchCondition);

        Assert.AreEqual(clause, "Production Like '%aa%'");
    }

    [TestMethod]
    public void 获得sql_不使用sql参数_使用EF的直接查询()
    {
        var input = new ShopInfoInput()
        {
            Production = "aa",
        };

        var expression = default(Expression<Func<ShopInfoInput, bool>>)
           .WhereIf(!string.IsNullOrEmpty(input.Production), a => a.Production.Contains(input.Production));

        var searchCondition = expression.ToWhereClause();
        var clause = WhereClauseHelper.GetFormattableStringClause(searchCondition);
        Assert.AreEqual(clause, "Production Like {0}");

        /*一个示例

            var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
                .WhereIf(true, a => a.Id == 1)
                .WhereIf(true, a => a.Name == "abc")
                .WhereIf(true, a => a.CreateAt > new DateTime(2023, 1, 1))
                .WhereIf(true, a => a.Flag.Value == Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81"));

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

        Dictionary<string, object> para = new()
        {
            { "@Id", 1 },
            { "@Id1", 2 },
            { "@IsDel", true },
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

        Dictionary<string, object> para = new()
        {
            { "@Id", 1 },
            { "@Id1", 2 },
            { "@IsDel", true },
        };
        CollectionAssert.AreEqual(searchCondition.Parameters, para);

        //这个是   WhereClauseHelper.ConvertParameters 使用示例
        var pms = WhereClauseHelper.ConvertParameters(searchCondition.Parameters, (key, val) =>
        {
            return new /*SqlParameter*/ { ParameterName = key, Value = val };
        });
    }

    [TestMethod]
    public void ValidateString()
    {
        Expression<Func<User, bool>> expression = u => u.Name != "aa";
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Name", "aa" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Name <> @Name");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateBool()
    {
        Expression<Func<User, bool>> expression = u => !u.Sex;

        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Sex", false }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Sex <> @Sex");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateBool2()
    {
        Expression<Func<User, bool>> expression = u => u.Sex && u.Name.StartsWith("Foo");
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Sex", true },
            { "@Name", "Foo%" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Sex = @Sex And Name Like @Name");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateBool3()
    {
        Expression<Func<User, bool>> expression = u => !(u.Sex == false);

        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Sex", true }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Sex <> @Sex");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateBool4()
    {
        Expression<Func<User, bool>> expression = u => !(u.Sex == true);
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Sex", false }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Sex <> @Sex");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateConstant()
    {
        Expression<Func<User, bool>> expression = u => u.Age >= 20;
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Age >= @Age");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateVariable()
    {
        int age = 20;
        Expression<Func<User, bool>> expression = u => u.Age >= age;
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Age >= @Age");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateAnd()
    {
        Expression<Func<User, bool>> expression = u => u.Sex && u.Age > 20;

        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Sex", true },
            { "@Age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Sex = @Sex And Age > @Age");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateOr()
    {
        Expression<Func<User, bool>> expression = u => u.Sex || u.Age > 20;

        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Sex", true },
            { "@Age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Sex = @Sex Or Age > @Age");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateDuplicateField()
    {
        Expression<Func<User, bool>> expression = u => u.Age < 15 || u.Age > 20;

        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 15 },
            { "@Age1", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Age < @Age Or Age > @Age1");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateMemberConstant()
    {
        UserFilter userFilter = new()
        {
            Age = 20
        };
        Expression<Func<User, bool>> expression = u => u.Age < userFilter.Age;
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Age < @Age");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateDeepMemberConstant()
    {
        UserFilter userFilter = new();
        userFilter.Internal.Age = 20;
        Expression<Func<User, bool>> expression = u => u.Age < userFilter.Internal.Age;
        var searchCondition = expression.ToWhereClause(sqlAdapter: new ToLowerSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "age < @age");//因为 ToLowerSqlAdapter
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateInstanceMethodConstant()
    {
        Expression<Func<User, bool>> expression = u => u.Age < GetInt();
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Age < @Age");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateStaticMethodConstant()
    {
        Expression<Func<User, bool>> expression = u => u.Age < UserFilter.GetInt(20);
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Age < @Age");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateMethodConstant2()
    {
        UserFilter userFilter = new();
        userFilter.Internal.Age = 20;
        Expression<Func<User, bool>> expression = u => u.Age < GetInt(userFilter.Internal.Age);
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Age < @Age");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateMethodChainConstant()
    {
        //#issue 2025.6.19-2  类型不对
        Expression<Func<User, bool>> expression = u => u.Age < int.Parse(GetInt().ToString());
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 20 } //这里正确的类型是 int  而不是 string
        };
        Assert.AreEqual(searchCondition.WhereClause, "Age < @Age");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateEqualMethod()
    {
        UserFilter filter = new()
        {
            Name = "Name"
        };
        Expression<Func<User, bool>> expression = u => u.Name.Equals(filter.Name.Substring(1));
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Name", "ame" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Name = @Name");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateTernary()
    {
        string name = "Gary";
        Expression<Func<User, bool>> expression = u => u.Name == (name == null ? "Foo" : name);
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Name", "Gary" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Name = @Name");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateTernary2()
    {
        string name = "Gary";
        Expression<Func<User, bool>> expression = u => u.Name == (name ?? "Foo"); //简化的语法
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Name", "Gary" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Name = @Name");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateTernary3()
    {
        string name = null;
        Expression<Func<User, bool>> expression = u => u.Name == (name ?? "Foo"); //简化的语法
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Name", "Foo" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Name = @Name");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateNotEqual()
    {
        Expression<Func<User, bool>> expression = u => u.Age != 20;
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Age <> @Age");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateStartsWithMethod()
    {
        UserFilter filter = new()
        {
            Name = "Name"
        };
        Expression<Func<User, bool>> expression = u => u.Name.StartsWith(filter.Name);
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Name", "Name%" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Name Like @Name");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateStartWith2()
    {
        UserFilter filter = new()
        {
            Name = "Name"
        };
        Expression<Func<User, bool>> expression = u => u.Name.StartsWith(filter.Name) || u.Name.Contains("Start");
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Name", "Name%" },
            { "@Name1", "%Start%" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Name Like @Name Or Name Like @Name1");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateEndsWithMethod()
    {
        UserFilter filter = new()
        {
            Name = "Name"
        };
        Expression<Func<User, bool>> expression = u => u.Name.EndsWith(filter.Name);
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Name", "%Name" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Name Like @Name");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateContainsMethod()
    {
        UserFilter filter = new()
        {
            Name = "Name"
        };
        Expression<Func<User, bool>> expression = u => u.Name.Contains(filter.Name);
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Name", "%Name%" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "Name Like @Name");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
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
    public void case_复杂版的表达式解析1()
    {
        UserFilter filter = new();
        filter.Internal.Age = 20;
        filter.Name = "Gary";

        Expression<Func<User, bool>> expression =
        //  (========left======) || (==================right====================)
        u => u.Sex && u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age;

        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "(Sex = @Sex And Age > @Age) Or (Sex = @Sex1 And Age > @Age1)");
    }

    [TestMethod]
    public void case_复杂版的表达式解析2()
    {
        UserFilter filter = new();
        filter.Internal.Age = 20;
        filter.Name = "Gary";
        Expression<Func<User, bool>> expression =
        //  (==left) && (==================================right====================)
        //              (==left==)  ||(====================right====================)
        u => u.Sex && (u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age);

        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Sex = @Sex And (Age > @Age Or (Sex = @Sex1 And Age > @Age1))");
    }

    [TestMethod]
    public void case_复杂版的表达式解析3()
    {
        UserFilter filter = new();
        filter.Internal.Age = 20;
        filter.Name = "Gary";

        Expression<Func<User, bool>> expression =
            u => u.Age > 10
                  && (u.Sex && u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age)
                  && (u.Name == filter.Name || u.Name.Contains(filter.Name.Substring(1, 2)));
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Age > @Age And ((Sex = @Sex And Age > @Age1) Or (Sex = @Sex1 And Age > @Age2)) And (Name = @Name Or Name Like @Name1)");
    }

    //[TestMethod]
    public void case_复杂版的表达式解析4()
    {
        UserFilter filter = new();
        filter.Internal.Age = 20;
        filter.Name = "Gary";

        Expression<Func<User, bool>> expression =
            u => u.Age > 10
                  || (u.Sex && u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age)
                  || (u.Name == filter.Name || u.Name.Contains(filter.Name.Substring(1, 2)));
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Age > @Age Or ((Sex = @Sex And Age > @Age1) Or (Sex = @Sex1 And Age > @Age2)) Or Name = @Name Or Name Like @Name1");
    }

    //[TestMethod]
    public void case_复杂版的表达式解析5()
    {
        UserFilter filter = new();
        filter.Internal.Age = 20;
        filter.Name = "Gary";

        Expression<Func<User, bool>> expression =
            u => (u.Age > 10) || (
                        (u.Age > 10 && u.Age > 11)
                         && (u.Sex && u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age)
                         && (u.Name == filter.Name || u.Name.Contains(filter.Name.Substring(1, 2)))
                      );
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "太复杂的去掉()的成本太高, 不写了...");
    }

    [TestMethod]
    public void case_去掉括号_3个or()
    {
        UserFilter filter = new();
        filter.Internal.Age = 20;

        Expression<Func<User, bool>> expression =
            u => u.Age > 10
                  || u.Age > 11
                  || u.Age > 12;
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Age > @Age Or Age > @Age1 Or Age > @Age2");
    }

    [TestMethod]
    public void case_去掉括号_4个or()
    {
        UserFilter filter = new();
        filter.Internal.Age = 20;

        Expression<Func<User, bool>> expression =
            u => u.Age > 10
                  || u.Age > 11
                  || u.Age > 12
                  || u.Age > 13;
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Age > @Age Or Age > @Age1 Or Age > @Age2 Or Age > @Age3");
    }

    [TestMethod]
    public void ValidateAll()
    {
        UserFilter filter = new();
        filter.Internal.Age = 20;
        filter.Name = "Gary";
        Expression<Func<User, bool>> expression =
            u =>
             (u.Sex && u.Age > 18 || u.Sex == false && u.Age > filter.Internal.Age)
              && (u.Name == filter.Name || u.Name.Contains(filter.Name.Substring(1, 2)));
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Sex", true },
            { "@Age", 18 },
            { "@Sex1", false },
            { "@Age1", 20 },
            { "@Name", "Gary" },
            { "@Name1", "%ar%" }
        };
        Assert.AreEqual(searchCondition.WhereClause, "((Sex = @Sex And Age > @Age) Or (Sex = @Sex1 And Age > @Age1)) And (Name = @Name Or Name Like @Name1)");

        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateArrayIn()
    {
        List<string> values = new() { "a", "b" };
        Expression<Func<User, bool>> expression = u => values.Contains(u.Name);
        var searchCondition = expression.ToWhereClause(sqlAdapter: new TestSqlAdapter());
        Assert.AreEqual(searchCondition.WhereClause, "Name In (@Name1 , @Name2 )");
        Dictionary<string, object> expectedParameters = new()
        {
            //{ "@Name", "'a','b'" }
            { "@Name1", "a" },
            { "@Name2", "b" },
        };
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateEnumerableIn()
    {
        string[] values = new string[] { "a", "b" };
        Expression<Func<User, bool>> expression = u => values.Contains(u.Name);
        var searchCondition = expression.ToWhereClause(sqlAdapter: new TestSqlAdapter());
        Assert.AreEqual(searchCondition.WhereClause, "Name In (@Name1 , @Name2 )");
        Dictionary<string, object> expectedParameters = new()
        {
            //{ "@Name", "'a','b'" }
            { "@Name1", "a" },
            { "@Name2", "b" },
        };
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void ValidateEnum()
    {
        Expression<Func<User, bool>> expression = u => u.Sex2 == Sex.Female;
        var searchCondition = expression.ToWhereClause(null, new TestSqlAdapter());
        Assert.AreEqual(searchCondition.WhereClause, "Sex2 = @Sex2");
        Dictionary<string, object> expectedParameters = new()
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
        Assert.AreEqual(searchCondition.WhereClause, "Sex2 = @Sex2");
        Dictionary<string, object> expectedParameters = new()
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
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 20 }
        };
        Assert.AreEqual(searchCondition.WhereClause, "UserAge <> @Age");
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

        Dictionary<string, object> expectedParameters = new()
        {
            { "@Age", 20 },
            { "@CreateStart", d1 },
            { "@CreateEnd", d2 },
        };
        Assert.AreEqual(searchCondition.WhereClause, "Age <> @Age Or (CreateStart > @CreateStart And CreateEnd < @CreateEnd)");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void 测试_表达式的生成_new一个DateTime()
    {
        var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
            .WhereIf(true, a => a.CreateAt > new DateTime(2023, 1, 1));

        var searchCondition = expression.ToWhereClause();
        Dictionary<string, object> expectedParameters = new()
        {
            { "@CreateAt", new DateTime(2023, 1, 1) }
        };

        Assert.AreEqual(searchCondition.WhereClause, "CreateAt > @CreateAt");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void 测试_表达式的生成_new一个Guid()
    {
        var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
            .WhereIf(true, a => a.Flag.Value == Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81"));

        var searchCondition = expression.ToWhereClause();
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Flag", Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81") }
        };

        Assert.AreEqual(searchCondition.WhereClause, "Flag = @Flag");
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void 测试_解析nullable的sql()
    {
        var v4 = Guid.Parse("DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81");
        Dictionary<string, object> expectedParameters = new()
        {
            { "@Flag", v4 }
        };

        {
            var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
                .WhereIf(true, a => a.Flag.Value == v4);

            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Flag = @Flag");
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }
        {
            var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
                .WhereIf(true, a => a.Flag == v4);

            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Flag = @Flag");
            DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
        }
    }

    [TestMethod]
    public void 测试_别名的替换()
    {
        var expression = default(Expression<Func<ExpressionSqlTest, bool>>)
            .WhereIf(true, a => a.CreateAtDay > 1);

        var searchCondition = expression.ToWhereClause(new Dictionary<string, string>()
        {
            { "CreateAtDay","datediff(day, CreateAt, GETDATE())"}
        });

        Assert.AreEqual(searchCondition.WhereClause, "datediff(day, CreateAt, GETDATE()) > @CreateAtDay");
        //DictionaryAssert.AreEqual(expectedParameters, where.Parameters);
    }
}

public record class ExpressionSqlTest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? CreateAt { get; set; }
    public int? CreateAtDay { get; set; }
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

public class Sample250828Input
{
    public int Id { get; set; }

    public string Production { get; set; }

    public string Unit_Name { get; set; }
    public string Remarks { get; set; }
}

public class AndAlsoInput
{
    public bool IsDept { get; set; }
    public bool IsAudit { get; set; }
    public bool IsDel { get; set; }
}