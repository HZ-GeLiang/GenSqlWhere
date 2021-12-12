using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSqlWhereClause
{
    /// <summary>
    /// 优先级:方法参数的 alias > Column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }

}
