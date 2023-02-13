using ExpressionToSqlWhereClause.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExpressionToSqlWhereClause.ExpressionTree;
using ExpressionToSqlWhereClause.ExpressionTree.Adapter;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace ExpressionToSqlWhereClause
{
    public static class WhereClauseExpressionExtensions
    {
        /// <summary>
        /// 转换为Where子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <param name="alias">别名</param>
        /// <param name="sqlAdapter">适配器</param>
        /// <returns></returns>
        public static (string WhereClause, Dictionary<string, object> Parameters) ToWhereClause<T>(
            this Expression<Func<T, bool>> expression,
            Dictionary<string, string> alias = null,
            ISqlAdapter sqlAdapter = default) where T : class
        {
            if (expression == null)
            {
                return (string.Empty, new Dictionary<string, object>(0));
            }

            #region 处理 alias
            alias ??= new Dictionary<string, string>();

            foreach (var propertyInfo in ReflectionHelper.GetProperties<T>())
            {
                //优先级: 方法参数的 alias > Column
                if (alias.ContainsKey(propertyInfo.Name))
                {
                    continue;
                }
                var attrs = ReflectionHelper.GetAttributeForProperty<ColumnAttribute>(propertyInfo, true);
                if (attrs.Length == 1)
                {
                    alias[propertyInfo.Name] = attrs[0].Name;
                }
            }
            #endregion

            var body = expression.Body;
            var parseResult = WhereClauseParser.Parse(body, alias, sqlAdapter);

            #region 处理 parseResult.Parameters

            foreach (var key in parseResult.Adhesive.GetParameterKeys())
            {
                var para = parseResult.Adhesive.GetParameter(key);
                var val = para.Value;

                #region 处理 val ==null 的情况

                if (val == null)
                {
                    if (para.Symbol == SqlKeys.Equal || para.Symbol == SqlKeys.NotEqual)
                    {
                        //为了支持 a.url == null , 此处需要翻译为 url is null
                        var sqlClauseOrigin = para.SqlClause;
                        var reg = "[ ].*[ ]@.*\\)$";
                        var strList = RegexHelper.GetStringByMatches(sqlClauseOrigin, reg);
                        if (strList.Count == 1)
                        {
                            string sqlClauseNew;
                            if (para.Symbol == SqlKeys.Equal)
                            {
                                sqlClauseNew = sqlClauseOrigin.Replace(strList[0], " Is Null)");
                            }
                            else if (para.Symbol == SqlKeys.NotEqual)
                            {
                                sqlClauseNew = sqlClauseOrigin.Replace(strList[0], " Is Not Null)");
                            }
                            else
                            {
                                throw new ExpressionToSqlWhereClauseException("逻辑错误,需要调整");
                            }

                            parseResult.WhereClause = parseResult.WhereClause.Replace(sqlClauseOrigin, sqlClauseNew);
                            parseResult.Adhesive.RemoveParameter(key);//移除参数

                            continue;
                        }
                        //else 没有处理 
                    }

                    //else 没有处理 
                }

                #endregion

                #region 翻译IEnumerable的值
                var parseValue = ConstantExtractor.ConstantExpressionValueToString(val, out var isIEnumerableObj);
                if (isIEnumerableObj)
                {
                    para.Value = parseValue;
                    continue;
                }
                #endregion
            }

            #endregion

            #region 处理 parseResult.WhereClause

            var whereClause = parseResult.WhereClause;

            #region 去掉 关系条件 全为 and 时 ,sql 语句的 () 可以优化
            //能力有限: 只能去掉全是 and 语句的 ()  注 in ()  的  () 不能排除, 所以,还要去掉 in
            var containsOr = whereClause.Contains(SqlKeys.or);
            if (!containsOr)
            {
                var containsin = whereClause.Contains(SqlKeys.@in);
                if (!containsin)
                {
                    if (whereClause.Contains("("))
                    {
                        whereClause = whereClause.Replace("(", string.Empty);
                    }
                    if (whereClause.Contains(")"))
                    {
                        whereClause = whereClause.Replace(")", string.Empty);
                    }
                }
            }
            #endregion

            #region 函数的[] 变成 ()

            if (whereClause.Contains("[") && whereClause.Contains("]"))
            {
                //需要注意, mssql下, 系统保留字是  [] 包裹的

                var syste_buildIn_names = alias.Values.Where(a => a.StartsWith("[") && a.EndsWith("]")).ToList();

                //把 []  包裹的用其他字符替换
                foreach (var item in syste_buildIn_names)
                {
                    var newValue = $@",{item.Substring(1, item.Length - 2)},";
                    whereClause = whereClause.Replace(item, newValue);
                }


                if (whereClause.Contains("[") && whereClause.Contains("]"))
                {
                    //这里是无脑替换.
                    whereClause = whereClause.Replace("[", "(");
                    whereClause = whereClause.Replace("]", ")");
                }

                //然后替换回来
                foreach (var item in syste_buildIn_names)
                {
                    var oldValue = $@",{item.Substring(1, item.Length - 2)},";
                    whereClause = whereClause.Replace(oldValue, item);
                }

            }

            #endregion

            #endregion

            var parameters = new Dictionary<string, object>(0);
            foreach (var key in parseResult.Adhesive.GetParameterKeys())
            {
                parameters.Add(key, parseResult.Adhesive.GetParameter(key).Value);
            }
            var result = (parseResult.WhereClause, parameters);
            return result;
        }
    }
}
