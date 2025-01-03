#if !UNITY_WEBGL
// <auto-generated>
// This .cs file is generated by T4 template. Don't change it. Change the .tt file instead.
// </auto-generated>
using System;

using CWJ.EnumHelper.Internal;

namespace CWJ.EnumHelper
{
    /// <summary>
    /// Provides high performance utilitis for enum type.
    /// </summary>
    public static partial class FastEnum
    {
        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(sbyte value)
            where T : struct, Enum
        {
            if (Cache_UnderlyingOperation<T>.UnderlyingType == typeof(sbyte))
                return SByteOperation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(byte value)
            where T : struct, Enum
        {
            if (Cache_UnderlyingOperation<T>.UnderlyingType == typeof(byte))
                return ByteOperation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(short value)
            where T : struct, Enum
        {
            if (Cache_UnderlyingOperation<T>.UnderlyingType == typeof(short))
                return Int16Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(ushort value)
            where T : struct, Enum
        {
            if (Cache_UnderlyingOperation<T>.UnderlyingType == typeof(ushort))
                return UInt16Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(int value)
            where T : struct, Enum
        {
            if (Cache_UnderlyingOperation<T>.UnderlyingType == typeof(int))
                return Int32Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(uint value)
            where T : struct, Enum
        {
            if (Cache_UnderlyingOperation<T>.UnderlyingType == typeof(uint))
                return UInt32Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(long value)
            where T : struct, Enum
        {
            if (Cache_UnderlyingOperation<T>.UnderlyingType == typeof(long))
                return Int64Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(ulong value)
            where T : struct, Enum
        {
            if (Cache_UnderlyingOperation<T>.UnderlyingType == typeof(ulong))
                return UInt64Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


    }
} 
#endif