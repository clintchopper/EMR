namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;

    #endregion

    public class TransportableDataTypeHelper
    {
        #region Static Members

        private static Dictionary<Type, TransportableDataType> _objObjectDataTypeMap = null;

        static TransportableDataTypeHelper()
        {
            _objObjectDataTypeMap = new Dictionary<Type, TransportableDataType>();
            _objObjectDataTypeMap.Add(typeof(DBNull), TransportableDataType.Null);
            _objObjectDataTypeMap.Add(typeof(Boolean), TransportableDataType.Boolean);
            _objObjectDataTypeMap.Add(typeof(byte), TransportableDataType.Byte);
            _objObjectDataTypeMap.Add(typeof(byte[]), TransportableDataType.ByteArray);
            _objObjectDataTypeMap.Add(typeof(Char), TransportableDataType.Char);
            _objObjectDataTypeMap.Add(typeof(DateTime), TransportableDataType.DateTime);
            _objObjectDataTypeMap.Add(typeof(Decimal), TransportableDataType.Decimal);
            _objObjectDataTypeMap.Add(typeof(Double), TransportableDataType.Double);
            _objObjectDataTypeMap.Add(typeof(Enum), TransportableDataType.Enum);
            _objObjectDataTypeMap.Add(typeof(Guid), TransportableDataType.Guid);
            _objObjectDataTypeMap.Add(typeof(Int16), TransportableDataType.Int16);
            _objObjectDataTypeMap.Add(typeof(Int32), TransportableDataType.Int32);
            _objObjectDataTypeMap.Add(typeof(Int64), TransportableDataType.Int64);
            _objObjectDataTypeMap.Add(typeof(ITransportableObject), TransportableDataType.ITransportableObject);
            _objObjectDataTypeMap.Add(typeof(SByte), TransportableDataType.SByte);
            _objObjectDataTypeMap.Add(typeof(Single), TransportableDataType.Single);
            _objObjectDataTypeMap.Add(typeof(String), TransportableDataType.String);
            _objObjectDataTypeMap.Add(typeof(UInt16), TransportableDataType.UInt16);
            _objObjectDataTypeMap.Add(typeof(UInt32), TransportableDataType.UInt32);
            _objObjectDataTypeMap.Add(typeof(UInt64), TransportableDataType.UInt64);
        }

        public static TransportableDataType GetTypeFromObject(object objValue)
        {
            TransportableDataType enuValueType = TransportableDataType.Unknown;
            
            if ((objValue == null) || (objValue is DBNull))
            {
                enuValueType = TransportableDataType.Null;
            }
            else if (objValue is ITransportableObject)
            {
                enuValueType = TransportableDataType.ITransportableObject;
            }
            else
            {
                Type objKey = objValue.GetType();
                if (_objObjectDataTypeMap.ContainsKey(objKey) == true)
                {
                    enuValueType = _objObjectDataTypeMap[objKey];
                }
                else if (objKey.IsSerializable == true)
                {
                    enuValueType = TransportableDataType.Object;
                }
            }            

            return enuValueType;
        }

        public static TransportableDataType GetTypeFromSystemType(Type objType)
        {
            TransportableDataType enuValueType = TransportableDataType.Unknown;

            if (objType == null)
            {
                enuValueType = TransportableDataType.Null;
            }
            else
            {
                if (_objObjectDataTypeMap.ContainsKey(objType) == true)
                {
                    enuValueType = _objObjectDataTypeMap[objType];
                }
                else if (objType.IsSerializable == true)
                {
                    enuValueType = TransportableDataType.Object;
                }
            }

            return enuValueType;
        }
        
        public static bool IsTypeSupported(object objValue)
        {
            TransportableDataType enuValueType = GetTypeFromObject(objValue);
            return (enuValueType != TransportableDataType.Unknown);
        }

        #endregion
    }
}
