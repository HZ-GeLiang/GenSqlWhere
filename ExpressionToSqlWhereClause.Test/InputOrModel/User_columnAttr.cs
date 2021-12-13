using System.ComponentModel.DataAnnotations.Schema;

namespace ExpressionToSqlWhereClause.Test
{
    public class User_columnAttr
    {
        [Column("UserAge")]   
        public int Age { get; set; }
    }
    
}
