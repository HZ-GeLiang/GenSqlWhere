﻿using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.Exceptions;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.SqlFunc;
using ExpressionToSqlWhereClause.SqlFunc.EntityConfig;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ExpressionToSqlWhereClause.Helpers;

/// <summary>
/// 条件表达式帮助类
/// </summary>
public static class QueryConfigHelper
{
    #region AddLike

    public static List<Expression<Func<TDbEntity, bool>>> AddLike<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();
        foreach (var prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
            if (ContinueAdd(that, prop, value))
            {
                continue;
            }
            if (value is string valueStr && !string.IsNullOrWhiteSpace(valueStr))
            {
                var exp = QueryConfigHelper.GetExpression_Contains<TDbEntity>(prop, valueStr);
                if (exp != null)
                {
                    whereLambdas.Add(exp);
                }
            }
        }
        return whereLambdas;
    }

    // t.SomeProperty.Contains("stringValue");
    private static Expression<Func<TDbEntity, bool>> GetExpression_Contains<TDbEntity>(string propertyName, string propertyValue)
        where TDbEntity : class
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            return null;
        }

        var parameterExp = Expression.Parameter(type_TEntity, "a");
        var propertyExp = Expression.Property(parameterExp, propertyName);//a.UserNickName
        var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        var someValue = Expression.Constant(propertyValue, typeof(string));
        var containsMethodExp = Expression.Call(propertyExp, method, someValue);//a.UserNickName.Contains(xx);
        return Expression.Lambda<Func<TDbEntity, bool>>(containsMethodExp, parameterExp);
    }

    #endregion

    #region AddLikeLeft

    public static List<Expression<Func<TDbEntity, bool>>> AddLikeLeft<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();
        foreach (var prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
            if (ContinueAdd(that, prop, value))
            {
                continue;
            }
            if (value is string valueStr && !string.IsNullOrWhiteSpace(valueStr))
            {
                var exp = QueryConfigHelper.GetExpression_StartsWith<TDbEntity>(prop, valueStr);
                if (exp != null)
                {
                    whereLambdas.Add(exp);
                }
            }
        }
        return whereLambdas;
    }

    // t.SomeProperty.StartsWith("stringValue");
    private static Expression<Func<TDbEntity, bool>> GetExpression_StartsWith<TDbEntity>(string propertyName, string propertyValue)
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            return null;
        }

        var parameterExp = Expression.Parameter(type_TEntity, "a");
        var propertyExp = Expression.Property(parameterExp, propertyName);//a.UserNickName
        var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        var someValue = Expression.Constant(propertyValue, typeof(string));
        var containsMethodExp = Expression.Call(propertyExp, method, someValue);//a.UserNickName.StartsWith(xx);
        return Expression.Lambda<Func<TDbEntity, bool>>(containsMethodExp, parameterExp);
    }

    #endregion

    #region AddLikeRight

    ///
    public static List<Expression<Func<TDbEntity, bool>>> AddLikeRight<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();
        foreach (var prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
            if (ContinueAdd(that, prop, value))
            {
                continue;
            }
            if (value is string valueStr && !string.IsNullOrWhiteSpace(valueStr))
            {
                var exp = QueryConfigHelper.GetExpression_EndsWith<TDbEntity>(prop, valueStr);
                if (exp != null)
                {
                    whereLambdas.Add(exp);
                }
            }
        }
        return whereLambdas;
    }

    // t.SomeProperty.EndsWith("stringValue");
    private static Expression<Func<TDbEntity, bool>> GetExpression_EndsWith<TDbEntity>(string propertyName, string propertyValue)
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            return null;
        }

        var parameterExp = Expression.Parameter(type_TEntity, "a");
        var propertyExp = Expression.Property(parameterExp, propertyName);//a.UserNickName
        var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        var someValue = Expression.Constant(propertyValue, typeof(string));
        var containsMethodExp = Expression.Call(propertyExp, method, someValue);//a.UserNickName.EndsWith(xx);
        return Expression.Lambda<Func<TDbEntity, bool>>(containsMethodExp, parameterExp);
    }

    #endregion

    #region AddEqual版本2 : 根据: AddInOrEuqal 衍生出来的

    public static List<Expression<Func<TDbEntity, bool>>> AddEqual<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();
        //todo:这里可以获得 TSearch 的属性的 Attribute
        foreach (var prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);

            if (ContinueAdd(that, prop, value))
            {
                continue;
            }
            AddEqualCore<TSearch, TDbEntity>(prop, valuePropType, value, whereLambdas);
        }

        return whereLambdas;
    }

    //核心-equal
    private static void AddEqualCore<TSearch, TDbEntity>(string propertyName, Type valuePropType, object propertyValue, List<Expression<Func<TDbEntity, bool>>> whereLambdas)
        where TSearch : class
        where TDbEntity : class
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            return;//TSearch 和 TDbEntity 都要有 相同的 propertyName
        }

        // AddEqual 的代码
        var parameterExp = Expression.Parameter(type_TEntity, "a");
        var propertyExp = Expression.Property(parameterExp, propertyName); //a.AuditStateId
        Type propType_TEntity = propertyExp.Type; //a.AuditStateId 的type(Dto中定义的属性类型)

        var attr_SqlFuncArray = ReflectionHelper.GetAttributeForProperty<SqlFuncAttribute>(typeof(TSearch).GetProperty(propertyName), true);
        var haveAttr_SqlFunc = attr_SqlFuncArray.Length > 0;
        if (haveAttr_SqlFunc)
        {
            if (attr_SqlFuncArray.Length > 1)
            {
                throw new FrameException($"特性:{nameof(SqlFuncAttribute)}不能标记多个.");
            }

            var attr_SqlFunc = attr_SqlFuncArray[0];
            if (attr_SqlFunc is MonthAttribute)
            {
                //GetLambda_MonthAttribute_Equal

                ParameterExpression parameterExpression = Expression.Parameter(type_TEntity, "u");
                //                  DbFunctions 的          Month(                      DateTime dt)
                var leftP2 = typeof(DbFunctions).GetMethod("Month", new Type[] { typeof(DateTime) }); //SqlFunc.DbFunctions.Month(DateTime dt)
                                                                                                      //  type_TEntity 的 propertyName 的 类型 必须 是   DateTime
                var leftP3 = new Expression[] { Expression.Property(parameterExpression, propertyName) }; // u.CreateAt
                var left = Expression.Call(null, leftP2, leftP3); //SqlFunc.DbFunctions.Month(u.CreateAt)

                //Sql 的 month() 返回的是int
                //https://docs.microsoft.com/zh-cn/sql/t-sql/functions/month-transact-sql?view=sql-server-2017
                var right = Expression.Constant(Convert.ToInt32(propertyValue), typeof(int));
                var body = Expression.Equal(left, right);
                var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExpression);
                whereLambdas.Add(lambda);
            }
        }
        else
        {
            var left = Expression.Convert(propertyExp, propType_TEntity);
            //var right = Expression.Constant(propertyValue, propertyValue.GetType());//ids这种情况就有问题
            //前半个if 是为了支持ids的查询: 当id需要支持多个值查询时, 前端模型只能是string类型, 然后这里就会因为类型不一致而发生异常
            if (valuePropType != propType_TEntity || propType_TEntity != typeof(string))
            {
                //解决 "" 转成 值类型抛出转换失败异常
                if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
                {
                    return;
                }
                propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);
            }

            var right = Expression.Constant(propertyValue, propType_TEntity);

            var body = Expression.Equal(left, right);
            var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExp);
            whereLambdas.Add(lambda);
        }
    }

    #endregion

    #region AddNotEqual-基于Equal的版本2

    public static List<Expression<Func<TDbEntity, bool>>> AddNotEqual<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();
        foreach (var prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
            if (ContinueAdd(that, prop, value))
            {
                continue;
            }
            AddNotEqualCore<TSearch, TDbEntity>(prop, valuePropType, value, whereLambdas);
        }
        return whereLambdas;
    }

    //核心
    private static void AddNotEqualCore<TSearch, TDbEntity>(string propertyName, Type valuePropType, object propertyValue, List<Expression<Func<TDbEntity, bool>>> whereLambdas)
        where TSearch : class
        where TDbEntity : class
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            return;
        }

        // AddEqual 的代码
        var parameterExp = Expression.Parameter(type_TEntity, "a");
        var propertyExp = Expression.Property(parameterExp, propertyName); //a.AuditStateId
        Type propType_TEntity = propertyExp.Type; //a.AuditStateId 的type(Dto中定义的属性类型)

        var attr_SqlFuncArray = ReflectionHelper.GetAttributeForProperty<SqlFuncAttribute>(typeof(TSearch).GetProperty(propertyName), true);
        var haveAttr_SqlFunc = attr_SqlFuncArray.Length > 0;
        if (haveAttr_SqlFunc)
        {
            if (attr_SqlFuncArray.Length > 1)
            {
                throw new FrameException($"特性:{nameof(SqlFuncAttribute)}不能标记多个.");
            }
            var attr_SqlFunc = attr_SqlFuncArray[0];
            if (attr_SqlFunc is MonthAttribute)
            {
                ParameterExpression parameterExpression = Expression.Parameter(type_TEntity, "u");
                var leftP2 = typeof(DbFunctions).GetMethod("Month", new Type[] { typeof(DateTime) });
                var leftP3 = new Expression[] { Expression.Property(parameterExpression, propertyName) };
                var left = Expression.Call(null, leftP2, leftP3);
                //Sql 的 month() 返回的是int
                //https://docs.microsoft.com/zh-cn/sql/t-sql/functions/month-transact-sql?view=sql-server-2017
                var right = Expression.Constant(Convert.ToInt32(propertyValue), typeof(int));
                var body = Expression.NotEqual(left, right);
                var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExpression);
                whereLambdas.Add(lambda);
            }
        }
        else
        {
            var left = Expression.Convert(propertyExp, propType_TEntity);
            //var right = Expression.Constant(propertyValue, propertyValue.GetType());//ids这种情况就有问题
            //前半个if 是为了支持ids的查询: 当id需要支持多个值查询时, 前端模型只能是string类型, 然后这里就会因为类型不一致而发生异常
            if (valuePropType != propType_TEntity || propType_TEntity != typeof(string))
            {
                //解决 "" 转成 值类型抛出转换失败异常
                if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
                {
                    return;
                }

                propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);
            }

            var right = Expression.Constant(propertyValue, propType_TEntity);

            var body = Expression.NotEqual(left, right);
            var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExp);
            whereLambdas.Add(lambda);
        }
    }

    #endregion

    #region AddNumberRange

    internal class NumberSearch
    {
        public string Prop { get; set; }

        public object[] NumberRange { get; set; }

        public bool? IsPair { get; set; }
    }

    public static List<Expression<Func<TDbEntity, bool>>> AddNumberRange<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }
        var type_TEntity = typeof(TDbEntity);

        //key : 字段 value : 开始值(包含), 结束值(包含)
        var numberDict = new Dictionary<string, NumberSearch>();

        #region 初始化 timeDict

        foreach (var prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);

            if (ContinueAdd(that, prop, value))
            {
                continue;
            }

            if (!Regex.IsMatch(value.ToString(), @"^(-?\d+)(\.\d+)?$")) //是否是数字(整数+浮点数)
            {
                throw new FrameException($"当前值({value})不是数字类型.");
            }

            if (prop != "Left" && prop.EndsWith("Left"))
            {
                var key = prop.RemoveSuffix("Left");
                if (!numberDict.ContainsKey(key))
                {
                    numberDict.Add(key, new NumberSearch() { Prop = key, NumberRange = new Object[2] });
                }

                numberDict[key].NumberRange[0] = value;
                if (numberDict[key].IsPair == null)
                {
                    numberDict[key].IsPair = true;
                }
                else
                {
                    if (numberDict[key].IsPair != true)
                    {
                        throw new FrameException("重复赋值");
                    }
                }
            }
            else if (prop != "Right" && prop.EndsWith("Right"))
            {
                var key = prop.RemoveSuffix("Right");
                if (!numberDict.ContainsKey(key))
                {
                    numberDict.Add(key, new NumberSearch() { Prop = key, NumberRange = new Object[2] });
                }
                numberDict[key].NumberRange[1] = value;
                if (numberDict[key].IsPair == null)
                {
                    numberDict[key].IsPair = true;
                }
                else
                {
                    if (numberDict[key].IsPair != true)
                    {
                        throw new FrameException("重复赋值");
                    }
                }
            }
            else
            {
                var key = prop;
                if (!numberDict.ContainsKey(key))
                {
                    numberDict.Add(key, new NumberSearch() { Prop = key, NumberRange = new Object[2] });
                }

                numberDict[key].NumberRange[0] = value;

                if (numberDict[key].IsPair == null)
                {
                    numberDict[key].IsPair = false;
                }
                else
                {
                    throw new FrameException("重复赋值");
                }
            }
        }

        #endregion

        #region 删除无效key

        var removeKey = new List<string>();
        foreach (var key in numberDict.Keys)
        {
            var times = numberDict[key];
            if (times.NumberRange[0] == null && times.NumberRange[1] == null)
            {
                removeKey.Add(key);
                continue;
            }
        }

        foreach (var key in removeKey)
        {
            numberDict.Remove(key);
        }

        #endregion

        foreach (var key in numberDict.Keys)
        {
            var numbers = numberDict[key];
            if (numbers.IsPair == false)
            {
                if (numbers.NumberRange[0] != null)
                {
                    numbers.NumberRange[1] = numbers.NumberRange[0];
                }
            }
            else
            {
                if (numbers.NumberRange[0] != null && numbers.NumberRange[1] != null)
                {
                    bool needSwap = (dynamic)numbers.NumberRange[0] > (dynamic)numbers.NumberRange[1]; //比较大小,小在放前面
                    if (needSwap)
                    {
                        (numbers.NumberRange[1], numbers.NumberRange[0]) = (numbers.NumberRange[0], numbers.NumberRange[1]);
                    }
                }
            }
        }

        //根据 NumberRange  创建表达式(一共是下面4钟情况)
        //isPair值    开始         结束
        //false      有值(包含)     有值(含)
        //true       有值(包含)     有值(含)
        //true       有值(包含)     无值
        //true       无值          有值(含)

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();

        foreach (var key in numberDict.Keys)
        {
            object d1 = null;//>=
            object d2 = null;//<=
            var ispair = (bool)numberDict[key].IsPair;

            if (ispair == false && numberDict[key].NumberRange[0] != null && numberDict[key].NumberRange[1] != null)
            {
                d1 = numberDict[key].NumberRange[0];
                d2 = numberDict[key].NumberRange[1];
            }
            else if (ispair == true && numberDict[key].NumberRange[0] != null && numberDict[key].NumberRange[1] != null)
            {
                d1 = numberDict[key].NumberRange[0];
                d2 = numberDict[key].NumberRange[1];
            }
            else if (ispair == true && numberDict[key].NumberRange[0] != null && numberDict[key].NumberRange[1] == null)
            {
                d1 = numberDict[key].NumberRange[0];
            }
            else if (ispair == true && numberDict[key].NumberRange[0] == null && numberDict[key].NumberRange[1] != null)
            {
                d2 = numberDict[key].NumberRange[1];
            }
            else
            {
                throw new FrameException("代码逻辑有问题, 不应该进入这个分支");
            }

            if (d1 != null) //>=
            {
                if (type_TEntity.GetProperty(key) != null)
                {
                    var parameterExp = Expression.Parameter(type_TEntity, "a");
                    var propertyExp = Expression.Property(parameterExp, key);//a.CreateAt
                    Type propType_TEntity = propertyExp.Type; //a.AuditStateId 的type(Dto中定义的属性类型)
                    var left = Expression.Convert(propertyExp, propType_TEntity);
                    var right = Expression.Constant(numberDict[key].NumberRange[0], propType_TEntity);
                    var body = Expression.GreaterThanOrEqual(left, right);
                    var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExp);
                    whereLambdas.Add(lambda);
                }
            }

            if (d2 != null) //<=
            {
                if (type_TEntity.GetProperty(key) != null)
                {
                    var parameterExp = Expression.Parameter(type_TEntity, "a");
                    var propertyExp = Expression.Property(parameterExp, key); //a.CreateAt
                    Type propType_TEntity = propertyExp.Type; //a.AuditStateId 的type(Dto中定义的属性类型)
                    var left = Expression.Convert(propertyExp, propType_TEntity);
                    var right = Expression.Constant(numberDict[key].NumberRange[1], propType_TEntity);
                    var body = Expression.LessThanOrEqual(left, right);
                    var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExp);
                    whereLambdas.Add(lambda);
                }
            }
        }
        return whereLambdas;
    }

    #endregion

    #region AddTimeRange

    public static List<Expression<Func<TDbEntity, bool>>> AddTimeRange<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        var timeDict = AddTimeRange_GetTimeDict(that, searchCondition);

        Get_TimePeriode(timeDict, null);

        var whereLambdas = AddTimeRange_GetWhereLambdas<TDbEntity>(timeDict);
        return whereLambdas;
    }

    #region 指定 period 的

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TSearch"></typeparam>
    /// <typeparam name="TDbEntity"></typeparam>
    /// <param name="that"></param>
    /// <param name="searchCondition"></param>
    /// <param name="period">当为秒的时候需要调用这个方法</param>
    /// <returns></returns>
    public static List<Expression<Func<TDbEntity, bool>>> AddTimeRange<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition, TimePeriod period)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        var timeDict = AddTimeRange_GetTimeDict(that, searchCondition);

        Get_TimePeriode(timeDict, () => period);

        var whereLambdas = AddTimeRange_GetWhereLambdas<TDbEntity>(timeDict);
        return whereLambdas;
    }

    #endregion

    private static Dictionary<string, TimeSearch> AddTimeRange_GetTimeDict<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        //key : 字段 value : 开始时间(包含), 结束时间(不含)
        var timeDict = new Dictionary<string, TimeSearch>();

        #region 初始化 timeDict

        foreach (var prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);

            if (ContinueAdd(that, prop, value))
            {
                continue;
            }
            if (value is not DateTime)
            {
                throw new FrameException("当前值不是 datetime 类型");
            }

            if (prop != "Start" && prop.EndsWith("Start"))
            {
                var key = prop.RemoveSuffix("Start");
                if (!timeDict.ContainsKey(key))
                {
                    timeDict.Add(key, new TimeSearch() { Prop = key, StartAndEnd = new DateTime?[2] });
                }

                timeDict[key].StartAndEnd[0] = (DateTime?)value;
                if (timeDict[key].IsPair == null)
                {
                    timeDict[key].IsPair = true;
                }
                else
                {
                    if (timeDict[key].IsPair != true)
                    {
                        throw new FrameException("重复赋值");
                    }
                }
            }
            else if (prop != "End" && prop.EndsWith("End"))
            {
                var key = prop.RemoveSuffix("End");
                if (!timeDict.ContainsKey(key))
                {
                    timeDict.Add(key, new TimeSearch() { Prop = key, StartAndEnd = new DateTime?[2] });
                }

                timeDict[key].StartAndEnd[1] = (DateTime?)value;
                if (timeDict[key].IsPair == null)
                {
                    timeDict[key].IsPair = true;
                }
                else
                {
                    if (timeDict[key].IsPair != true)
                    {
                        throw new FrameException("重复赋值");
                    }
                }
            }
            else
            {
                var key = prop;
                if (!timeDict.ContainsKey(key))
                {
                    timeDict.Add(key, new TimeSearch() { Prop = key, StartAndEnd = new DateTime?[2] });
                }
                timeDict[key].StartAndEnd[0] = (DateTime?)value;
                if (timeDict[key].IsPair == null)
                {
                    timeDict[key].IsPair = false;
                }
                else
                {
                    throw new FrameException("重复赋值");
                }
            }
        }

        #endregion

        #region 删除无效key

        var removeKey = new List<string>();
        foreach (var key in timeDict.Keys)
        {
            var times = timeDict[key];

            if (times.StartAndEnd[0] == null && times.StartAndEnd[1] == null)
            {
                removeKey.Add(key);
                continue;
            }
        }

        foreach (var key in removeKey)
        {
            timeDict.Remove(key);
        }

        #endregion

        return timeDict;
    }

    /// <summary>
    /// 处理时间精度
    /// </summary>
    /// <param name="timeDict"></param>
    /// <param name="getTimePeriodFunc"></param>
    private static void Get_TimePeriode(Dictionary<string, TimeSearch> timeDict, Func<TimePeriod> getTimePeriodFunc)
    {
        var 主动查询时间周期 = getTimePeriodFunc == null;
        TimePeriod? fixedTimePeriod = null;
        if (!主动查询时间周期)
        {
            fixedTimePeriod = getTimePeriodFunc.Invoke();
        }
        foreach (var key in timeDict.Keys)
        {
            var times = timeDict[key];
            if (times.IsPair == false)
            {
                if (times.StartAndEnd[0] != null)
                {
                    DateTime time = times.StartAndEnd[0].Value;
                    var range = 主动查询时间周期 ? 获得查询的时间精度(time) : fixedTimePeriod.Value;
                    times.StartAndEnd[1] = GetEndTime(range, time);
                }
            }
            else
            {
                if (times.StartAndEnd[0] != null && times.StartAndEnd[1] != null) //都有值 或 start值
                {
                    DateTime d1 = times.StartAndEnd[0].Value;
                    DateTime d2 = times.StartAndEnd[1].Value;

                    //SwopUtil.SwopAsc(ref d1, ref d2);

                    if (Comparer<DateTime>.Default.Compare(d1, d2) > 0)
                    {
                        (d2, d1) = (d1, d2);
                    }

                    if (d1 == d2)//效果等价于 只有一个字段 给查询的时间范围
                    {
                        var period = 主动查询时间周期 ? 获得查询的时间精度(d1) : fixedTimePeriod.Value;
                        d2 = GetEndTime(period, d1);
                    }
                    else
                    {
                        var period = 主动查询时间周期 ? 获得查询的时间精度(d2) : fixedTimePeriod.Value;
                        d2 = GetEndTime(period, d2);
                    }

                    times.StartAndEnd[0] = d1;
                    times.StartAndEnd[1] = d2;
                }
                else if (times.StartAndEnd[0] == null && times.StartAndEnd[1] != null)//只有 end 有值
                {
                    var endTime = (DateTime)times.StartAndEnd[1];
                    var period = 主动查询时间周期 ? 获得查询的时间精度(endTime) : fixedTimePeriod.Value;
                    var newEndTime = GetEndTime((TimePeriod)period, endTime);
                    times.StartAndEnd[1] = newEndTime;
                }
            }
        }
    }

    private static List<Expression<Func<TDbEntity, bool>>> AddTimeRange_GetWhereLambdas<TDbEntity>(Dictionary<string, TimeSearch> timeDict)
        where TDbEntity : class
    {
        var type_TEntity = typeof(TDbEntity);
        //根据 TimePeriod 创建表达式(一共是下面4钟情况)
        //isPair值    开始         结束
        //false      有值(包含)     有值(不含)
        //true       有值(包含)     有值(不含)
        //true       有值(包含)     无值
        //true       无值          有值(不含)

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();

        foreach (var key in timeDict.Keys)
        {
            DateTime? d1 = null;//>=
            DateTime? d2 = null;//<
            var ispair = (bool)timeDict[key].IsPair;

            if (ispair == false && timeDict[key].StartAndEnd[0] != null && timeDict[key].StartAndEnd[1] != null)
            {
                d1 = timeDict[key].StartAndEnd[0];
                d2 = timeDict[key].StartAndEnd[1];
            }
            else if (ispair == true && timeDict[key].StartAndEnd[0] != null && timeDict[key].StartAndEnd[1] != null)
            {
                d1 = timeDict[key].StartAndEnd[0];
                d2 = timeDict[key].StartAndEnd[1];
            }
            else if (ispair == true && timeDict[key].StartAndEnd[0] != null && timeDict[key].StartAndEnd[1] == null)
            {
                d1 = timeDict[key].StartAndEnd[0];
            }
            else if (ispair == true && timeDict[key].StartAndEnd[0] == null && timeDict[key].StartAndEnd[1] != null)
            {
                d2 = timeDict[key].StartAndEnd[1];
            }
            else
            {
                throw new FrameException("代码逻辑有问题, 不应该进入这个分支");
            }

            if (d1 != null) //>=
            {
                if (type_TEntity.GetProperty(key) != null)
                {
                    var parameterExp = Expression.Parameter(type_TEntity, "a");
                    var propertyExp = Expression.Property(parameterExp, key); //a.CreateAt
                    var left = Expression.Convert(propertyExp, typeof(DateTime));
                    var right = Expression.Constant(timeDict[key].StartAndEnd[0], typeof(DateTime));
                    var body = Expression.GreaterThanOrEqual(left, right);
                    var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExp);
                    whereLambdas.Add(lambda);
                }
            }

            if (d2 != null) //<
            {
                if (type_TEntity.GetProperty(key) != null)
                {
                    var parameterExp = Expression.Parameter(type_TEntity, "a");
                    var propertyExp = Expression.Property(parameterExp, key); //a.CreateAt
                    var left = Expression.Convert(propertyExp, typeof(DateTime));
                    var right = Expression.Constant(timeDict[key].StartAndEnd[1], typeof(DateTime));
                    var body = Expression.LessThan(left, right);
                    var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExp);
                    whereLambdas.Add(lambda);
                }
            }
        }

        return whereLambdas;
    }

    private static DateTime GetEndTime(TimePeriod period, DateTime time)
    {
        if (period == TimePeriod.Day)
        {
            return new DateTime(time.Year, time.Month, time.Day, 0, 0, 0).AddDays(1);
        }
        if (period == TimePeriod.Hour)
        {
            return new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0).AddHours(1);
        }
        if (period == TimePeriod.Minute)
        {
            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0).AddMinutes(1);
        }
        if (period == TimePeriod.Second)
        {
            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Minute).AddSeconds(1);
        }

        throw new FrameException("后续开发修改过代码逻辑, 但是此处却未修改,需要修改代码");
    }

    /// <summary>
    /// 目前该方法的唯一问题: 时间精度为 Second 时, 这个方法并不能返回你要的精度
    /// d1:2022-1-1 9:0:0
    /// d2:2022-1-1 9:1:0  此时 返回的  精度是 Minute
    /// d1:2022-1-1 0:0:0
    /// d2:2022-1-1 1:0:0  此时 返回的  精度是 Hour
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private static TimePeriod 获得查询的时间精度(DateTime time)
    {
        if (time.Hour == 0)
        {
            if (time.Minute == 0)
            {
                if (time.Second == 0)
                {
                    return TimePeriod.Day;
                }
            }
        }
        else
        {
            if (time.Minute == 0)
            {
                if (time.Second == 0)
                {
                    return TimePeriod.Hour;
                }
            }
            else
            {
                if (time.Second == 0)
                {
                    return TimePeriod.Minute;
                }
                else
                {
                    return TimePeriod.Second;
                }
            }
        }

        throw new Exception("此片段逻辑有误,需要修改代码");
    }

    #endregion

    #region AddIn

    /// <summary>
    /// 实际翻译成in 还是 Euqal , 根据 split() 后的个数而定
    /// </summary>
    /// <typeparam name="TSearch"></typeparam>
    /// <typeparam name="TDbEntity"></typeparam>
    /// <param name="that"></param>
    /// <param name="propertyNames"></param>
    /// <returns></returns>
    /// <exception cref="FrameException"></exception>
    public static List<Expression<Func<TDbEntity, bool>>> AddIn<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> propertyNames)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(propertyNames))
        {
            return Default<TDbEntity>();
        }
        var type_TEntity = typeof(TDbEntity);

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();
        foreach (var propertyName in propertyNames)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(propertyName);
            if (ContinueAdd(that, propertyName, value))
            {
                continue;
            }

            //注释原因:开始是单个搜索,后来改成多个搜索的,然后只要修改Model类型(不用修改其他代码)的情况
            //if (value.GetType() != typeof(string))
            //{
            //    throw new ArgumentException("当前属性不是string,无法使用Split()函数");
            //}

            IEnumerable<string> splits;
            if (value is string s)
            {
                splits = s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct();
            }
            else if (value.GetType().IsObjectCollection())
            {
                var list = new List<string>();
                ForeachHelper.Each(value, a =>
                {
                    list.Add(a.ToString());
                });
                splits = list;
            }
            else
            {
                splits = value.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct();
            }

            if (!splits.Any())
            {
                continue;
            }

            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                continue;
            }

            var parameterExp = Expression.Parameter(type_TEntity, "a");//pe
            var propertyExp = Expression.Property(parameterExp, propertyName); //a.AuditStateId  //me

            Type propType_TEntity = propertyExp.Type; //a.AuditStateId 的type (Dto中定义的属性类型)

            //IEnumerable<T> 的T 必需和  propType_TEntity 一样
            //string + 整数
            //string    ushort    short    int    uint    char    float    double    long    ulong    decimal   datetime
            //1          1          1       1       1      1         1       1         1       1         1        0

            Expression<Func<TDbEntity, bool>> lambda = null;
            SqlFuncAttribute[] attr_SqlFuncArray = ReflectionHelper.GetAttributeForProperty<SqlFuncAttribute>(typeof(TSearch).GetProperty(propertyName), true);
            var haveAttr_SqlFunc = attr_SqlFuncArray.Length > 0;
            if (haveAttr_SqlFunc)
            {
                if (attr_SqlFuncArray.Length > 1)
                {
                    var exMsg = $"特性:{nameof(SqlFuncAttribute)}不能标记多个.";
                    throw new FrameException(exMsg);
                }

                var attr_SqlFunc = attr_SqlFuncArray[0];
                if (attr_SqlFunc is MonthInAttribute)
                {
                    #region 不要问怎么写的, 问就是反编译 代码抄的

                    //反编译代码: Expression<Func<User_SqlFunc_Entity, bool>> expression = u => SqlFunc.DbFunctions.MonthIn(u.CreateAt) == new List<int> { 5, 6 };

                    ParameterExpression parameterExpression = Expression.Parameter(type_TEntity, "u");
                    var leftP2 = typeof(DbFunctions).GetMethod("MonthIn", new Type[] { typeof(DateTime) });
                    var leftP3 = new Expression[]
                    {
                        Expression.Property(parameterExpression, ReflectionHelper.GetMethod(type_TEntity, "get_" + propertyName))
                    };
                    var left = Expression.Call(null, leftP2, leftP3);

                    var right_Para = new System.Linq.Expressions.ElementInit[splits.Count()];

                    var index = 0;
                    foreach (var val in splits.ToInt32())
                    {
                        right_Para[index] = Expression.ElementInit(ReflectionHelper.GetMethod(typeof(List<int>), "Add"), new Expression[]
                        {
                            Expression.Constant(val, typeof(int))
                        });

                        index++;
                    }
                    ListInitExpression right = Expression.ListInit(Expression.New(typeof(List<int>)), right_Para);

                    var body = Expression.Equal(left, right);

                    lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExpression);

                    #endregion
                }
            }

            if (lambda == null)
            {
                if (propType_TEntity == typeof(string))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits);
                }

                #region 值类型

                else if (propType_TEntity == typeof(bool))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToBool());
                }
                else if (propType_TEntity == typeof(byte))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToInt8());
                }
                else if (propType_TEntity == typeof(sbyte))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToUInt8());
                }
                else if (propType_TEntity == typeof(short))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToInt16());
                }
                else if (propType_TEntity == typeof(ushort))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToUInt16());
                }
                else if (propType_TEntity == typeof(int))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToInt32());
                }
                else if (propType_TEntity == typeof(uint))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToUInt32());
                }
                else if (propType_TEntity == typeof(long))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToInt64());
                }
                else if (propType_TEntity == typeof(ulong))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToUInt64());
                }
                else if (propType_TEntity == typeof(float))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToFloat());
                }
                else if (propType_TEntity == typeof(double))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToDouble());
                }
                else if (propType_TEntity == typeof(decimal))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToDecimal());
                }
                else if (propType_TEntity == typeof(char))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToChar());
                }

                #endregion

                #region 可空值类型

                else if (propType_TEntity == typeof(bool?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableBool());
                }
                else if (propType_TEntity == typeof(byte?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableInt8());
                }
                else if (propType_TEntity == typeof(sbyte?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToUInt8());
                }
                else if (propType_TEntity == typeof(short?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableInt16());
                }
                else if (propType_TEntity == typeof(ushort?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableUInt16());
                }
                else if (propType_TEntity == typeof(int?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableInt32());
                }
                else if (propType_TEntity == typeof(uint?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableUInt32());
                }
                else if (propType_TEntity == typeof(long?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableInt64());
                }
                else if (propType_TEntity == typeof(ulong?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableUInt64());
                }
                else if (propType_TEntity == typeof(float?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableFloat());
                }
                else if (propType_TEntity == typeof(double?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableDouble());
                }
                else if (propType_TEntity == typeof(decimal?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableDecimal());
                }
                else if (propType_TEntity == typeof(char?))
                {
                    lambda = GetExpression_In<TDbEntity>(parameterExp, propertyExp, splits.ToNullableChar());
                }

                #endregion
            }

            if (lambda == null)
            {
                var exMsg = $"SearchType:{nameof(SearchType.In)},操作遇到不支持的属性类型:{propType_TEntity}";
                throw new FrameException(exMsg);
            }

            whereLambdas.Add(lambda);
        }
        return whereLambdas;
    }

    private static Expression<Func<TDbEntity, bool>> GetExpression_In<TDbEntity>(ParameterExpression parameterExp, MemberExpression propertyExp, object listObj)
        where TDbEntity : class
    {
        //参考
        //https://stackoverflow.com/questions/18491610/dynamic-linq-expression-for-ienumerableint-containsmemberexpression
        //https://stackoverflow.com/questions/26659824/create-a-predicate-builder-for-x-listofints-containsx-listofintstocheck

        //ParameterExpression parameterExp = Expression.Parameter(type_TEntity, "a");
        //MemberExpression propertyExp = Expression.Property(parameterExp, propertyName); //a.AuditStateId

        //var someValue = Expression.Constant(listObj, typeof(List<Int16?>));
        ConstantExpression someValue = Expression.Constant(listObj);

        //会生产 sql  CAST( `t`.`audit_state_id` AS int ) , 但是 mysql(5.7.32)  int 要翻译成改成 signed 不然提示sql语法有误
        //var convertExpression = Expression.Convert(propertyExp, typeof(Int32));
        //var call = Expression.Call(someValue, "Contains", new Type[] { }, convertExpression);

        //var method = typeof(Enumerable).GetMethod("Contains");//会报错 ,说模棱两可的
        //var method = typeof(IEnumerable<Int32>).GetMethod("Contains");//没有这个方法
        //var method = typeof(List<Int32?>).GetMethod("Contains"); //List<Int32?> 要和  a.AuditStateId 的类型是一样的

        //var call = Expression.Call(someValue, method, propertyExp); //这行代码是可以跑的.

        var call = Expression.Call(typeof(Enumerable), "Contains", new[] { propertyExp.Type }, someValue, propertyExp);

        //containsMethodExp
        var lambda = Expression.Lambda<Func<TDbEntity, bool>>(call, parameterExp);
        //“动态不包含”为：
        // var lambda = Expression.Lambda<Func<TDbEntity, bool>>(Expression.Not(call), pe);
        return lambda;
    }

    #endregion

    #region AddGt

    public static List<Expression<Func<TDbEntity, bool>>> AddGt<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();

        var type_TEntity = typeof(TDbEntity);
        foreach (string prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
            if (ContinueAdd(that, prop, value) || type_TEntity.GetProperty(prop) == null)
            {
                continue;
            }
            if (!valuePropType.IsClass)
            {
                var exp = QueryConfigHelper.GetExpression_gt<TDbEntity>(prop, value);
                if (exp != null)
                {
                    whereLambdas.Add(exp);
                }
            }
        }

        return whereLambdas;
    }

    // t.SomeProperty > 5
    private static Expression<Func<TDbEntity, bool>> GetExpression_gt<TDbEntity>(string propertyName, object propertyValue)
        where TDbEntity : class
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            return null;
        }

        var parameterExp = Expression.Parameter(type_TEntity, "a");
        var propertyExp = Expression.Property(parameterExp, propertyName); //a.Id
        Type propType_TEntity = propertyExp.Type; // domain(typeof(TEntity)) 中的Id属性的类型
        var left = Expression.Convert(propertyExp, propType_TEntity);

        //解决 "" 转成 值类型抛出转换失败异常
        if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
        {
            return null;
        }

        propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);

        var right = Expression.Constant(propertyValue, propType_TEntity);
        var body = Expression.GreaterThan(left, right);
        var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExp);
        return lambda;
    }

    #endregion

    #region AddGe

    public static List<Expression<Func<TDbEntity, bool>>> AddGe<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TDbEntity : class
        where TSearch : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();
        var type_TEntity = typeof(TDbEntity);
        foreach (string prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
            if (ContinueAdd(that, prop, value) || type_TEntity.GetProperty(prop) == null)
            {
                continue;
            }
            if (!valuePropType.IsClass)
            {
                var exp = QueryConfigHelper.GetExpression_ge<TDbEntity>(prop, value);
                if (exp != null)
                {
                    whereLambdas.Add(exp);
                }
            }
        }

        return whereLambdas;
    }

    // t.SomeProperty >= 5
    private static Expression<Func<TDbEntity, bool>> GetExpression_ge<TDbEntity>(string propertyName, object propertyValue)
        where TDbEntity : class
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            return null;
        }

        var parameterExp = Expression.Parameter(type_TEntity, "a");
        var propertyExp = Expression.Property(parameterExp, propertyName); //a.Id
        Type propType_TEntity = propertyExp.Type; // domain(typeof(TEntity)) 中的Id属性的类型
        var left = Expression.Convert(propertyExp, propType_TEntity);

        //解决 "" 转成 值类型抛出转换失败异常
        if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
        {
            return null;
        }

        propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);

        var right = Expression.Constant(propertyValue, propType_TEntity);
        var body = Expression.GreaterThanOrEqual(left, right);
        var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExp);
        return lambda;
    }

    #endregion

    #region AddLt

    public static List<Expression<Func<TDbEntity, bool>>> AddLt<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();
        var type_TEntity = typeof(TDbEntity);
        foreach (string prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
            if (ContinueAdd(that, prop, value) || type_TEntity.GetProperty(prop) == null)
            {
                continue;
            }
            if (!valuePropType.IsClass)
            {
                var exp = QueryConfigHelper.GetExpression_lt<TDbEntity>(prop, value);
                if (exp != null)
                {
                    whereLambdas.Add(exp);
                }
            }
        }

        return whereLambdas;
    }

    // t.SomeProperty < 5
    private static Expression<Func<TDbEntity, bool>> GetExpression_lt<TDbEntity>(string propertyName, object propertyValue)
        where TDbEntity : class
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            return null;
        }

        var parameterExp = Expression.Parameter(type_TEntity, "a");
        var propertyExp = Expression.Property(parameterExp, propertyName); //a.Id
        Type propType_TEntity = propertyExp.Type; // domain(typeof(TEntity)) 中的Id属性的类型
        var left = Expression.Convert(propertyExp, propType_TEntity);

        //解决 "" 转成 值类型抛出转换失败异常
        if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
        {
            return null;
        }

        propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);

        var right = Expression.Constant(propertyValue, propType_TEntity);
        var body = Expression.LessThan(left, right);
        var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExp);
        return lambda;
    }

    #endregion

    #region AddLe

    public static List<Expression<Func<TDbEntity, bool>>> AddLe<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, List<string> searchCondition)
        where TSearch : class
        where TDbEntity : class
    {
        if (!HaveCount(searchCondition))
        {
            return Default<TDbEntity>();
        }

        List<Expression<Func<TDbEntity, bool>>> whereLambdas = new();
        var type_TEntity = typeof(TDbEntity);
        foreach (string prop in searchCondition)
        {
            var propertyValue = new PropertyValue<TSearch>(that.Search);
            (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
            if (ContinueAdd(that, prop, value) || type_TEntity.GetProperty(prop) == null)
            {
                continue;
            }
            if (!valuePropType.IsClass)
            {
                var exp = QueryConfigHelper.GetExpression_le<TDbEntity>(prop, value);
                if (exp != null)
                {
                    whereLambdas.Add(exp);
                }
            }
        }

        return whereLambdas;
    }

    // t.SomeProperty <= 5
    private static Expression<Func<TDbEntity, bool>> GetExpression_le<TDbEntity>(string propertyName, object propertyValue)
        where TDbEntity : class
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            return null;
        }

        var parameterExp = Expression.Parameter(type_TEntity, "a");
        var propertyExp = Expression.Property(parameterExp, propertyName); //a.Id
        Type propType_TEntity = propertyExp.Type; // domain(typeof(TEntity)) 中的Id属性的类型
        var left = Expression.Convert(propertyExp, propType_TEntity);

        //解决 "" 转成 值类型抛出转换失败异常
        if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
        {
            return null;
        }

        propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);

        var right = Expression.Constant(propertyValue, propType_TEntity);
        var body = Expression.LessThanOrEqual(left, right);
        var lambda = Expression.Lambda<Func<TDbEntity, bool>>(body, parameterExp);
        return lambda;
    }

    #endregion

    #region WhereHasValue/WhereNoValue

    /// <summary>
    /// 当前字段有值, 字符串类型: IsNull(字段,"") != ""
    /// </summary>
    /// <typeparam name="TDbEntity"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Expression<Func<TDbEntity, bool>> GetExpression_HasValue<TDbEntity>(string propertyName)
        where TDbEntity : class
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            throw new Exception($"{propertyName}不在{typeof(TDbEntity).FullName}类型中");
        }

        if (prop_Info.PropertyType != typeof(string))
        {
            throw new Exception($"{propertyName}不在{typeof(TDbEntity).FullName}类型中只能为string类型");
        }

        var parameterExp = Expression.Parameter(type_TEntity, "a");
        var left = Expression.MakeBinary(
            ExpressionType.NotEqual,
            Expression.Coalesce(
                Expression.MakeMemberAccess(parameterExp, prop_Info),
                Expression.Constant("", typeof(string))
            ),
            Expression.Constant("")
        //dnspy 反编译没有这个2个, ExpressionTreeToString有
        //,false
        //,typeof(string).GetMethod("op_Inequality")
        );
        var lambda = Expression.Lambda<Func<TDbEntity, bool>>(left, parameterExp);
        return lambda;
    }

    /// <summary>
    /// 字符串类型: IsNull(字段,"") == ""
    /// </summary>
    /// <typeparam name="TDbEntity"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Expression<Func<TDbEntity, bool>> GetExpression_NoValue<TDbEntity>(string propertyName)
        where TDbEntity : class
    {
        var type_TEntity = typeof(TDbEntity);
        var prop_Info = type_TEntity.GetProperty(propertyName);
        if (prop_Info == null)
        {
            throw new Exception($"{propertyName}不在{typeof(TDbEntity).FullName}类型中");
        }
        if (prop_Info.PropertyType != typeof(string))
        {
            throw new Exception($"{propertyName}不在{typeof(TDbEntity).FullName}类型中只能为string类型");
        }

        var parameterExp = Expression.Parameter(type_TEntity, "a");

        var left = Expression.MakeBinary(
            ExpressionType.Equal,
            Expression.Coalesce(
                Expression.MakeMemberAccess(parameterExp, prop_Info),
                Expression.Constant("", typeof(string))
            ),
            Expression.Constant("")
        //dnspy 反编译没有这个2个, ExpressionTreeToString有
        //, false
        //, typeof(string).GetMethod("op_Equality")
        );
        var lambda = Expression.Lambda<Func<TDbEntity, bool>>(left, parameterExp);
        return lambda;
    }

    #endregion

    #region WhereNotDeleted

    public static Expression<Func<TDbEntity, bool>> GetExpression_NotDeleted<TDbEntity>(string propertyName)
        where TDbEntity : class
    {
        //todo:
        return null;
    }

    #endregion

    private static bool HaveCount(List<string> searchCondition) => searchCondition != null && searchCondition.Count > 0;

    private static bool HaveCount(string[] searchCondition) => searchCondition != null && searchCondition.Length > 0;

    private static List<Expression<Func<TDbEntity, bool>>> Default<TDbEntity>() => new();

    private static bool ContinueAdd<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, string prop, object value)
        where TSearch : class
        where TDbEntity : class
    {
        if (ExistsWhereIf(that, prop)) //有定义满足哪种情况下才算有值
        {
            if (InvokeWhereIf(that, prop) == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else //默认判断是否有值就以null值进行判断
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool ExistsWhereIf<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, string propName)
           where TSearch : class
           where TDbEntity : class
        {
            if (that == null ||
                that.Search == null ||
                that.WhereIf == null ||
                that.WhereIf.ContainsKey(propName) == false
               )
            {
                return false;
            }

            return true;
        }

        bool InvokeWhereIf<TSearch, TDbEntity>(QueryConfig<TSearch, TDbEntity> that, string propName)
            where TSearch : class
            where TDbEntity : class
        {
            Expression<Func<TSearch, bool>> exp = that.WhereIf[propName];
            var result = exp.Compile().Invoke(that.Search);
            return result;
        }
    }
}