using System;
using System.Collections.Concurrent;
using System.Reflection;
using PredicateBuilder.Standard.Exceptions;

namespace PredicateBuilder.Standard
{

    //使用
    //People people = new People { Name = "Wayne" };
    //PropertyValue<People> propertyValue = new PropertyValue<People>(people);
    //object value = propertyValue.Get("Name");

    //快速的动态获取属性值
    internal class PropertyValue<T>
    {
        private static ConcurrentDictionary<string, MemberGetDelegate> _memberGetDelegate = new ConcurrentDictionary<string, MemberGetDelegate>();
        delegate object MemberGetDelegate(T target);
        public PropertyValue(T target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            Target = target;
        }
        public T Target { get; private set; }
        public object Get(string name, out Type propType)
        {
            if (typeof(T) != Target.GetType())
            {
                throw new PredicateBuilderException("当前类不能使用多态这个特性");
            }
            Type type = typeof(T);
            //Type type = Target.GetType();//多态, 取实际类型
            PropertyInfo property = type.GetProperty(name);
            propType = property.PropertyType;
            if (property.PropertyType.IsClass)
            {
                //用了多态, 这里会报错: Cannot bind to the target method because its signature is not compatible with that of the delegate type.
                var memberGet = (MemberGetDelegate)Delegate.CreateDelegate(typeof(MemberGetDelegate), property.GetGetMethod());
                return memberGet(Target);
            }
            else
            {
                return property.GetValue(Target);
            }

            //MemberGetDelegate memberGet = _memberGetDelegate.GetOrAdd(name, BuildDelegate);
            //return memberGet(Target);
        }
        private MemberGetDelegate BuildDelegate(string name)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(name);
            //if (property.GetType().IsClass)
            //{
            //    return (MemberGetDelegate)Delegate.CreateDelegate(typeof(MemberGetDelegate), property.GetGetMethod());
            //}
            return (MemberGetDelegate)Delegate.CreateDelegate(typeof(MemberGetDelegate), property.GetGetMethod());

        }

        //http://www.cocoachina.com/articles/103961
        //根据语言规范,协变不支持值类型.

        public object GetPropValue(string name)
        {
            //如果只想获取值,请使用PropertyInfo.GetValue方法：
            return GetType().GetProperty(name).GetValue(Target);
        }

        public Func<object> GetPropValue2(string name)
        {
            //如果您想返回Func< object>它将在每次调用时获取值,只需在该反射调用周围创建一个lambda即可
            return () => GetType().GetProperty(name).GetValue(Target);
        }

    }
}
