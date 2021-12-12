using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionToSqlWhereClause.Test.InputOrModel
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
    //public enum Sex
    //{
    //    Male = 1,
    //    Female = 2
    //}
}
