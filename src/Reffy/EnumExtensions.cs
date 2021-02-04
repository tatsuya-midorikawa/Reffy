using System;

namespace Reffy
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 整数値からEnum型の値を取得する
        /// </summary>
        /// <param name="type">Enum型のTypeインスタンス</param>
        /// <param name="value">Enum型内で定義されている値に割り当てられている整数値</param>
        /// <returns>Enum型インスタンス</returns>
        public static object GetEnumValue(this Type type, byte value)
            => type.IsEnum ? Enum.ToObject(type, value) : throw new Exception("'type' is not Enum type.");

        /// <summary>
        /// 整数値からEnum型の値を取得する
        /// </summary>
        /// <param name="type">Enum型のTypeインスタンス</param>
        /// <param name="value">Enum型内で定義されている値に割り当てられている整数値</param>
        /// <returns>Enum型インスタンス</returns>
        public static object GetEnumValue(this Type type, sbyte value)
            => type.IsEnum ? Enum.ToObject(type, value) : throw new Exception("'type' is not Enum type.");

        /// <summary>
        /// 整数値からEnum型の値を取得する
        /// </summary>
        /// <param name="type">Enum型のTypeインスタンス</param>
        /// <param name="value">Enum型内で定義されている値に割り当てられている整数値</param>
        /// <returns>Enum型インスタンス</returns>
        public static object GetEnumValue(this Type type, ushort value)
            => type.IsEnum ? Enum.ToObject(type, value) : throw new Exception("'type' is not Enum type.");

        /// <summary>
        /// 整数値からEnum型の値を取得する
        /// </summary>
        /// <param name="type">Enum型のTypeインスタンス</param>
        /// <param name="value">Enum型内で定義されている値に割り当てられている整数値</param>
        /// <returns>Enum型インスタンス</returns>
        public static object GetEnumValue(this Type type, short value)
            => type.IsEnum ? Enum.ToObject(type, value) : throw new Exception("'type' is not Enum type.");

        /// <summary>
        /// 整数値からEnum型の値を取得する
        /// </summary>
        /// <param name="type">Enum型のTypeインスタンス</param>
        /// <param name="value">Enum型内で定義されている値に割り当てられている整数値</param>
        /// <returns>Enum型インスタンス</returns>
        public static object GetEnumValue(this Type type, uint value)
            => type.IsEnum ? Enum.ToObject(type, value) : throw new Exception("'type' is not Enum type.");

        /// <summary>
        /// 整数値からEnum型の値を取得する
        /// </summary>
        /// <param name="type">Enum型のTypeインスタンス</param>
        /// <param name="value">Enum型内で定義されている値に割り当てられている整数値</param>
        /// <returns>Enum型インスタンス</returns>
        public static object GetEnumValue(this Type type, int value)
            => type.IsEnum ? Enum.ToObject(type, value) : throw new Exception("'type' is not Enum type.");

        /// <summary>
        /// 整数値からEnum型の値を取得する
        /// </summary>
        /// <param name="type">Enum型のTypeインスタンス</param>
        /// <param name="value">Enum型内で定義されている値に割り当てられている整数値</param>
        /// <returns>Enum型インスタンス</returns>
        public static object GetEnumValue(this Type type, ulong value)
            => type.IsEnum ? Enum.ToObject(type, value) : throw new Exception("'type' is not Enum type.");

        /// <summary>
        /// 整数値からEnum型の値を取得する
        /// </summary>
        /// <param name="type">Enum型のTypeインスタンス</param>
        /// <param name="value">Enum型内で定義されている値に割り当てられている整数値</param>
        /// <returns>Enum型インスタンス</returns>
        public static object GetEnumValue(this Type type, long value)
            => type.IsEnum? Enum.ToObject(type, value) : throw new Exception("'type' is not Enum type.");
    }
}
