using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpressionToSqlWhereClause.Test
{
    public class User_SqlFunc_Column
    {
        [Column("CreateTime")]
        public DateTime CreateAt { get; set; }
    }

}
