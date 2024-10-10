using ExpressionToSqlWhereClause.Exceptions;
using ExpressionToSqlWhereClause.ExpressionTree;
using ExpressionToSqlWhereClause.ExpressionTree.Adapter;
using ExpressionToSqlWhereClause.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.ExtensionMethods;

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
    public static SearchCondition ToWhereClause<T>(
        this Expression<Func<T, bool>> expression,
        Dictionary<string, string> alias = null,
        ISqlAdapter sqlAdapter = default) where T : class
    {
        if (expression == null)
        {
            return new SearchCondition();
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
        ClauseParserResult parseResult = WhereClauseParser.Parse(body, alias, sqlAdapter);

        #region 处理 parseResult 的  Adhesive  和 WhereClause

        foreach (var key in parseResult.Adhesive.GetParameterKeys())
        {
            var para = parseResult.Adhesive.GetParameter(key);
            var val = para.Value;

            // 处理 val == null 的情况
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
                            throw new FrameException("逻辑错误,需要调整");
                        }

                        parseResult.WhereClause = parseResult.WhereClause.Replace(sqlClauseOrigin, sqlClauseNew);
                        parseResult.Adhesive.RemoveParameter(key);//移除参数

                        continue;
                    }
                    //else 没有处理
                }

                //else 没有处理
            }

            //翻译IEnumerable的值
            var parseValue = ConstantExtractor.ConstantExpressionValueToString(val, out var isIEnumerableObj);
            if (isIEnumerableObj)
            {
                para.Value = parseValue;
                continue;
            }
        }

        #endregion

        var result = new SearchCondition(parseResult);
        return result;
    }
}