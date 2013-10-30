namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Diagnostics;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Security.Encryption;
    using ReLi.Framework.Library.Remoting;

    #endregion

    public class BinaryReaderExtension : BinaryReader
    {
        private int _intBitsIndex = -1;
        private List<bool> _objBits = null;

        public BinaryReaderExtension(Stream objStreamInput)
            : base(objStreamInput)
        {
            LoadBits();
        }

        public BinaryReaderExtension(Stream objStreamInput, Encoding enuEncoding)
            : base(objStreamInput, enuEncoding)
        {
            LoadBits();
        }

        private void LoadBits()
        {
            BaseStream.Position = BaseStream.Length - sizeof(int);

            byte[] bytArrayLength = new byte[sizeof(int)];
            BaseStream.Read(bytArrayLength, 0, bytArrayLength.Length);
            int intBytesLength = BitConverter.ToInt32(bytArrayLength, 0);
            if (intBytesLength > 0)
            {
                BaseStream.Position = BaseStream.Length - intBytesLength - sizeof(int);

                byte[] bytBitsData = new byte[intBytesLength];
                BaseStream.Read(bytBitsData, 0, bytBitsData.Length);

                BitArray objBitArray = new BitArray(bytBitsData);
                bool[] objBoolArray = new bool[objBitArray.Count];
                objBitArray.CopyTo(objBoolArray, 0);

                Bits.Clear();
                Bits.AddRange(objBoolArray);

                BaseStream.SetLength(BaseStream.Length - intBytesLength - sizeof(int));
                BaseStream.Position = 0;
            }
            else
            {
                BaseStream.SetLength(BaseStream.Length - sizeof(int));
                BaseStream.Position = 0;
            }
        }

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

        public TEnumType ReadEnum<TEnumType>()
        {
            if ((typeof(TEnumType).IsEnum == false))
            {
                throw new ArgumentException("TEnumType must be an enumerated type.");
            }

            TEnumType enuValue = (TEnumType)((object)ReadInt32());
            return enuValue;
        }

        public override bool ReadBoolean()
        {
            _intBitsIndex++;
            return Bits[_intBitsIndex];
        }

        public override string ReadString()
        {
            string strValue = null;

            bool blnIsNull = ReadBoolean();
            if (blnIsNull == false)
            {
                strValue = base.ReadString();
            }

            return strValue;
        }

        public Guid ReadGuid()
        {
            Guid objGuid = Guid.Empty;
            bool blnIsEmpty = ReadBoolean();
            if (blnIsEmpty == false)
            {
                byte[] bytBytes = ReadBytes(16); /// Guid is always 16 bytes
                objGuid = new Guid(bytBytes);
            }
            return objGuid;
        }

        public BitArray ReadBitArray()
        {
            int intByteCount = ReadInt32();
            byte[] bytBytes = ReadBytes(intByteCount);

            BitArray objBitArray = new BitArray(bytBytes);
            return objBitArray;
        }

        public byte[] ReadByteArray()
        {
            byte[] bytBytes = null;

            bool blnIsNull = ReadBoolean();
            if (blnIsNull == false)
            {
                int intByteCount = ReadInt32();
                bytBytes = ReadBytes(intByteCount);
            }

            return bytBytes;
        }

        public DateTime ReadDateTime()
        {
            Int64 intValue = ReadInt64();
            DateTime objDateTime = DateTime.FromBinary(intValue);
            return objDateTime;
        }

        public TimeSpan ReadTimeSpan()
        {
            Int64 intValue = ReadInt64();
            return TimeSpan.FromTicks(intValue);
        }

        public object ReadObject()
        {
            TransportableDataType enuDataType = (TransportableDataType)ReadByte();
            return ReadObject(enuDataType);
        }

        public object ReadObject(TransportableDataType enuDataType)
        {
            object objValue = null;

            switch (enuDataType)
            {
                case (TransportableDataType.Null):
                    break;

                case (TransportableDataType.Boolean):
                    objValue = ReadBoolean();
                    break;

                case (TransportableDataType.Byte):
                    objValue = ReadByte();
                    break;

                case (TransportableDataType.ByteArray):
                    objValue = ReadByteArray();
                    break;

                case (TransportableDataType.Char):
                    objValue = ReadChar();
                    break;

                case (TransportableDataType.DateTime):
                    objValue = ReadDateTime();
                    break;

                case (TransportableDataType.Decimal):
                    objValue = ReadDecimal();
                    break;

                case (TransportableDataType.Double):
                    objValue = ReadDouble();
                    break;

                case (TransportableDataType.Guid):
                    objValue = ReadGuid();
                    break;

                case (TransportableDataType.Enum):
                    objValue = ReadInt32();
                    break;

                case (TransportableDataType.Int16):
                    objValue = ReadInt16();
                    break;

                case (TransportableDataType.Int32):
                    objValue = ReadInt32();
                    break;

                case (TransportableDataType.Int64):
                    objValue = ReadInt64();
                    break;

                case (TransportableDataType.ITransportableObject):
                    objValue = ReadTransportableObject<ITransportableObject>();
                    break;

                case (TransportableDataType.SByte):
                    objValue = ReadSByte();
                    break;

                case (TransportableDataType.Single):
                    objValue = ReadSingle();
                    break;

                case (TransportableDataType.String):
                    objValue = ReadString();
                    break;

                case (TransportableDataType.UInt16):
                    objValue = ReadUInt16();
                    break;

                case (TransportableDataType.UInt32):
                    objValue = ReadUInt32();
                    break;

                case (TransportableDataType.UInt64):
                    objValue = ReadUInt64();
                    break;

                case (TransportableDataType.TimeSpan):
                    objValue = new TimeSpan(ReadInt64());
                    break;

                case (TransportableDataType.Object):

                    bool blnIsNull = ReadBoolean();
                    if (blnIsNull == false)
                    {
                        byte[] bytObjectData = ReadByteArray();
                        using (MemoryStream objMemoryStream = new MemoryStream(bytObjectData))
                        {
                            BinaryFormatter objBinaryFormatter = new BinaryFormatter();
                            objValue = objBinaryFormatter.Deserialize(objMemoryStream);
                        }
                    }

                    break;
            }

            return objValue;
        }

        public string ReadEncryptedString()
        {
            return EncryptionBase.TripleDES.Decrypt(ReadString());
        }

        public TTransportableObjectType ReadTransportableObject<TTransportableObjectType>()
            where TTransportableObjectType : ITransportableObject
        {
            ITransportableObject objValue = null;

            bool blnIsNull = ReadBoolean();
            if (blnIsNull == false)
            {
                byte[] bytTransportableObjectData = ReadByteArray();
                objValue = TransportableObject.Expand(bytTransportableObjectData);
            }

            return (TTransportableObjectType)objValue;
        }
    }
}
