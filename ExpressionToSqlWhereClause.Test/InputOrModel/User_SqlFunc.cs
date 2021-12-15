using System;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.Test
{
    /// <summary>
    /// input
    /// </summary>
    public class User_SqlFunc
    {
        public int CreateAtMonth { get; set; }
        public int DelAtMonth { get; set; }
    }

    public class User_SqlFunc2
    {
        public List<int> CreateAtMonth { get; set; }
    }

    /// <summary>
    /// 数据库实体
    /// </summary>
    public class User_SqlFunc_Entity
    {
        public DateTime CreateAt { get; set; }
        public DateTime DelAt { get; set; }
    }
}
