using ExpressionToSqlWhereClause.Exceptions;
using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.Helper;
using ExpressionToSqlWhereClause.SqlFunc;
using ExpressionToSqlWhereClause.SqlFunc.EntityConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ExpressionToSqlWhereClause.EntityConfig
{
    ///
    public static class WhereLambdaHelper
    {

        #region AddLike

        public static List<Expression<Func<TEntity, bool>>> AddLike<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();

            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddLike<TEntity, TSearch>(that, props);

        }

        public static List<Expression<Func<TEntity, bool>>> AddLike<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (ContinueAdd(that, prop, value))
                {
                    continue;
                }
                if (value is string valueStr && !string.IsNullOrWhiteSpace(valueStr))
                {
                    var exp = WhereLambdaHelper.GetExpression_Contains<TEntity>(prop, valueStr);
                    if (exp != null)
                    {
                        whereLambdas.Add(exp);
                    }
                }
            }
            return whereLambdas;
        }

        // t.SomeProperty.Contains("stringValue");
        private static Expression<Func<TEntity, bool>> GetExpression_Contains<TEntity>(string propertyName, string propertyValue)
            where TEntity : class
        {
            var type_TEntity = typeof(TEntity);
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
            return Expression.Lambda<Func<TEntity, bool>>(containsMethodExp, parameterExp);
        }

        #endregion

        #region AddLikeLeft

        public static List<Expression<Func<TEntity, bool>>> AddLikeLeft<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();

            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddLikeLeft<TEntity, TSearch>(that, props);
        }

        public static List<Expression<Func<TEntity, bool>>> AddLikeLeft<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (ContinueAdd(that, prop, value))
                {
                    continue;
                }
                if (value is string valueStr && !string.IsNullOrWhiteSpace(valueStr))
                {
                    var exp = WhereLambdaHelper.GetExpression_StartsWith<TEntity>(prop, valueStr);
                    if (exp != null)
                    {
                        whereLambdas.Add(exp);
                    }
                }

            }
            return whereLambdas;
        }


        // t.SomeProperty.StartsWith("stringValue");
        private static Expression<Func<TEntity, bool>> GetExpression_StartsWith<TEntity>(string propertyName, string propertyValue)
        {
            var type_TEntity = typeof(TEntity);
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
            return Expression.Lambda<Func<TEntity, bool>>(containsMethodExp, parameterExp);
        }

        #endregion

        #region AddLikeRight

        ///
        public static List<Expression<Func<TEntity, bool>>> AddLikeRight<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();
            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddLikeRight<TEntity, TSearch>(that, props);
        }

        ///
        public static List<Expression<Func<TEntity, bool>>> AddLikeRight<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (ContinueAdd(that, prop, value))
                {
                    continue;
                }
                if (value is string valueStr && !string.IsNullOrWhiteSpace(valueStr))
                {
                    var exp = WhereLambdaHelper.GetExpression_EndsWith<TEntity>(prop, valueStr);
                    if (exp != null)
                    {
                        whereLambdas.Add(exp);
                    }
                }
            }
            return whereLambdas;
        }


        // t.SomeProperty.EndsWith("stringValue");
        private static Expression<Func<TEntity, bool>> GetExpression_EndsWith<TEntity>(string propertyName, string propertyValue)
        {
            var type_TEntity = typeof(TEntity);
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
            return Expression.Lambda<Func<TEntity, bool>>(containsMethodExp, parameterExp);
        }

        #endregion

        #region AddEqual??????2 : ??????: AddInOrEuqal ???????????????

        public static List<Expression<Func<TEntity, bool>>> AddEqual<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();
            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddEqual<TEntity, TSearch>(that, props);
        }

        public static List<Expression<Func<TEntity, bool>>> AddEqual<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();
            //todo:?????????????????? TSearch ???????????? Attribute
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);

                if (ContinueAdd(that, prop, value))
                {
                    continue;
                }
                AddEqualCore<TEntity, TSearch>(prop, valuePropType, value, whereLambdas);

            }
            return whereLambdas;
        }

        //??????-equal
        private static void AddEqualCore<TEntity, TSearch>(string propertyName, Type valuePropType, object propertyValue, List<Expression<Func<TEntity, bool>>> whereLambdas)
            where TEntity : class
            where TSearch : class
        {
            var type_TEntity = typeof(TEntity);
            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                return;
            }

            // AddEqual ?????????
            var parameterExp = Expression.Parameter(type_TEntity, "a");
            var propertyExp = Expression.Property(parameterExp, propertyName); //a.AuditStateId
            Type propType_TEntity = propertyExp.Type; //a.AuditStateId ???type(Dto????????????????????????)

            var attr_SqlFuncArray = ReflectionHelper.GetAttributeForProperty<SqlFuncAttribute>(typeof(TSearch).GetProperty(propertyName), true);
            var haveAttr_SqlFunc = attr_SqlFuncArray.Length > 0;
            if (haveAttr_SqlFunc)
            {
                if (attr_SqlFuncArray.Length > 1)
                {
                    throw new FrameException($"??????:{nameof(SqlFuncAttribute)}??????????????????.");
                }

                var attr_SqlFunc = attr_SqlFuncArray[0];
                if (attr_SqlFunc is MonthAttribute)
                {
                    //GetLambda_MonthAttribute_Equal

                    ParameterExpression parameterExpression = Expression.Parameter(type_TEntity, "u");
                    //                  DbFunctions ???          Month(                      DateTime dt) 
                    var leftP2 = typeof(DbFunctions).GetMethod("Month", new Type[] { typeof(DateTime) }); //SqlFunc.DbFunctions.Month(DateTime dt) 
                                                                                                          //  type_TEntity ??? propertyName ??? ?????? ?????? ???   DateTime
                    var leftP3 = new Expression[] { Expression.Property(parameterExpression, propertyName) }; // u.CreateAt
                    var left = Expression.Call(null, leftP2, leftP3); //SqlFunc.DbFunctions.Month(u.CreateAt)


                    //Sql ??? month() ????????????int 
                    //https://docs.microsoft.com/zh-cn/sql/t-sql/functions/month-transact-sql?view=sql-server-2017
                    var right = Expression.Constant(Convert.ToInt32(propertyValue), typeof(int));
                    var body = Expression.Equal(left, right);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExpression);
                    whereLambdas.Add(lambda);

                }
            }
            else
            {
                var left = Expression.Convert(propertyExp, propType_TEntity);
                //var right = Expression.Constant(propertyValue, propertyValue.GetType());//ids????????????????????????
                //?????????if ???????????????ids?????????: ???id??????????????????????????????, ?????????????????????string??????, ??????????????????????????????????????????????????????
                if (valuePropType != propType_TEntity || propType_TEntity != typeof(string))
                {
                    //?????? "" ?????? ?????????????????????????????????
                    if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
                    {
                        return;
                    }
                    propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);
                }

                var right = Expression.Constant(propertyValue, propType_TEntity);

                var body = Expression.Equal(left, right);
                var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
                whereLambdas.Add(lambda);
            }
        }

        #endregion

        #region AddNotEqual-??????Equal?????????2

        public static List<Expression<Func<TEntity, bool>>> AddNotEqual<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();
            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddNotEqual<TEntity, TSearch>(that, props);
        }

        public static List<Expression<Func<TEntity, bool>>> AddNotEqual<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (ContinueAdd(that, prop, value))
                {
                    continue;
                }
                AddNotEqualCore<TEntity, TSearch>(prop, valuePropType, value, whereLambdas);

            }
            return whereLambdas;
        }

        //??????
        private static void AddNotEqualCore<TEntity, TSearch>(string propertyName, Type valuePropType, object propertyValue, List<Expression<Func<TEntity, bool>>> whereLambdas)
            where TEntity : class
            where TSearch : class
        {
            var type_TEntity = typeof(TEntity);
            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                return;
            }

            // AddEqual ?????????
            var parameterExp = Expression.Parameter(type_TEntity, "a");
            var propertyExp = Expression.Property(parameterExp, propertyName); //a.AuditStateId
            Type propType_TEntity = propertyExp.Type; //a.AuditStateId ???type(Dto????????????????????????)

            var attr_SqlFuncArray = ReflectionHelper.GetAttributeForProperty<SqlFuncAttribute>(typeof(TSearch).GetProperty(propertyName), true);
            var haveAttr_SqlFunc = attr_SqlFuncArray.Length > 0;
            if (haveAttr_SqlFunc)
            {
                if (attr_SqlFuncArray.Length > 1)
                {
                    throw new FrameException($"??????:{nameof(SqlFuncAttribute)}??????????????????.");
                }
                var attr_SqlFunc = attr_SqlFuncArray[0];
                if (attr_SqlFunc is MonthAttribute)
                {
                    ParameterExpression parameterExpression = Expression.Parameter(type_TEntity, "u");
                    var leftP2 = typeof(DbFunctions).GetMethod("Month", new Type[] { typeof(DateTime) });
                    var leftP3 = new Expression[] { Expression.Property(parameterExpression, propertyName) };
                    var left = Expression.Call(null, leftP2, leftP3);
                    //Sql ??? month() ????????????int 
                    //https://docs.microsoft.com/zh-cn/sql/t-sql/functions/month-transact-sql?view=sql-server-2017
                    var right = Expression.Constant(Convert.ToInt32(propertyValue), typeof(int));
                    var body = Expression.NotEqual(left, right);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExpression);
                    whereLambdas.Add(lambda);
                }
            }
            else
            {
                var left = Expression.Convert(propertyExp, propType_TEntity);
                //var right = Expression.Constant(propertyValue, propertyValue.GetType());//ids????????????????????????
                //?????????if ???????????????ids?????????: ???id??????????????????????????????, ?????????????????????string??????, ??????????????????????????????????????????????????????
                if (valuePropType != propType_TEntity || propType_TEntity != typeof(string))
                {
                    //?????? "" ?????? ?????????????????????????????????
                    if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
                    {
                        return;
                    }

                    propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);
                }

                var right = Expression.Constant(propertyValue, propType_TEntity);

                var body = Expression.NotEqual(left, right);
                var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
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

        public static List<Expression<Func<TEntity, bool>>> AddNumberRange<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();
            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddNumberRange<TEntity, TSearch>(that, props);
        }

        public static List<Expression<Func<TEntity, bool>>> AddNumberRange<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }
            var type_TEntity = typeof(TEntity);

            //key : ?????? value : ?????????(??????), ?????????(??????)
            var numberDict = new Dictionary<string, NumberSearch>();

            #region ????????? timeDict
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);

                if (ContinueAdd(that, prop, value))
                {
                    continue;
                }

                if (!Regex.IsMatch(value.ToString(), @"^(-?\d+)(\.\d+)?$")) //???????????????(??????+?????????)
                {
                    throw new FrameException($"?????????({value})??????????????????.");
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
                            throw new FrameException("????????????");
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
                            throw new FrameException("????????????");
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
                        throw new FrameException("????????????");
                    }
                }

            }
            #endregion

            #region ????????????key
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
                        bool needSwap = (dynamic)numbers.NumberRange[0] > (dynamic)numbers.NumberRange[1]; //????????????,??????????????? 
                        if (needSwap)
                        {
                            (numbers.NumberRange[1], numbers.NumberRange[0]) = (numbers.NumberRange[0], numbers.NumberRange[1]);
                        }
                    }
                }
            }

            //?????? NumberRange  ???????????????(???????????????4?????????)
            //isPair???    ??????         ??????
            //false      ??????(??????)     ??????(???)
            //true       ??????(??????)     ??????(???)
            //true       ??????(??????)     ??????
            //true       ??????          ??????(???)

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();

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
                    throw new FrameException("?????????????????????, ???????????????????????????");
                }

                if (d1 != null) //>=
                {
                    if (type_TEntity.GetProperty(key) != null)
                    {
                        var parameterExp = Expression.Parameter(type_TEntity, "a");
                        var propertyExp = Expression.Property(parameterExp, key);//a.CreateAt
                        Type propType_TEntity = propertyExp.Type; //a.AuditStateId ???type(Dto????????????????????????)
                        var left = Expression.Convert(propertyExp, propType_TEntity);
                        var right = Expression.Constant(numberDict[key].NumberRange[0], propType_TEntity);
                        var body = Expression.GreaterThanOrEqual(left, right);
                        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
                        whereLambdas.Add(lambda);
                    }
                }

                if (d2 != null) //<=
                {
                    if (type_TEntity.GetProperty(key) != null)
                    {
                        var parameterExp = Expression.Parameter(type_TEntity, "a");
                        var propertyExp = Expression.Property(parameterExp, key); //a.CreateAt
                        Type propType_TEntity = propertyExp.Type; //a.AuditStateId ???type(Dto????????????????????????)
                        var left = Expression.Convert(propertyExp, propType_TEntity);
                        var right = Expression.Constant(numberDict[key].NumberRange[1], propType_TEntity);
                        var body = Expression.LessThanOrEqual(left, right);
                        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
                        whereLambdas.Add(lambda);
                    }
                }

            }
            return whereLambdas;
        }

        #endregion

        #region AddTimeRange

        public static List<Expression<Func<TEntity, bool>>> AddTimeRange<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();
            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddTimeRange<TEntity, TSearch>(that, props);
        }

        public static List<Expression<Func<TEntity, bool>>> AddTimeRange<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            var timeDict = AddTimeRange_GetTimeDict(that, props);

            Get_TimePeriode(timeDict, null);

            var whereLambdas = AddTimeRange_GetWhereLambdas<TEntity>(timeDict);
            return whereLambdas;
        }

        private static Dictionary<string, TimeSearch> AddTimeRange_GetTimeDict<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, string[] props)
            where TEntity : class
            where TSearch : class
        {
            //key : ?????? value : ????????????(??????), ????????????(??????)
            var timeDict = new Dictionary<string, TimeSearch>();

            #region ????????? timeDict
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);

                if (ContinueAdd(that, prop, value))
                {
                    continue;
                }
                if (value is not DateTime)
                {
                    throw new FrameException("??????????????? datetime ??????");
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
                            throw new FrameException("????????????");
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
                            throw new FrameException("????????????");
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
                        throw new FrameException("????????????");
                    }
                }

            }
            #endregion

            #region ????????????key
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
        /// ??????????????????
        /// </summary>
        /// <param name="timeDict"></param>
        /// <param name="getTimePeriodFunc"></param>
        private static void Get_TimePeriode(Dictionary<string, TimeSearch> timeDict, Func<TimePeriod> getTimePeriodFunc)
        {
            var ???????????????????????? = getTimePeriodFunc == null;
            TimePeriod? fixedTimePeriod = null;
            if (!????????????????????????)
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
                        var range = ???????????????????????? ? ???????????????????????????(time) : fixedTimePeriod.Value;
                        times.StartAndEnd[1] = GetEndTime(range, time);
                    }
                }
                else
                {
                    if (times.StartAndEnd[0] != null && times.StartAndEnd[1] != null) //????????? ??? start???
                    {
                        DateTime d1 = times.StartAndEnd[0].Value;
                        DateTime d2 = times.StartAndEnd[1].Value;

                        //SwopUtil.SwopAsc(ref d1, ref d2);

                        if (Comparer<DateTime>.Default.Compare(d1, d2) > 0)
                        {
                            (d2, d1) = (d1, d2);
                        }

                        if (d1 == d2)//??????????????? ?????????????????? ????????????????????????  
                        {
                            var period = ???????????????????????? ? ???????????????????????????(d1) : fixedTimePeriod.Value;
                            d2 = GetEndTime(period, d1);
                        }
                        else
                        {
                            var period = ???????????????????????? ? ???????????????????????????(d2) : fixedTimePeriod.Value;
                            d2 = GetEndTime(period, d2);

                        }

                        times.StartAndEnd[0] = d1;
                        times.StartAndEnd[1] = d2;
                    }
                    else if (times.StartAndEnd[0] == null && times.StartAndEnd[1] != null)//?????? end ??????
                    {
                        var endTime = (DateTime)times.StartAndEnd[1];
                        var period = ???????????????????????? ? ???????????????????????????(endTime) : fixedTimePeriod.Value;
                        var newEndTime = GetEndTime((TimePeriod)period, endTime);
                        times.StartAndEnd[1] = newEndTime;
                    }
                }
            }
        }

        private static List<Expression<Func<TEntity, bool>>> AddTimeRange_GetWhereLambdas<TEntity>(Dictionary<string, TimeSearch> timeDict)
            where TEntity : class
        {
            var type_TEntity = typeof(TEntity);
            //?????? TimePeriod ???????????????(???????????????4?????????)
            //isPair???    ??????         ??????
            //false      ??????(??????)     ??????(??????)
            //true       ??????(??????)     ??????(??????)
            //true       ??????(??????)     ??????
            //true       ??????          ??????(??????)

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();

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
                    throw new FrameException("?????????????????????, ???????????????????????????");
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
                        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
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
                        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
                        whereLambdas.Add(lambda);
                    }
                }
            }

            return whereLambdas;
        }

        #region ?????? period ???

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSearch"></typeparam>
        /// <param name="period">??????????????????????????????????????????</param>
        /// <param name="searchModel"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static List<Expression<Func<TEntity, bool>>> AddTimeRange<TEntity, TSearch>(TimePeriod period, WhereLambda<TEntity, TSearch> that, List<string> props)
            where TEntity : class
            where TSearch : class
        {
            return HaveCount(props)
                ? AddTimeRange<TEntity, TSearch>(period, that, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddTimeRange<TEntity, TSearch>(TimePeriod period, WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            var timeDict = AddTimeRange_GetTimeDict(that, props);

            Get_TimePeriode(timeDict, () => period);

            var whereLambdas = AddTimeRange_GetWhereLambdas<TEntity>(timeDict);
            return whereLambdas;
        }

        #endregion

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

            throw new FrameException("?????????????????????????????????, ????????????????????????,??????????????????");

        }

        /// <summary>
        /// ??????????????????????????????: ??????????????? Second ???, ??????????????????????????????????????????
        /// d1:2022-1-1 9:0:0
        /// d2:2022-1-1 9:1:0  ?????? ?????????  ????????? Minute
        /// 
        /// d1:2022-1-1 0:0:0
        /// d2:2022-1-1 1:0:0  ?????? ?????????  ????????? Hour
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static TimePeriod ???????????????????????????(DateTime time)
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

            throw new Exception("?????????????????????,??????????????????");
        }

        #endregion

        #region AddIn

        /// <summary>
        /// ???????????????in ?????? Euqal , ?????? split() ??????????????????
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSearch"></typeparam>
        /// <param name="searchModel"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static List<Expression<Func<TEntity, bool>>> AddIn<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();
            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddIn<TEntity, TSearch>(that, props);
        }

        /// <summary>
        /// ???????????????in ?????? Euqal , ?????? split() ??????????????????
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSearch"></typeparam>
        /// <param name="searchModel"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static List<Expression<Func<TEntity, bool>>> AddIn<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] propertyNames)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(propertyNames))
            {
                return Default<TEntity>();
            }
            var type_TEntity = typeof(TEntity);

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();
            foreach (var propertyName in propertyNames)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(propertyName);
                if (ContinueAdd(that, propertyName, value))
                {
                    continue;
                }

                //????????????:?????????????????????,???????????????????????????,??????????????????Model??????(????????????????????????)?????????
                //if (value.GetType() != typeof(string))
                //{
                //    throw new ArgumentException("??????????????????string,????????????Split()??????");
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

                Type propType_TEntity = propertyExp.Type; //a.AuditStateId ???type (Dto????????????????????????)

                //IEnumerable<T> ???T ?????????  propType_TEntity ??????
                //string + ??????
                //string    ushort    short    int    uint    char    float    double    long    ulong    decimal   datetime
                //1          1          1       1       1      1         1       1         1       1         1        0

                Expression<Func<TEntity, bool>> lambda = null;
                SqlFuncAttribute[] attr_SqlFuncArray = ReflectionHelper.GetAttributeForProperty<SqlFuncAttribute>(typeof(TSearch).GetProperty(propertyName), true);
                var haveAttr_SqlFunc = attr_SqlFuncArray.Length > 0;
                if (haveAttr_SqlFunc)
                {
                    if (attr_SqlFuncArray.Length > 1)
                    {
                        var exMsg = $"??????:{nameof(SqlFuncAttribute)}??????????????????.";
                        throw new FrameException(exMsg);
                    }

                    var attr_SqlFunc = attr_SqlFuncArray[0];
                    if (attr_SqlFunc is MonthInAttribute)
                    {
                        #region ?????????????????????, ?????????????????? ????????????
                        //???????????????: Expression<Func<User_SqlFunc_Entity, bool>> expression = u => SqlFunc.DbFunctions.MonthIn(u.CreateAt) == new List<int> { 5, 6 };

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

                        lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExpression);
                        #endregion
                    }
                }

                if (lambda == null)
                {
                    if (propType_TEntity == typeof(string))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits);
                    }

                    #region ?????????

                    else if (propType_TEntity == typeof(bool))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToBool());
                    }
                    else if (propType_TEntity == typeof(byte))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToInt8());
                    }
                    else if (propType_TEntity == typeof(sbyte))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToUInt8());
                    }
                    else if (propType_TEntity == typeof(short))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToInt16());
                    }
                    else if (propType_TEntity == typeof(ushort))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToUInt16());
                    }
                    else if (propType_TEntity == typeof(int))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToInt32());
                    }
                    else if (propType_TEntity == typeof(uint))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToUInt32());
                    }
                    else if (propType_TEntity == typeof(long))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToInt64());
                    }
                    else if (propType_TEntity == typeof(ulong))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToUInt64());
                    }
                    else if (propType_TEntity == typeof(float))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToFloat());
                    }
                    else if (propType_TEntity == typeof(double))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToDouble());
                    }
                    else if (propType_TEntity == typeof(decimal))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToDecimal());
                    }
                    else if (propType_TEntity == typeof(char))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToChar());
                    }

                    #endregion

                    #region ???????????????

                    else if (propType_TEntity == typeof(bool?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableBool());
                    }
                    else if (propType_TEntity == typeof(byte?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableInt8());
                    }
                    else if (propType_TEntity == typeof(sbyte?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToUInt8());
                    }
                    else if (propType_TEntity == typeof(short?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableInt16());
                    }
                    else if (propType_TEntity == typeof(ushort?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableUInt16());
                    }
                    else if (propType_TEntity == typeof(int?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableInt32());
                    }
                    else if (propType_TEntity == typeof(uint?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableUInt32());
                    }
                    else if (propType_TEntity == typeof(long?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableInt64());
                    }
                    else if (propType_TEntity == typeof(ulong?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableUInt64());
                    }
                    else if (propType_TEntity == typeof(float?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableFloat());
                    }
                    else if (propType_TEntity == typeof(double?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableDouble());
                    }
                    else if (propType_TEntity == typeof(decimal?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableDecimal());
                    }
                    else if (propType_TEntity == typeof(char?))
                    {
                        lambda = GetExpression_In<TEntity>(parameterExp, propertyExp, splits.ToNullableChar());
                    }

                    #endregion 
                }

                if (lambda == null)
                {
                    var exMsg = $"SearchType:{nameof(SearchType.In)},????????????????????????????????????:{propType_TEntity}";
                    throw new FrameException(exMsg);
                }

                whereLambdas.Add(lambda);

            }
            return whereLambdas;
        }

        private static Expression<Func<TEntity, bool>> GetExpression_In<TEntity>(ParameterExpression parameterExp, MemberExpression propertyExp, object listObj)
            where TEntity : class
        {
            //??????
            //https://stackoverflow.com/questions/18491610/dynamic-linq-expression-for-ienumerableint-containsmemberexpression
            //https://stackoverflow.com/questions/26659824/create-a-predicate-builder-for-x-listofints-containsx-listofintstocheck

            //ParameterExpression parameterExp = Expression.Parameter(type_TEntity, "a"); 
            //MemberExpression propertyExp = Expression.Property(parameterExp, propertyName); //a.AuditStateId   

            //var someValue = Expression.Constant(listObj, typeof(List<Int16?>));
            ConstantExpression someValue = Expression.Constant(listObj);

            //????????? sql  CAST( `t`.`audit_state_id` AS int ) , ?????? mysql(5.7.32)  int ?????????????????? signed ????????????sql????????????
            //var convertExpression = Expression.Convert(propertyExp, typeof(Int32));
            //var call = Expression.Call(someValue, "Contains", new Type[] { }, convertExpression);  

            //var method = typeof(Enumerable).GetMethod("Contains");//????????? ,??????????????????
            //var method = typeof(IEnumerable<Int32>).GetMethod("Contains");//??????????????????
            //var method = typeof(List<Int32?>).GetMethod("Contains"); //List<Int32?> ??????  a.AuditStateId ?????????????????????

            //var call = Expression.Call(someValue, method, propertyExp); //???????????????????????????.

            var call = Expression.Call(typeof(Enumerable), "Contains", new[] { propertyExp.Type }, someValue, propertyExp);

            //containsMethodExp
            var lambda = Expression.Lambda<Func<TEntity, bool>>(call, parameterExp);
            //???????????????????????????
            // var lambda = Expression.Lambda<Func<TEntity, bool>>(Expression.Not(call), pe); 
            return lambda;
        }

        #endregion

        #region AddGt

        public static List<Expression<Func<TEntity, bool>>> AddGt<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();
            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }

            return AddGt<TEntity, TSearch>(that, props);
        }

        public static List<Expression<Func<TEntity, bool>>> AddGt<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();

            var type_TEntity = typeof(TEntity);
            foreach (string prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (ContinueAdd(that, prop, value) || type_TEntity.GetProperty(prop) == null)
                {
                    continue;
                }
                if (!valuePropType.IsClass)
                {
                    var exp = WhereLambdaHelper.GetExpression_gt<TEntity>(prop, value);
                    if (exp != null)
                    {
                        whereLambdas.Add(exp);
                    }
                }
            }

            return whereLambdas;
        }

        // t.SomeProperty > 5
        private static Expression<Func<TEntity, bool>> GetExpression_gt<TEntity>(string propertyName, object propertyValue)
            where TEntity : class
        {
            var type_TEntity = typeof(TEntity);
            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                return null;
            }

            var parameterExp = Expression.Parameter(type_TEntity, "a");
            var propertyExp = Expression.Property(parameterExp, propertyName); //a.Id
            Type propType_TEntity = propertyExp.Type; // domain(typeof(TEntity)) ??????Id???????????????
            var left = Expression.Convert(propertyExp, propType_TEntity);

            //?????? "" ?????? ?????????????????????????????????
            if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
            {
                return null;
            }

            propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);

            var right = Expression.Constant(propertyValue, propType_TEntity);
            var body = Expression.GreaterThan(left, right);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
            return lambda;
        }

        #endregion

        #region AddGe

        public static List<Expression<Func<TEntity, bool>>> AddGe<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();
            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddGe<TEntity, TSearch>(that, props);
        }

        public static List<Expression<Func<TEntity, bool>>> AddGe<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();
            var type_TEntity = typeof(TEntity);
            foreach (string prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (ContinueAdd(that, prop, value) || type_TEntity.GetProperty(prop) == null)
                {
                    continue;
                }
                if (!valuePropType.IsClass)
                {
                    var exp = WhereLambdaHelper.GetExpression_ge<TEntity>(prop, value);
                    if (exp != null)
                    {
                        whereLambdas.Add(exp);
                    }
                }
            }

            return whereLambdas;
        }

        // t.SomeProperty >= 5
        private static Expression<Func<TEntity, bool>> GetExpression_ge<TEntity>(string propertyName, object propertyValue)
            where TEntity : class
        {
            var type_TEntity = typeof(TEntity);
            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                return null;
            }

            var parameterExp = Expression.Parameter(type_TEntity, "a");
            var propertyExp = Expression.Property(parameterExp, propertyName); //a.Id
            Type propType_TEntity = propertyExp.Type; // domain(typeof(TEntity)) ??????Id???????????????
            var left = Expression.Convert(propertyExp, propType_TEntity);

            //?????? "" ?????? ?????????????????????????????????
            if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
            {
                return null;
            }

            propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);

            var right = Expression.Constant(propertyValue, propType_TEntity);
            var body = Expression.GreaterThanOrEqual(left, right);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
            return lambda;
        }

        #endregion

        #region AddLt

        public static List<Expression<Func<TEntity, bool>>> AddLt<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();
            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddLt<TEntity, TSearch>(that, props);
        }

        public static List<Expression<Func<TEntity, bool>>> AddLt<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();
            var type_TEntity = typeof(TEntity);
            foreach (string prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (ContinueAdd(that, prop, value) || type_TEntity.GetProperty(prop) == null)
                {
                    continue;
                }
                if (!valuePropType.IsClass)
                {
                    var exp = WhereLambdaHelper.GetExpression_lt<TEntity>(prop, value);
                    if (exp != null)
                    {
                        whereLambdas.Add(exp);
                    }
                }
            }

            return whereLambdas;
        }

        // t.SomeProperty < 5
        private static Expression<Func<TEntity, bool>> GetExpression_lt<TEntity>(string propertyName, object propertyValue)
            where TEntity : class
        {
            var type_TEntity = typeof(TEntity);
            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                return null;
            }

            var parameterExp = Expression.Parameter(type_TEntity, "a");
            var propertyExp = Expression.Property(parameterExp, propertyName); //a.Id
            Type propType_TEntity = propertyExp.Type; // domain(typeof(TEntity)) ??????Id???????????????
            var left = Expression.Convert(propertyExp, propType_TEntity);

            //?????? "" ?????? ?????????????????????????????????
            if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
            {
                return null;
            }

            propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);

            var right = Expression.Constant(propertyValue, propType_TEntity);
            var body = Expression.LessThan(left, right);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
            return lambda;
        }

        #endregion

        #region AddLe

        public static List<Expression<Func<TEntity, bool>>> AddLe<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, SearchType searchType)
            where TEntity : class
            where TSearch : class
        {
            var props = that.SearchCondition[searchType]?.ToArray();
            if (HaveCount(props) == false)
            {
                return Default<TEntity>();
            }
            return AddLe<TEntity, TSearch>(that, props);
        }

        public static List<Expression<Func<TEntity, bool>>> AddLe<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, params string[] props)
            where TEntity : class
            where TSearch : class
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new();
            var type_TEntity = typeof(TEntity);
            foreach (string prop in props)
            {
                var propertyValue = new PropertyValue<TSearch>(that.Search);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (ContinueAdd(that, prop, value) || type_TEntity.GetProperty(prop) == null)
                {
                    continue;
                }
                if (!valuePropType.IsClass)
                {
                    var exp = WhereLambdaHelper.GetExpression_le<TEntity>(prop, value);
                    if (exp != null)
                    {
                        whereLambdas.Add(exp);
                    }
                }
            }

            return whereLambdas;
        }

        // t.SomeProperty <= 5
        private static Expression<Func<TEntity, bool>> GetExpression_le<TEntity>(string propertyName, object propertyValue)
            where TEntity : class
        {
            var type_TEntity = typeof(TEntity);
            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                return null;
            }

            var parameterExp = Expression.Parameter(type_TEntity, "a");
            var propertyExp = Expression.Property(parameterExp, propertyName); //a.Id
            Type propType_TEntity = propertyExp.Type; // domain(typeof(TEntity)) ??????Id???????????????
            var left = Expression.Convert(propertyExp, propType_TEntity);

            //?????? "" ?????? ?????????????????????????????????
            if (propertyValue.GetType() == typeof(string) && (string)propertyValue == "" && propType_TEntity.IsStructType())
            {
                return null;
            }

            propertyValue = ConvertHelper.ChangeType(propertyValue, propType_TEntity);

            var right = Expression.Constant(propertyValue, propType_TEntity);
            var body = Expression.LessThanOrEqual(left, right);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
            return lambda;
        }

        #endregion

        private static bool HaveCount(List<string> props) => props != null && props.Count > 0;
        private static bool HaveCount(string[] props) => props != null && props.Length > 0;

        private static List<Expression<Func<TEntity, bool>>> Default<TEntity>() => new();

        private static bool ContinueAdd<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, string prop, object value)
            where TEntity : class
            where TSearch : class
        {
            if (ExistsWhereIf(that, prop))
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
            else
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
        }

        private static bool ExistsWhereIf<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, string propName)
            where TEntity : class
            where TSearch : class
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

        private static bool InvokeWhereIf<TEntity, TSearch>(WhereLambda<TEntity, TSearch> that, string propName)
            where TEntity : class
            where TSearch : class
        {
            Expression<Func<TSearch, bool>> exp = that.WhereIf[propName];
            var result = exp.Compile().Invoke(that.Search);
            return result;
        }

    }
}
