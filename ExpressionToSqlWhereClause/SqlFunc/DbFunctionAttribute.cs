using System;

namespace ExpressionToSqlWhereClause.SqlFunc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DbFunctionAttribute : Attribute //类名字参考自ef
    {
        public DbFunctionAttribute()
        {
            this.FormatOnlyName = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="format">格式化</param>
        public DbFunctionAttribute(string format)
        {
            //举例: SqlFunc.DbFunctions.Month(u.CreateAt) 要翻译为 Month(CreateAt)
            //需要[DbFunctionAttribute(format: "Month({0})", formatOnlyName: true)]

            this.Format = format;
            this.FormatOnlyName = true;
        }

        /// <summary>
        /// 翻译为sql的处理方式
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 是否只保留属性名, 默认 true
        /// 格式化的时候只要属性名, 即取消 u.xxx 的 u.
        /// 目前不对外开放, 如果对外开放,那么 表达时的 u.xxx的 u就要是表名/表别名(因为我不知道sql的xxx是来自哪个表)
        /// </summary>
        public bool FormatOnlyName { get; private set; } = true;
    }
}