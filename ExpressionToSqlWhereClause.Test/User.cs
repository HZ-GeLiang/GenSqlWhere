using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionToSqlWhereClause.Test
{
    
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
}
