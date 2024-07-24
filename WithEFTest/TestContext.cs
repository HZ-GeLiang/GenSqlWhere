using Microsoft.EntityFrameworkCore;

public partial class TestContext : DbContext
{
    public TestContext()
    {
    }

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
    }

    public DbSet<ExpressionSqlTest> ExpressionSqlTests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connStr = "Data Source = 127.0.0.1 ; Initial Catalog = EFCorePractice; Integrated Security = True";
            optionsBuilder.UseSqlServer(connStr);
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

/*
 create table ExpressionSqlTest
(
    Id       int identity,
    Name     nvarchar(50),
    CreateAt datetime,
    Flag     uniqueidentifier
)
go

create unique index ExpressionSqlTest_Id_uindex
    on ExpressionSqlTest (Id)
go

INSERT INTO EFCorePractice.dbo.ExpressionSqlTest (Name, CreateAt, Flag) VALUES (N'abc', N'2023-02-14 23:17:33.000', N'DEA5B56C-D1B1-4513-83E7-B58B9D3EBB81')

 */