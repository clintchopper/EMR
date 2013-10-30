namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;

    #endregion

    public class SerializedValueTypeHelper
    {
        #region Static Members

        public static SerializedValueType GetTypeFromObject(object objValue)
        {
            SerializedValueType enuValueType = SerializedValueType.Unknown;

            if ((objValue == null) || (objValue is DBNull))
            {
                enuValueType = SerializedValueType.Null;
            }
            else if (objValue is Boolean)
            {
                enuValueType = SerializedValueType.Boolean;
            }
            else if (objValue is byte)
            {
                enuValueType = SerializedValueType.Byte;
            }
            else if (objValue is byte[])
            {
                enuValueType = SerializedValueType.ByteArray;
            }
            else if (objValue is Char)
            {
                enuValueType = SerializedValueType.Char;
            }
            else if (objValue is DateTime)
            {
                enuValueType = SerializedValueType.DateTime;
            }
            else if (objValue is Decimal)
            {
                enuValueType = SerializedValueType.Decimal;
            }
            else if (objValue is Double)
            {
                enuValueType = SerializedValueType.Double;
            }
            else if (objValue is Enum)
            {
                enuValueType = SerializedValueType.Enum;
            }
            else if (objValue is Guid)
            {
                enuValueType = SerializedValueType.Guid;
            }
            else if (objValue is Int16)
            {
                enuValueType = SerializedValueType.Int16;
            }
            else if (objValue is Int32)
            {
                enuValueType = SerializedValueType.Int32;
            }
            else if (objValue is Int64)
            {
                enuValueType = SerializedValueType.Int64;
            }
            else if (objValue is SByte)
            {
                enuValueType = SerializedValueType.SByte;
            }
            else if (objValue is Single)
            {
                enuValueType = SerializedValueType.Single;
            }
            else if (objValue is String)
            {
                enuValueType = SerializedValueType.String;
            }
            else if (objValue is UInt16)
            {
                enuValueType = SerializedValueType.UInt16;
            }
            else if (objValue is UInt32)
            {
                enuValueType = SerializedValueType.UInt32;
            }
            else if (objValue is UInt64)
            {
                enuValueType = SerializedValueType.UInt64;
            }
            else if (objValue is TimeSpan)
            {
                enuValueType = SerializedValueType.Timespan;
            }

            return enuValueType;
        }

        public static SerializedValueType GetTypeFromSystemType(Type objType)
        {
            SerializedValueType enuValueType = SerializedValueType.Unknown;

            if ((objType == null) || (objType.IsAssignableFrom(typeof(DBNull)) == true))
            {
                enuValueType = SerializedValueType.Null;
            }
            else if (objType.IsAssignableFrom(typeof(Boolean)) == true)
            {
                enuValueType = SerializedValueType.Boolean;
            }
            else if (objType.IsAssignableFrom(typeof(byte)) == true)
            {
                enuValueType = SerializedValueType.Byte;
            }
            else if (objType.IsAssignableFrom(typeof(byte[])) == true)
            {
                enuValueType = SerializedValueType.ByteArray;
            }
            else if (objType.IsAssignableFrom(typeof(char)) == true)
            {
                enuValueType = SerializedValueType.Char;
            }
            else if (objType.IsAssignableFrom(typeof(DateTime)) == true)
            {
                enuValueType = SerializedValueType.DateTime;
            }
            else if (objType.IsAssignableFrom(typeof(decimal)) == true)
            {
                enuValueType = SerializedValueType.Decimal;
            }
            else if (objType.IsAssignableFrom(typeof(double)) == true)
            {
                enuValueType = SerializedValueType.Double;
            }
            else if (objType.IsAssignableFrom(typeof(Enum)) == true)
            {
                enuValueType = SerializedValueType.Enum;
            }
            else if (objType.IsAssignableFrom(typeof(Guid)) == true)
            {
                enuValueType = SerializedValueType.Guid;
            }
            else if (objType.IsAssignableFrom(typeof(Int16)) == true)
            {
                enuValueType = SerializedValueType.Int16;
            }
            else if (objType.IsAssignableFrom(typeof(Int32)) == true)
            {
                enuValueType = SerializedValueType.Int32;
            }
            else if (objType.IsAssignableFrom(typeof(Int64)) == true)
            {
                enuValueType = SerializedValueType.Int64;
            }
            else if (objType.IsAssignableFrom(typeof(SByte)) == true)
            {
                enuValueType = SerializedValueType.SByte;
            }
            else if (objType.IsAssignableFrom(typeof(Single)) == true)
            {
                enuValueType = SerializedValueType.Single;
            }
            else if (objType.IsAssignableFrom(typeof(String)) == true)
            {
                enuValueType = SerializedValueType.String;
            }
            else if (objType.IsAssignableFrom(typeof(UInt16)) == true)
            {
                enuValueType = SerializedValueType.UInt16;
            }
            else if (objType.IsAssignableFrom(typeof(UInt32)) == true)
            {
                enuValueType = SerializedValueType.UInt32;
            }
            else if (objType.IsAssignableFrom(typeof(UInt64)) == true)
            {
                enuValueType = SerializedValueType.UInt64;
            }
            else if (objType.IsAssignableFrom(typeof(TimeSpan)) == true)
            {
                enuValueType = SerializedValueType.Timespan;
            }

            return enuValueType;
        }

        public static bool IsTypeSupported(object objValue)
        {
            SerializedValueType enuValueType = GetTypeFromObject(objValue);
            return (enuValueType != SerializedValueType.Unknown);
        }

        public static bool IsTypeSupported(Type objType)
        {
            SerializedValueType enuValueType = GetTypeFromSystemType(objType);
            return (enuValueType != SerializedValueType.Unknown);
        }

        #endregion
    }
}
