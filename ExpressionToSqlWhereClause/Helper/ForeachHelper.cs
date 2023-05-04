using System;
using System.Reflection;

namespace ExpressionToSqlWhereClause.Helper
{
    internal sealed class ForeachHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerableObj">可遍历对象</param>
        /// <param name="action">Action 的 object 表示当前 遍历的值 </param>
        /// <returns></returns>
        public static void Each(object enumerableObj, Action<object> action)
        {
            //注: 在这里没有判断 enumerableObj 是否为 IsObjectCollection 

            if (action == null)
            {
                return;
            }

            #region 用一段fore的内部原理来获得当前对象

            //1.获得迭代器
            MethodInfo enumerableObj_GetEnumerator = enumerableObj.GetType().GetMethod("GetEnumerator");
            if (enumerableObj_GetEnumerator == null)
            {
                return;
            }
            var itor = enumerableObj_GetEnumerator.Invoke(enumerableObj, new object[] { }); //迭代器
            try
            {
                //2.调用迭代器的MoveNext
                var typeItor = itor.GetType();
                var method_MoveNext = typeItor.GetMethod("MoveNext");
                if (method_MoveNext == null)
                {
                    return;
                }
                while (true)
                {
                    //3.判断是否有下一个
                    bool hasNext = (bool)method_MoveNext.Invoke(itor, new object[] { });
                    if (hasNext == false) break;
                    //4.获得下一个的值
                    var currentProp = typeItor.GetProperty("Current");
                    object currentValue = currentProp.GetValue(itor);
                    action.Invoke(currentValue);
                }
            }
            finally
            {
                // 释放迭代器资源
                IDisposable disposable = itor as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            #endregion
        }
    }
}
