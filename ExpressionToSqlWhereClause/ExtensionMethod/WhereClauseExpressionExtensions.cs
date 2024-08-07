﻿using ExpressionToSqlWhereClause.Exceptions;
using ExpressionToSqlWhereClause.ExpressionTree;
using ExpressionToSqlWhereClause.ExpressionTree.Adapter;
using ExpressionToSqlWhereClause.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ExpressionToSqlWhereClause.ExtensionMethod
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
            var parseResult = WhereClauseParser.Parse(body, alias, sqlAdapter);

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

            var result = new SearchCondition(
                  parseResult.WhereClause,
                  TrimParentheses(parseResult.WhereClause),
                  GetParameters(parseResult.Adhesive)
                );

            return result;
        }

        public static SearchCondition ToNumberParamNameWhereClause<T>(
           this Expression<Func<T, bool>> expression,
           Dictionary<string, string> alias = null,
           ISqlAdapter sqlAdapter = default) where T : class
        {
            var searchCondition = expression.ToWhereClause();
            WhereClauseHelper.ParamNameToNumber(searchCondition);//使用 FormattableParameters 有值
            return searchCondition;
        }

        public static SearchCondition ToFormattableWhereClause<T>(
            this Expression<Func<T, bool>> expression,
            Dictionary<string, string> alias = null,
            ISqlAdapter sqlAdapter = default) where T : class
        {
            var searchCondition = expression.ToWhereClause();
            WhereClauseHelper.ToFormattableString(searchCondition);//使用 FormattableParameters 有值
            return searchCondition;
        }

        /// <summary>
        /// 去掉()
        /// </summary>
        /// <param name="WhereClause"></param>
        /// <returns></returns>
        private static string TrimParentheses(string WhereClause)
        {
            var canTrimAll = ParenthesesTrimHelper.CanTrimAll(WhereClause, str =>
            {
                //CanTrimAll的基础上 在增加一个, 如果全部为Or 的, 那么可以去掉所有的 ()
                return str.Contains(SqlKeys.And) == false;
            });

            if (canTrimAll) //去掉 关系条件 全为 and 时的 ()
            {
                var str = ParenthesesTrimHelper.TrimAll(WhereClause, new char[] { '(', ')' });//全是and 的
                return str;
            }
            else
            {
                var str = ParenthesesTrimHelper.ParseTrimAll(WhereClause);
                return str;
            }
        }

        /// <summary>
        /// 获得参数
        /// </summary>
        /// <param name="Adhesive"></param>
        /// <returns></returns>
        private static Dictionary<string, object> GetParameters(WhereClauseAdhesive Adhesive)
        {
            var parameters = new Dictionary<string, object>(0);
            foreach (var key in Adhesive.GetParameterKeys())
            {
                parameters.Add(key, Adhesive.GetParameter(key).Value);
            }
            return parameters;
        }
    }

    public class SearchCondition
    {
        public SearchCondition()
        {
            this.WhereClauseRaw = string.Empty;
            this.WhereClause = string.Empty;
            this.Parameters = new Dictionary<string, object>();
            this.FormattableParameters = null;
        }

        public SearchCondition(string whereClauseRaw, string whereClause, Dictionary<string, object> parameters)
        {
            this.WhereClauseRaw = whereClauseRaw;
            this.WhereClause = whereClause;
            this.Parameters = parameters;
        }

        /// <summary>
        /// where子句
        /// </summary>
        public string WhereClause { get; private set; }

        /// <summary>
        /// 查询参数
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// FormattableStringd的
        /// </summary>
        public object[] FormattableParameters { get; private set; }

        /// <summary>
        /// 未加工处理的 WhereClause
        /// </summary>
        public string WhereClauseRaw { get; private set; }

        public void SetWhereClause(string whereClause)
        {
            this.WhereClause = whereClause;
        }

        public void SetFormattableStringParameters(string whereClause, object[] fmtParameters)
        {
            this.WhereClause = whereClause;
            this.FormattableParameters = fmtParameters;
        }

        public FormattableString GetFormattableString(string querySql)
        {
            var formattableStr = FormattableStringFactory.Create(querySql, this.FormattableParameters);
            return formattableStr;
        }
    }
}