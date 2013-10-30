namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Diagnostics;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Remoting;
    using ReLi.Framework.Library.Security.Encryption;
    using System.Runtime.Serialization.Formatters.Binary;

    #endregion

    public class BinaryWriterExtension : BinaryWriter
    {
        private List<bool> _objBits = null;

        protected BinaryWriterExtension()
        { }

        public BinaryWriterExtension(Stream objOutput)
            : base(objOutput)
        { }

        public BinaryWriterExtension(Stream objOutput, Encoding enuEncoding)
            : base(objOutput, enuEncoding)
        { }

        protected List<bool> Bits
        {
            get
            {
                if (_objBits == null)
                {
                    _objBits = new List<bool>();
                }

                return _objBits;
            }
        }

        public override void Flush()
        {
            BitArray objBitArray = new BitArray(Bits.ToArray());
            byte[] objBytes = objBitArray.GetBytes();
            Write(objBytes, 0, objBytes.Length);
            Write(objBytes.Length);

            base.Flush();
        }

        public void WriteEnum(Enum enuEnum)
        {
            int intEnumValue = Convert.ToInt32(enuEnum);
            base.Write(intEnumValue);
        }

        public override void Write(bool value)
        {
            Bits.Add(value);
        }

        public override void Write(string strValue)
        {
            bool blnIsNull = (strValue == null);
            Write(blnIsNull);

            if (blnIsNull == false)
            {
                base.Write(strValue);
            }
        }

        public void WriteGuid(Guid objGuid)
        {
            bool blnIsEmpty = (objGuid == Guid.Empty);
            Write(blnIsEmpty);

            if (blnIsEmpty == false)
            {
                byte[] objGuidBytes = objGuid.ToByteArray();
                Write(objGuidBytes, 0, 16);
            }
        }

        public void WriteBitArray(BitArray objBitArray)
        {
            byte[] objBytes = objBitArray.GetBytes();

            base.Write(objBytes.Length);
            Write(objBytes, 0, objBytes.Length);
        }

        public void WriteByteArray(byte[] objByteArray)
        {
            bool blnIsNull = (objByteArray == null);
            Write(blnIsNull);

            if (blnIsNull == false)
            {
                base.Write(objByteArray.Length);
                Write(objByteArray, 0, objByteArray.Length);
            }
        }

        public void WriteDateTime(DateTime objDateTime)
        {
            Write(objDateTime.ToBinary());
        }

        public void WriteTimeSpan(TimeSpan objTimeSpan)
        {
            Write(objTimeSpan.Ticks);
        }

        public void WriteEncryptedString(string strString)
        {
            Write(EncryptionBase.TripleDES.Encrypt(strString));
        }

        public void WriteObject(object objObject)
        {
            TransportableDataType enuTransportableDataType = TransportableDataTypeHelper.GetTypeFromObject(objObject);
            WriteObject(objObject, enuTransportableDataType, true);
        }

        public void WriteObject(object objObject, TransportableDataType enuTransportableDataType)
        {
            WriteObject(objObject, enuTransportableDataType, true);
        }

        public void WriteObject(object objObject, TransportableDataType enuTransportableDataType, bool blnIncludeType)
        {
            if (blnIncludeType == true)
            {
                Byte bytEnumValue = Convert.ToByte(enuTransportableDataType);
                base.Write(bytEnumValue);
            }
            switch (enuTransportableDataType)
            {
                case (TransportableDataType.Unknown):
                    throw new ArgumentException(string.Format("The argument's type ({0}) is not serializable.", objObject.GetType().FullName));

                case (TransportableDataType.Null):
                    break;

                case (TransportableDataType.Boolean):
                    Write((bool)objObject);
                    break;

                case (TransportableDataType.Byte):
                    Write((byte)objObject);
                    break;

                case (TransportableDataType.ByteArray):
                    WriteByteArray((byte[])objObject);
                    break;

                case (TransportableDataType.Char):
                    Write((char)objObject);
                    break;

                case (TransportableDataType.DateTime):
                    WriteDateTime((DateTime)objObject);
                    break;

                case (TransportableDataType.Decimal):
                    Write((decimal)objObject);
                    break;

                case (TransportableDataType.Double):
                    Write((double)objObject);
                    break;

                case (TransportableDataType.Guid):
                    WriteGuid((Guid)objObject);
                    break;

                case (TransportableDataType.Enum):
                    WriteEnum((Enum)objObject);
                    break;

                case (TransportableDataType.Int16):
                    Write((Int16)objObject);
                    break;

                case (TransportableDataType.Int32):
                    Write((Int32)objObject);
                    break;

                case (TransportableDataType.Int64):
                    Write((Int64)objObject);
                    break;

                case (TransportableDataType.ITransportableObject):
                    WriteTransportableObject((ITransportableObject)objObject);
                    break;

                case (TransportableDataType.SByte):
                    Write((sbyte)objObject);
                    break;

                case (TransportableDataType.Single):
                    Write((Single)objObject);
                    break;

                case (TransportableDataType.String):
                    Write((string)objObject);
                    break;

                case (TransportableDataType.UInt16):
                    Write((UInt16)objObject);
                    break;

                case (TransportableDataType.UInt32):
                    Write((UInt32)objObject);
                    break;

                case (TransportableDataType.UInt64):
                    Write((UInt64)objObject);
                    break;

                case (TransportableDataType.TimeSpan):
                    WriteTimeSpan((TimeSpan)objObject);
                    break;

                case (TransportableDataType.Object):

                    bool blnIsNull = (objObject == null);
                    Write(blnIsNull);

                    if (blnIsNull == false)
                    {
                        using (MemoryStream objMemoryStream = new MemoryStream())
                        {
                            BinaryFormatter objBinaryFormatter = new BinaryFormatter();
                            objBinaryFormatter.Serialize(objMemoryStream, objObject);

                            byte[] bytObjectData = objMemoryStream.ToArray();
                            WriteByteArray(bytObjectData);
                        }
                    }
                    break;
            }

        }

        public void WriteTransportableObject(ITransportableObject objObject)
        {
            bool blnIsNull = (objObject == null);
            Write(blnIsNull);

            if (blnIsNull == false)
            {
                byte[] bytTransportableObjectData = (objObject).Compress();
                WriteByteArray(bytTransportableObjectData);
            }
        }

    }
}
