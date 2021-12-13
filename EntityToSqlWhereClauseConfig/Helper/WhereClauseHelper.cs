using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace EntityToSqlWhereClauseConfig.Helper
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
            if (string.IsNullOrWhiteSpace(whereclause))
            {
                return null;
            }

            string pattern = "@[a-zA-Z0-9_]*";
            var matches = Regex.Matches(whereclause, pattern);
            //要倒序替换
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                whereclause = whereclause.Replace(matches[i].Value, "@" + i);
            }
            return whereclause;
        }

        //public static System.Data.SqlClient.SqlParameter[] ParamToSqlParameter(Dictionary<string, object> param)
        //{
        //    if (param == null)
        //    {
        //        return new System.Data.SqlClient.SqlParameter[0];
        //    }
        //    var cmdParms = new System.Data.SqlClient.SqlParameter[param.Keys.Count];
        //    var i = 0;
        //    foreach (var key in param.Keys)
        //    {
        //        cmdParms[i] = new System.Data.SqlClient.SqlParameter(key, param[key]);
        //        i++;
        //    }
        //    return cmdParms;
        //}

    }
}
