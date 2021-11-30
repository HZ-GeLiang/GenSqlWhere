using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EntityToSqlWhereCaluseConfig.Helper
{
    public class WhereClauseHelper
    {
        /// <summary>
        /// sql参数名转成数字的
        /// </summary>
        /// <param name="whereclause"></param>
        /// <returns></returns>
        public static string ParamNameToNumber(string whereclause)
        {
            string pattern = "@[a-zA-Z0-9_]*";
            var matches = Regex.Matches(whereclause, pattern);
            //要倒序替换
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                whereclause = whereclause.Replace(matches[i].Value, "@" + i);
            }
            return whereclause;
        }
    }
}
