﻿using System;
using System.Reflection;

namespace ExpressionToSqlWhereClause.Helpers;

internal sealed class ForeachHelper
{
    /// <summary>
    /// 遍历集合
    /// </summary>
    /// <param name="collection">可遍历的集合对象</param>
    /// <param name="action">object: 表示当前遍历对象 </param>
    /// <returns>是否有遍历</returns>
    public static bool Each(object collection, Action<object> action)
    {
        //注: 在这里没有判断 enumerableObj 是否为 IsObjectCollection(自己封装的)
        //原理: 参考fore的内部实现

        if (action == null)
        {
            return false;
        }

        if (GetEachContext(collection, out var itor, out var typeItor, out var moveNext))
        {
            try
            {
                var pms_Invoke = Array.Empty<object>();
                while (true)
                {
                    //3.判断是否有下一个
                    bool hasNext = (bool)moveNext.Invoke(itor, pms_Invoke);
                    if (hasNext == false)
                    {
                        break;
                    }
                    //注: 如果遍历的对象是 List<int?> 且 GetValue() 的值不为null, 那么类型是 int
#if DEBUG
                    //4.获得下一个的值
                    object currentItem = typeItor.GetProperty("Current").GetValue(itor);
                    action.Invoke(currentItem);
#else
                    action.Invoke(typeItor.GetProperty("Current").GetValue(itor));
#endif
                }
            }
            finally
            {
                (itor as IDisposable)?.Dispose();   // 释放迭代器资源
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// 获得遍历对象
    /// </summary>
    /// <param name="collection">可遍历的集合对象</param>
    /// <param name="itor">迭代器</param>
    /// <param name="typeItor">迭代器类型</param>
    /// <param name="moveNext">moveNext方法</param>
    /// <returns></returns>
    internal static bool GetEachContext(object collection,
        out object itor,
        out Type typeItor,
        out MethodInfo moveNext)
    {
        var enumeratorMethod = collection.GetType().GetMethod("GetEnumerator");
        if (enumeratorMethod != null)
        {
            //1.获得迭代器
            itor = enumeratorMethod.Invoke(collection, new object[] { });
            if (itor != null)
            {
                typeItor = itor.GetType();
                //2.调用迭代器的MoveNext
                moveNext = typeItor.GetMethod("MoveNext");

                return true;
            }
        }

        itor = null;
        typeItor = null;
        moveNext = null;
        return false;
    }
}