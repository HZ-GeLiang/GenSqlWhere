using System;
using System.Collections.Generic;
using System.Linq;

namespace PredicateBuilder.ExtensionMethod
{
    /// <summary>
    /// 特定的类型
    /// </summary>
    internal static partial class EnumerableExtensions
    {

        public static IEnumerable<bool> ToBool(this IEnumerable<string> splits)
            => splits.Select(split => bool.Parse(split));
        public static IEnumerable<byte> ToInt8(this IEnumerable<string> splits)
            => splits.Select(split => byte.Parse(split));
        public static IEnumerable<sbyte> ToUInt8(this IEnumerable<string> splits)
            => splits.Select(split => sbyte.Parse(split));
        public static IEnumerable<short> ToInt16(this IEnumerable<string> splits)
           => splits.Select(split => short.Parse(split));
        public static IEnumerable<ushort> ToUInt16(this IEnumerable<string> splits)
            => splits.Select(split => ushort.Parse(split));
        public static IEnumerable<int> ToInt32(this IEnumerable<string> splits)
            => splits.Select(split => int.Parse(split));
        public static IEnumerable<uint> ToUInt32(this IEnumerable<string> splits)
            => splits.Select(split => uint.Parse(split));
        public static IEnumerable<long> ToInt64(this IEnumerable<string> splits)
            => splits.Select(split => long.Parse(split));
        public static IEnumerable<ulong> ToUInt64(this IEnumerable<string> splits)
            => splits.Select(split => ulong.Parse(split));
        public static IEnumerable<float> ToFloat(this IEnumerable<string> splits)
            => splits.Select(split => float.Parse(split));
        public static IEnumerable<double> ToDouble(this IEnumerable<string> splits)
            => splits.Select(split => double.Parse(split));
        public static IEnumerable<decimal> ToDecimal(this IEnumerable<string> splits)
            => splits.Select(split => decimal.Parse(split));


        //可空值类型
        public static IEnumerable<bool?> ToNullableBool(this IEnumerable<string> splits)
            => splits.Select(split => (bool?)bool.Parse(split));
        public static IEnumerable<byte?> ToNullableInt8(this IEnumerable<string> splits)
            => splits.Select(split => (byte?)byte.Parse(split));
        public static IEnumerable<sbyte?> ToNullableUInt8(this IEnumerable<string> splits)
            => splits.Select(split => (sbyte?)sbyte.Parse(split));
        public static IEnumerable<short?> ToNullableInt16(this IEnumerable<string> splits)
            => splits.Select(split => (short?)short.Parse(split));
        public static IEnumerable<ushort?> ToNullableUInt16(this IEnumerable<string> splits)
            => splits.Select(split => (ushort?)ushort.Parse(split));
        public static IEnumerable<int?> ToNullableInt32(this IEnumerable<string> splits)
            => splits.Select(split => (int?)int.Parse(split));
        public static IEnumerable<uint?> ToNullableUInt32(this IEnumerable<string> splits)
            => splits.Select(split => (uint?)uint.Parse(split));
        public static IEnumerable<long?> ToNullableInt64(this IEnumerable<string> splits)
            => splits.Select(split => (long?)long.Parse(split));
        public static IEnumerable<ulong?> ToNullableUInt64(this IEnumerable<string> splits)
            => splits.Select(split => (ulong?)ulong.Parse(split));
        public static IEnumerable<float?> ToNullableFloat(this IEnumerable<string> splits)
            => splits.Select(split => (float?)float.Parse(split));
        public static IEnumerable<double?> ToNullableDouble(this IEnumerable<string> splits)
            => splits.Select(split => (double?)double.Parse(split));
        public static IEnumerable<decimal?> ToNullableDecimal(this IEnumerable<string> splits)
            => splits.Select(split => (decimal?)decimal.Parse(split));
        public static IEnumerable<char?> ToNullableChar(this IEnumerable<string> splits)
            => splits.Select(split => (char?)char.Parse(split));
        public static IEnumerable<char> ToChar(this IEnumerable<string> splits)
            => splits.Select(split => Convert.ToChar(split));
    }
}