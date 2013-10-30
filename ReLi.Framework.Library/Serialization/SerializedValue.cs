namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Serialization;
    using ReLi.Framework.Library.Security.Encryption;

    #endregion
    
    public class SerializedValue
    {
        private string _strName;
        private object _objValue;
        private SerializedValueType _enuValueType;
        private bool _blnEncrypted;

        public SerializedValue(string strName, bool objValue)
            : this(strName, objValue, SerializedValueType.Boolean)
        {}

        public SerializedValue(string strName, byte objValue)
            : this(strName, objValue, SerializedValueType.Byte)
        {}

        public SerializedValue(string strName, byte[] objValue)
            : this(strName, objValue, SerializedValueType.ByteArray)
        {}

        public SerializedValue(string strName, char objValue)
            : this(strName, objValue, SerializedValueType.Char)
        {}

        public SerializedValue(string strName, DateTime objValue)
            : this(strName, objValue, SerializedValueType.DateTime)
        {}

        public SerializedValue(string strName, Decimal objValue)
            : this(strName, objValue, SerializedValueType.Decimal)
        {}

        public SerializedValue(string strName, Double objValue)
            : this(strName, objValue, SerializedValueType.Double)
        {}

        public SerializedValue(string strName, Guid objValue)
            : this(strName, objValue, SerializedValueType.Guid)
        {}

        public SerializedValue(string strName, Enum objValue)
            : this(strName, objValue, SerializedValueType.Enum)
        {}

        public SerializedValue(string strName, Int16 objValue)
            : this(strName, objValue, SerializedValueType.Int16)
        {}

        public SerializedValue(string strName, Int32 objValue)
            : this(strName, objValue, SerializedValueType.Int32)
        {}

        public SerializedValue(string strName, Int64 objValue)
            : this(strName, objValue, SerializedValueType.Int64)
        {}

        public SerializedValue(string strName, SByte objValue)
            : this(strName, objValue, SerializedValueType.SByte)
        {}

        public SerializedValue(string strName, Single objValue)
            : this(strName, objValue, SerializedValueType.Single)
        {}

        public SerializedValue(string strName, String objValue)
            : this(strName, objValue, SerializedValueType.String)
        {}

        public SerializedValue(string strName, String objValue, bool blnEncrypted)
            : this(strName, objValue, SerializedValueType.String, blnEncrypted)
        {}

        public SerializedValue(string strName, UInt16 objValue)
            : this(strName, objValue, SerializedValueType.UInt16)
        {}

        public SerializedValue(string strName, UInt32 objValue)
            : this(strName, objValue, SerializedValueType.UInt32)
        {}

        public SerializedValue(string strName, UInt64 objValue)
            : this(strName, objValue, SerializedValueType.UInt64)
        {}

        public SerializedValue(string strName, TimeSpan objValue)
            : this(strName, objValue, SerializedValueType.Timespan)
        { }

        public SerializedValue(string strName, object objValue, SerializedValueType enuValueType)
            : this(strName, objValue, enuValueType, false)
        {}

        public SerializedValue(string strName, object objValue, SerializedValueType enuValueType, bool blnEncrypted)
        {
            if (enuValueType == SerializedValueType.Unknown)
            {
                throw new ArgumentOutOfRangeException("objValue", "The SerializedValueType for this object is not known or supported."); 
            }

            Name = strName;
            _objValue = objValue;
            ValueType = enuValueType;
            Encrypted = blnEncrypted;
        }

        public string Name
        {
            get
            {
                return _strName;
            }
            private set
            {
                if ((value == null) || (value.Trim().Length == 0))
                {
                    throw new ArgumentOutOfRangeException("Name", "A valid non-null, non-empty string is required.");
                }

                _strName = value;
            }
        }

        public object Value
        {
            get
            {
                return _objValue;
            }
            set
            {
                SerializedValueType enuValueType = SerializedValueTypeHelper.GetTypeFromObject(value);
                if (enuValueType == SerializedValueType.Unknown)
                {
                    throw new ArgumentOutOfRangeException("objValue", "The SerializedValueType for this object is not known or supported.");
                }

                _objValue = value;
                ValueType = enuValueType;
            }
        }

        public SerializedValueType ValueType
        {
            get
            {
                return _enuValueType;
            }
            protected set
            {
                _enuValueType = value;
            }
        }

        public bool Encrypted
        {
            get
            {
                return _blnEncrypted;
            }
            set
            {
                _blnEncrypted = value;
            }
        }

        public bool IsNull
        {
            get
            {
                return Value == null;
            }
        }

        public bool GetBoolean()
        {
            return (bool)Value;
        }

        public byte GetByte()
        {
            return (byte)Value;
        }

        public byte[] GetByteArray()
        {
            return (byte[])Value;
        }

        public char GetChar()
        {
            return (char)Value;
        }

        public DateTime GetDateTime()
        {
            return (DateTime)Value;
        }
        
        public decimal GetDecimal()
        {
            return (decimal)Value;
        }

        public double GetDouble()
        {
            return (double)Value;
        }

        public Guid GetGuid()
        {
            return (Guid)Value;
        }

        public Int16 GetInt16()
        {
            return (Int16)Value;
        }

        public Int32 GetInt32()
        {
            return (Int32)Value;
        }

        public Int64 GetInt64()
        {
            return (Int64)Value;
        }

        public SByte GetSByte()
        {
            return (SByte)Value;
        }

        public Single GetSingle()
        {
            return (Single)Value;
        }

        public String GetString()
        {
            return (string)Value;
        }

        public UInt16 GetUInt16()
        {
            return (UInt16)Value;
        }

        public UInt32 GetUInt32()
        {
            return (UInt32)Value;
        }

        public UInt64 GetUInt64()
        {
            return (UInt64)Value;
        }

        public TimeSpan GetTimespan()
        {
            return (TimeSpan)Value;
        }

        public object GetObject()
        {
            return Value;
        }

        public TEnumType GetEnum<TEnumType>()
            where TEnumType : struct, IConvertible 
        {
            TEnumType enuValue = (TEnumType)Enum.Parse(typeof(TEnumType), Value.ToString());
            return enuValue;
        }

        public TObjectType GetValue<TObjectType>()
        {
            TObjectType objValue = default(TObjectType);
            Type objObjectType = typeof(TObjectType);

            if (objObjectType.IsEnum == true)
            {
                objValue = (TObjectType)Enum.Parse(objObjectType, Value.ToString());
            }
            else
            {
                SerializedValueType enuValueType = SerializedValueTypeHelper.GetTypeFromSystemType(objObjectType);
                switch (enuValueType)
                {
                    case (SerializedValueType.Boolean):
                        objValue = (TObjectType)((object)GetBoolean());
                        break;

                    case (SerializedValueType.Byte):
                        objValue = (TObjectType)((object)GetByte());
                        break;

                    case (SerializedValueType.Char):
                        objValue = (TObjectType)((object)GetChar());
                        break;

                    case (SerializedValueType.DateTime):
                        objValue = (TObjectType)((object)GetDateTime());
                        break;

                    case (SerializedValueType.Decimal):
                        objValue = (TObjectType)((object)GetDecimal());
                        break;

                    case (SerializedValueType.Double):
                        objValue = (TObjectType)((object)GetDouble());
                        break;

                    case (SerializedValueType.Enum):
                        objValue = (TObjectType)Enum.Parse(objObjectType, GetString());
                        break;

                    case (SerializedValueType.Guid):
                        objValue = (TObjectType)((object)GetGuid());
                        break;

                    case (SerializedValueType.Int16):
                        objValue = (TObjectType)((object)GetInt16());
                        break;

                    case (SerializedValueType.Int32):
                        objValue = (TObjectType)((object)GetInt32());
                        break;

                    case (SerializedValueType.Int64):
                        objValue = (TObjectType)((object)GetInt64());
                        break;

                    case (SerializedValueType.SByte):
                        objValue = (TObjectType)((object)GetSByte());
                        break;

                    case (SerializedValueType.Single):
                        objValue = (TObjectType)((object)GetSingle());
                        break;

                    case (SerializedValueType.String):
                        objValue = (TObjectType)((object)GetString());
                        break;

                    case (SerializedValueType.UInt16):
                        objValue = (TObjectType)((object)GetUInt16());
                        break;

                    case (SerializedValueType.UInt32):
                        objValue = (TObjectType)((object)GetUInt32());
                        break;

                    case (SerializedValueType.UInt64):
                        objValue = (TObjectType)((object)GetUInt64());
                        break;

                    case (SerializedValueType.Timespan):
                        objValue = (TObjectType)((object)GetTimespan());
                        break;

                    case (SerializedValueType.ByteArray):
                        objValue = (TObjectType)((object)GetByteArray());
                        break;
                }
            }

            return objValue;
        }
    }
}
