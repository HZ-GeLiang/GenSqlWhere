using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ExpressionToSqlWhereClause.Helper
{
    internal sealed class RegexHelper
    {
        public static List<string> GetStringByMatches(string source, string pattern)
        {
            MatchCollection regex = Regex.Matches(source, pattern);
            List<string> list = new List<string>();
            foreach (Match item in regex)
            {
                list.Add(item.Value);
            }
            return list;
        }
    }
}
