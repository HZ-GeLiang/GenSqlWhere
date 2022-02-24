using ExpressionToSqlWhereClause.SqlFunc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.Helper;
using ExpressionToSqlWhereClause.Helper.EntityConfig;
using ExpressionToSqlWhereClause.SqlFunc.EntityConfig;
using ReflectionHelper = ExpressionToSqlWhereClause.Helper.EntityConfig.ReflectionHelper;

// ReSharper disable once CheckNamespace
namespace EntityToSqlWhereClauseConfig
{
    public static class WhereLambdaHelper
    {
        private static bool HaveCount(List<string> props) => props != null && props.Count > 0;
        private static bool HaveCount(string[] props) => props != null && props.Length > 0;

        private static List<Expression<Func<TEntity, bool>>> Default<TEntity>() => new List<Expression<Func<TEntity, bool>>>();

        #region AddLike

        public static List<Expression<Func<TEntity, bool>>> AddLike<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddLike<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddLike<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            //保留代码,提示可以转换成linq
            //List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            //foreach (var prop in props)
            //{
            //    var propertyValue = new PropertyValue<TSearchModel>(searchModel);
            //    object value = propertyValue.Get(prop);
            //    if (value == null)
            //    {
            //        continue;
            //    }
            //    if (value is string)
            //    {
            //        string valueStr = (string)value;
            //        if (!string.IsNullOrWhiteSpace(valueStr))
            //        {
            //            var exp = WhereLambdaHelper.GetExpression_Contains<TEntity>(prop, valueStr);
            //            whereLambdas.Add(exp);
            //        }
            //    }  
            //}
            //return whereLambdas;

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (value == null)
                {
                    continue;
                }
                if (value is string && !string.IsNullOrWhiteSpace((string)value))
                {
                    var exp = WhereLambdaHelper.GetExpression_Contains<TEntity>(prop, (string)value);
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

        public static List<Expression<Func<TEntity, bool>>> AddLikeLeft<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddLikeLeft<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddLikeLeft<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (value == null)
                {
                    continue;
                }
                if (value is string && !string.IsNullOrWhiteSpace((string)value))
                {
                    var exp = WhereLambdaHelper.GetExpression_StartsWith<TEntity>(prop, (string)value);
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

        public static List<Expression<Func<TEntity, bool>>> AddLikeRight<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddLikeRight<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddLikeRight<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (value == null)
                {
                    continue;
                }
                if (value is string && !string.IsNullOrWhiteSpace((string)value))
                {
                    var exp = WhereLambdaHelper.GetExpression_EndsWith<TEntity>(prop, (string)value);
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

        #region AddEqual版本1 (最开始的代码) 后来根据 AddInOrEuqal 衍生出 AddEqual版本2
        /*
        public static List<Expression<Func<TEntity, bool>>> AddEqual<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddEqual<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }
        public static List<Expression<Func<TEntity, bool>>> AddEqual<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                object value = propertyValue.Get(prop, out var propType);
                if (value == null)
                {
                    continue;
                }
                if (value.GetType().IsClass)
                {
                    if (!(value is string) || string.IsNullOrWhiteSpace((string)value))
                    {
                        throw new ExpressionToSqlWhereClauseException("不支持的引用类型");
                    }
                }
                else
                {
                    if (value is DateTime)
                    {
                        throw new ExpressionToSqlWhereClauseException("不支持DateTime类型");
                    }
                }
                //值类型(DateTime) + 非string的引用类型 不给翻译.直接报错

                var parameterExp = Expression.Parameter(type_TEntity, "a");
                var propertyExp = Expression.Property(parameterExp, prop);//a.UserNickName
                var left = Expression.Convert(propertyExp, value.GetType());
                var right = Expression.Constant(value, value.GetType());
                var body = Expression.Equal(left, right);
                var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);

                whereLambdas.Add(lambda);
            }
            return whereLambdas;

        }
        */

        #endregion

        #region AddEqual版本2

        public static List<Expression<Func<TEntity, bool>>> AddEqual<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddEqual<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddEqual<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            //todo:这里可以获得 TSearchModel 的属性的 Attribute
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (value == null)
                {
                    continue;
                }

                AddEqualCore<TEntity, TSearchModel>(prop, valuePropType, value, whereLambdas);
            }
            return whereLambdas;
        }

        //核心-equal
        private static void AddEqualCore<TEntity, TSearchModel>(string propertyName, Type valuePropType, object propertyValue, List<Expression<Func<TEntity, bool>>> whereLambdas)
        {
            var type_TEntity = typeof(TEntity);
            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                return;
            }

            // AddEqual 的代码
            var parameterExp = Expression.Parameter(type_TEntity, "a");
            var propertyExp = Expression.Property(parameterExp, propertyName); //a.AuditStateId
            Type propType_TEntity = propertyExp.Type; //a.AuditStateId 的type(Dto中定义的属性类型)


            var attr_SqlFuncArray = ReflectionHelper.GetAttributeForProperty<SqlFuncAttribute>(typeof(TSearchModel).GetProperty(propertyName), true);
            var haveAttr_SqlFunc = attr_SqlFuncArray.Length > 0;
            if (haveAttr_SqlFunc)
            {
                if (attr_SqlFuncArray.Length > 1)
                {
                    throw new ExpressionToSqlWhereClauseException($"特性:{nameof(SqlFuncAttribute)}不能标记多个.");
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
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExpression);
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
                var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
                whereLambdas.Add(lambda);
            }
        }

        #endregion

        #region AddNotEqual-基于Equal的版本2

        public static List<Expression<Func<TEntity, bool>>> AddNotEqual<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddNotEqual<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddNotEqual<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (value == null)
                {
                    continue;
                }

                AddNotEqualCore<TEntity, TSearchModel>(prop, valuePropType, value, whereLambdas);
            }
            return whereLambdas;
        }

        //核心
        private static void AddNotEqualCore<TEntity, TSearchModel>(string propertyName, Type valuePropType, object propertyValue, List<Expression<Func<TEntity, bool>>> whereLambdas)
        {
            var type_TEntity = typeof(TEntity);
            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                return;
            }

            // AddEqual 的代码
            var parameterExp = Expression.Parameter(type_TEntity, "a");
            var propertyExp = Expression.Property(parameterExp, propertyName); //a.AuditStateId
            Type propType_TEntity = propertyExp.Type; //a.AuditStateId 的type(Dto中定义的属性类型)


            var attr_SqlFuncArray = ReflectionHelper.GetAttributeForProperty<SqlFuncAttribute>(typeof(TSearchModel).GetProperty(propertyName), true);
            var haveAttr_SqlFunc = attr_SqlFuncArray.Length > 0;
            if (haveAttr_SqlFunc)
            {
                if (attr_SqlFuncArray.Length > 1)
                {
                    throw new ExpressionToSqlWhereClauseException($"特性:{nameof(SqlFuncAttribute)}不能标记多个.");
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
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExpression);
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

        public static List<Expression<Func<TEntity, bool>>> AddNumberRange<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddNumberRange<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddNumberRange<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }
            var type_TEntity = typeof(TEntity);

            //key : 字段 value : 开始值(包含), 结束值(包含)
            var numberDict = new Dictionary<string, NumberSearch>();

            #region 初始化 timeDict
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);

                if (value == null)
                {
                    continue;
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                if (!Regex.IsMatch(value.ToString(), @"^(-?\d+)(\.\d+)?$")) //是否是数字(整数+浮点数)
                {
                    throw new ExpressionToSqlWhereClauseException($"当前值({value})不是数字类型.");
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
                            throw new ExpressionToSqlWhereClauseException("重复赋值");
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
                            throw new ExpressionToSqlWhereClauseException("重复赋值");
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
                        throw new ExpressionToSqlWhereClauseException("重复赋值");
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
                            object t = numbers.NumberRange[0];
                            numbers.NumberRange[0] = numbers.NumberRange[1];
                            numbers.NumberRange[1] = t;
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

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();

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
                    throw new ExpressionToSqlWhereClauseException("代码逻辑有问题, 不应该进入这个分支");
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
                        Type propType_TEntity = propertyExp.Type; //a.AuditStateId 的type(Dto中定义的属性类型)
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

        internal class TimeSearch
        {
            public string Prop { get; set; }

            public DateTime?[] StartAndEnd { get; set; }

            public bool? IsPair { get; set; }

        }

        public static List<Expression<Func<TEntity, bool>>> AddTimeRange<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddTimeRange<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddTimeRange<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            var timeDict = AddTimeRange_GetTimeDict(searchModel, props);

            Get_TimePeriode(timeDict, null);

            var whereLambdas = AddTimeRange_GetWhereLambdas<TEntity>(timeDict);
            return whereLambdas;
        }

        private static Dictionary<string, TimeSearch> AddTimeRange_GetTimeDict<TSearchModel>(TSearchModel searchModel, string[] props)
        {
            //key : 字段 value : 开始时间(包含), 结束时间(不含)
            var timeDict = new Dictionary<string, TimeSearch>();

            #region 初始化 timeDict
            foreach (var prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);

                if (value == null)
                {
                    continue;
                }
                if (!(value is DateTime))
                {
                    throw new ExpressionToSqlWhereClauseException("当前值不是 datetime 类型");
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
                            throw new ExpressionToSqlWhereClauseException("重复赋值");
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
                            throw new ExpressionToSqlWhereClauseException("重复赋值");
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
                        throw new ExpressionToSqlWhereClauseException("重复赋值");
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
                            var t = d1;
                            d1 = d2;
                            d2 = t;
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

        private static List<Expression<Func<TEntity, bool>>> AddTimeRange_GetWhereLambdas<TEntity>(Dictionary<string, TimeSearch> timeDict)
        {
            var type_TEntity = typeof(TEntity);
            //根据 TimePeriod 创建表达式(一共是下面4钟情况)
            //isPair值    开始         结束
            //false      有值(包含)     有值(不含)
            //true       有值(包含)     有值(不含)
            //true       有值(包含)     无值
            //true       无值          有值(不含)

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();

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
                    throw new ExpressionToSqlWhereClauseException("代码逻辑有问题, 不应该进入这个分支");
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

        #region 指定 period 的
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSearchModel"></typeparam>
        /// <param name="period">当为秒的时候需要调用这个方法</param>
        /// <param name="searchModel"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static List<Expression<Func<TEntity, bool>>> AddTimeRange<TEntity, TSearchModel>(TimePeriod period, TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddTimeRange<TEntity, TSearchModel>(period, searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddTimeRange<TEntity, TSearchModel>(TimePeriod period, TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            var timeDict = AddTimeRange_GetTimeDict(searchModel, props);

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

            throw new ExpressionToSqlWhereClauseException("后续开发修改过代码逻辑, 但是此处却未修改,需要修改代码");

        }


        /// <summary>
        /// 时间周期
        /// </summary>
        public enum TimePeriod
        {
            //None,
            /// <summary>
            /// 天
            /// </summary>
            Day = 1,
            /// <summary>
            /// 小时
            /// </summary>
            Hour,
            /// <summary>
            /// 分钟
            /// </summary>
            Minute,
            /// <summary>
            /// 秒
            /// </summary>
            Second,
        }

        /// <summary>
        /// 目前该方法的唯一问题: 时间精度为 Second 时, 这个方法并不能返回你要的精度
        /// d1:2022-1-1 9:0:0
        /// d2:2022-1-1 9:1:0  此时 返回的  精度是 Minute
        /// 
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
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSearchModel"></typeparam>
        /// <param name="searchModel"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static List<Expression<Func<TEntity, bool>>> AddIn<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddIn<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }
        /// <summary>
        /// 实际翻译成in 还是 Euqal , 根据 split() 后的个数而定
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSearchModel"></typeparam>
        /// <param name="searchModel"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static List<Expression<Func<TEntity, bool>>> AddIn<TEntity, TSearchModel>(TSearchModel searchModel, params string[] propertyNames)
        {
            if (!HaveCount(propertyNames))
            {
                return Default<TEntity>();
            }
            var type_TEntity = typeof(TEntity);

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            foreach (var propertyName in propertyNames)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(propertyName);
                if (value == null)
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

                if (!splits.Any()) { continue; }

                var prop_Info = type_TEntity.GetProperty(propertyName);
                if (prop_Info == null) { continue; }

                var parameterExp = Expression.Parameter(type_TEntity, "a");//pe
                var propertyExp = Expression.Property(parameterExp, propertyName); //a.AuditStateId  //me

                Type propType_TEntity = propertyExp.Type; //a.AuditStateId 的type (Dto中定义的属性类型)

                //IEnumerable<T> 的T 必需和  propType_TEntity 一样
                //string + 整数
                //string    ushort    short    int    uint    char    float    double    long    ulong    decimal   datetime
                //1          1          1       1       1      1         1       1         1       1         1        0

                Expression<Func<TEntity, bool>> lambda = null;
                SqlFuncAttribute[] attr_SqlFuncArray = ReflectionHelper.GetAttributeForProperty<SqlFuncAttribute>(typeof(TSearchModel).GetProperty(propertyName), true);
                var haveAttr_SqlFunc = attr_SqlFuncArray.Length > 0;
                if (haveAttr_SqlFunc)
                {
                    if (attr_SqlFuncArray.Length > 1)
                    {
                        var exMsg = $"特性:{nameof(SqlFuncAttribute)}不能标记多个.";
                        throw new ExpressionToSqlWhereClauseException(exMsg);
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

                    #region 值类型

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

                    #region 可空值类型

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
                    var exMsg = $"SearchType:{nameof(SearchType.@in)},操作遇到不支持的属性类型:{propType_TEntity}";
                    throw new ExpressionToSqlWhereClauseException(exMsg);
                }

                whereLambdas.Add(lambda);

            }
            return whereLambdas;
        }

        private static Expression<Func<TEntity, bool>> GetExpression_In<TEntity>(ParameterExpression parameterExp, MemberExpression propertyExp, object listObj)
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
            var lambda = Expression.Lambda<Func<TEntity, bool>>(call, parameterExp);
            //“动态不包含”为：
            // var lambda = Expression.Lambda<Func<TEntity, bool>>(Expression.Not(call), pe); 
            return lambda;
        }

        #endregion

        #region AddGt

        public static List<Expression<Func<TEntity, bool>>> AddGt<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddGt<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddGt<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();

            var type_TEntity = typeof(TEntity);
            foreach (string prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (value == null || type_TEntity.GetProperty(prop) == null)
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
        {
            var type_TEntity = typeof(TEntity);
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
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
            return lambda;
        }

        #endregion

        #region AddGe

        public static List<Expression<Func<TEntity, bool>>> AddGe<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddGe<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddGe<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            var type_TEntity = typeof(TEntity);
            foreach (string prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (value == null || type_TEntity.GetProperty(prop) == null)
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
        {
            var type_TEntity = typeof(TEntity);
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
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
            return lambda;
        }

        #endregion

        #region AddLt

        public static List<Expression<Func<TEntity, bool>>> AddLt<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddLt<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddLt<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            var type_TEntity = typeof(TEntity);
            foreach (string prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (value == null || type_TEntity.GetProperty(prop) == null)
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
        {
            var type_TEntity = typeof(TEntity);
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
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
            return lambda;
        }

        #endregion

        #region AddLe

        public static List<Expression<Func<TEntity, bool>>> AddLe<TEntity, TSearchModel>(TSearchModel searchModel, List<string> props)
        {
            return HaveCount(props)
                ? AddLe<TEntity, TSearchModel>(searchModel, props.ToArray())
                : Default<TEntity>();
        }

        public static List<Expression<Func<TEntity, bool>>> AddLe<TEntity, TSearchModel>(TSearchModel searchModel, params string[] props)
        {
            if (!HaveCount(props))
            {
                return Default<TEntity>();
            }

            List<Expression<Func<TEntity, bool>>> whereLambdas = new List<Expression<Func<TEntity, bool>>>();
            var type_TEntity = typeof(TEntity);
            foreach (string prop in props)
            {
                var propertyValue = new PropertyValue<TSearchModel>(searchModel);
                (PropertyInfo property_info, object value, Type valuePropType) = propertyValue.Get(prop);
                if (value == null || type_TEntity.GetProperty(prop) == null)
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
        {
            var type_TEntity = typeof(TEntity);
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
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameterExp);
            return lambda;
        }

        #endregion
    }

  
}
