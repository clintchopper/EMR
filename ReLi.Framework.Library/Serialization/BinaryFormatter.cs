namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Xml;
    using System.Security;
    using System.Reflection;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.XmlLite;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Security.Hash;
    using ReLi.Framework.Library.Security.Encryption;
    using ReLi.Framework.Library.Compression;

    #endregion

    public class BinaryFormatter : IFormatter
    {
        private const byte ObjectRecordType = 1;
        private const byte ValueRecordType = 2;
        private const byte EndObjectRecordType = 3;
        private const byte EndDataType = 4;
        private const int CompressionThreshold = 10000;

        public BinaryFormatter()
        { }

        public void SerializeToFile(SerializedObject objSerializedObject, string strFilePath)
        {
            if (objSerializedObject == null)
            {
                throw new ArgumentNullException("objSerializedObject", "A valid non-null SerializedObject is required.");
            }

            byte[] bytData = SerializeToByteArray(objSerializedObject);
            using (FileStream objFileStream = new FileStream(strFilePath, FileMode.Create, FileAccess.Write))
            {
                objFileStream.Write(bytData, 0, bytData.Length);
            }
        }

        public void SerializeToStream(SerializedObject objSerializedObject, Stream objStream)
        {
            if (objSerializedObject == null)
            {
                throw new ArgumentNullException("objSerializedObject", "A valid non-null SerializedObject is required.");
            }
            if (objStream == null)
            {
                throw new ArgumentNullException("objStream", "A valid non-null Stream is required.");
            }

            byte[] bytData = SerializeToByteArray(objSerializedObject);
            using (MemoryStream objMemoryStream = new MemoryStream(bytData))
            {
                objStream.Write(bytData, 0, bytData.Length);
            }
        }

        public byte[] SerializeToByteArray(SerializedObject objSerializedObject)
        {
            byte[] bytData = null;

            using (MemoryStream objTempStream = new MemoryStream())
            {
                BinaryFormatterKeyManager objKeyManager = new BinaryFormatterKeyManager();
                BinaryWriter objBinaryRecordWriter = new BinaryWriter(objTempStream);
                Serialize(objSerializedObject, objBinaryRecordWriter, objKeyManager);
                objBinaryRecordWriter.Write(EndDataType);

                using (MemoryStream objMemoryStream = new MemoryStream())
                {
                    using (BinaryWriter objBinaryWriter = new BinaryWriter(objMemoryStream))
                    {
                        objKeyManager.SerializeToStream(objBinaryWriter);
                        objBinaryWriter.Write(objTempStream.ToArray());
                    }
                    bytData = objMemoryStream.ToArray();
                }
            }

            byte[] bytReturnData = bytData;

            if (bytData.Length >= CompressionThreshold)
            {
                bytReturnData = QuickLZ.compress(bytData, 1);
            }

            return bytReturnData;
        }

        private void Serialize(SerializedObject objSerializedObject, BinaryWriter objBinaryWriter, BinaryFormatterKeyManager objKeyManager)
        {
            string strElementName = objKeyManager.Element.GetKey(objSerializedObject.Name);
            string strTypeName = objKeyManager.Type.GetKey(objSerializedObject.TypeInfo.TypeName);
            string strAssemblyName = objKeyManager.Assembly.GetKey(objSerializedObject.TypeInfo.AssemblyName);

            objBinaryWriter.Write(ObjectRecordType);
            objBinaryWriter.Write(strElementName);
            objBinaryWriter.Write(strTypeName);
            objBinaryWriter.Write(strAssemblyName);

            foreach (SerializedValue objChildValue in objSerializedObject.Values.ToArray())
            {
                string strChildName = objKeyManager.Element.GetKey(objChildValue.Name);

                objBinaryWriter.Write(ValueRecordType);
                objBinaryWriter.Write(strChildName);
                objBinaryWriter.Write((byte)objChildValue.ValueType);
                objBinaryWriter.Write(objChildValue.Encrypted);
                objBinaryWriter.Write(objChildValue.IsNull);

                if (objChildValue.IsNull == false)
                {
                    switch (objChildValue.ValueType)
                    {
                        case (SerializedValueType.Boolean):
                            objBinaryWriter.Write(objChildValue.GetBoolean());
                            break;

                        case (SerializedValueType.Byte):
                            objBinaryWriter.Write(objChildValue.GetByte());
                            break;

                        case (SerializedValueType.ByteArray):
                            byte[] objBytes = objChildValue.GetByteArray();
                            objBinaryWriter.Write(objBytes.Length);
                            objBinaryWriter.Write(objBytes, 0, objBytes.Length);
                            break;

                        case (SerializedValueType.Char):
                            objBinaryWriter.Write(objChildValue.GetChar());
                            break;

                        case (SerializedValueType.DateTime):
                            objBinaryWriter.Write(objChildValue.GetDateTime().ToBinary());
                            break;

                        case (SerializedValueType.Decimal):
                            objBinaryWriter.Write(objChildValue.GetDecimal());
                            break;

                        case (SerializedValueType.Double):
                            objBinaryWriter.Write(objChildValue.GetDouble());
                            break;

                        case (SerializedValueType.Enum):
                            objBinaryWriter.Write(objChildValue.GetInt32());
                            break;

                        case (SerializedValueType.Guid):
                            byte[] objGuidBytes = objChildValue.GetGuid().ToByteArray();
                            objBinaryWriter.Write(objGuidBytes.Length);
                            objBinaryWriter.Write(objGuidBytes, 0, objGuidBytes.Length);
                            break;

                        case (SerializedValueType.Int16):
                            objBinaryWriter.Write(objChildValue.GetInt16());
                            break;

                        case (SerializedValueType.Int32):
                            objBinaryWriter.Write(objChildValue.GetInt32());
                            break;

                        case (SerializedValueType.Int64):
                            objBinaryWriter.Write(objChildValue.GetInt64());
                            break;

                        case (SerializedValueType.Null):
                            break;

                        case (SerializedValueType.SByte):
                            objBinaryWriter.Write(objChildValue.GetSByte());
                            break;

                        case (SerializedValueType.Single):
                            objBinaryWriter.Write(objChildValue.GetSingle());
                            break;

                        case (SerializedValueType.String):
                            string strValue = objChildValue.GetString();
                            if (objChildValue.Encrypted == true)
                            {
                                strValue = EncryptionBase.Instance.Encrypt(strValue);
                            }

                            objBinaryWriter.Write(strValue);
                            break;

                        case (SerializedValueType.UInt16):
                            objBinaryWriter.Write(objChildValue.GetUInt16());
                            break;

                        case (SerializedValueType.UInt32):
                            objBinaryWriter.Write(objChildValue.GetUInt32());
                            break;

                        case (SerializedValueType.UInt64):
                            objBinaryWriter.Write(objChildValue.GetUInt64());
                            break;

                        case (SerializedValueType.Timespan):
                            objBinaryWriter.Write(objChildValue.GetTimespan().ToString());
                            break;

                        default:
                            string strErrorMessage = string.Format("No case handler is defined for SerializedValueType.{0}", objChildValue.ValueType);
                            throw new Exception(strErrorMessage);
                    }
                }
            }

            foreach (SerializedObject objChildObject in objSerializedObject.Objects.ToArray())
            {
                Serialize(objChildObject, objBinaryWriter, objKeyManager);
            }

            objBinaryWriter.Write(EndObjectRecordType);
        }

        public SerializedObject DeserializeFromFile(string strFilePath)
        {
            if (strFilePath == null)
            {
                throw new ArgumentNullException("strFilePath", "A valid non-null string is required.");
            }

            SerializedObject objSerializedObject = null;
            using (FileStream objFileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
            {
                objSerializedObject = DeserializeFromStream(objFileStream);
            }

            return objSerializedObject;
        }

        public SerializedObject DeserializeFromStream(Stream objStream)
        {
            if (objStream == null)
            {
                throw new ArgumentNullException("objStream", "A valid non-null Stream is required.");
            }

            byte[] bytData = ConversionHelper.ToByteArray(objStream);
            SerializedObject objSerializedObject = DeserializeFromByteArray(bytData);

            return objSerializedObject;
        }

        public SerializedObject DeserializeFromByteArray(byte[] bytData)
        {
            if (bytData == null)
            {
                throw new ArgumentNullException("bytData", "A valid non-null byte[] is required.");
            }

            byte[] bytDecompressedData = bytData;
            if (QuickLZ.headerLen(bytDecompressedData) == QuickLZ.DEFAULT_HEADERLEN)
            {
                bytDecompressedData = QuickLZ.decompress(bytDecompressedData);
            }

            SerializedObject objSerializedObject = null;

            using (MemoryStream objMemoryStream = new MemoryStream(bytDecompressedData))
            {
                using (BinaryReader objBinaryReader = new BinaryReader(objMemoryStream))
                {
                    BinaryFormatterKeyManager objKeyManager = new BinaryFormatterKeyManager(objBinaryReader);
                    objSerializedObject = Deserialize(objBinaryReader, objKeyManager);
                }
            }

            return objSerializedObject;
        }

        private SerializedObject Deserialize(BinaryReader objBinaryReader, BinaryFormatterKeyManager objKeyManager)
        {
            SerializedObject objRootObject = null;
            SerializedObject objCurrentObject = null;
            Stack<SerializedObject> objObjectStack = new Stack<SerializedObject>();

            byte objDataType = 0;
            while ((objDataType = objBinaryReader.ReadByte()) != EndDataType)
            {
                switch (objDataType)
                {
                    case (ObjectRecordType):

                        string strObjectName = objKeyManager.Element.FindValueByKey(objBinaryReader.ReadString());
                        string strObjectTypeName = objKeyManager.Type.FindValueByKey(objBinaryReader.ReadString());
                        string strObjectAssessmblyName = objKeyManager.Assembly.FindValueByKey(objBinaryReader.ReadString());

                        objCurrentObject = new SerializedObject(strObjectName, new SerializedTypeInfo(strObjectTypeName, strObjectAssessmblyName));
                        if (objRootObject == null)
                        {
                            objRootObject = objCurrentObject;
                            objObjectStack.Push(objCurrentObject);
                        }
                        else
                        {
                            SerializedObject objParentObject = objObjectStack.Peek();
                            objParentObject.Objects.Add(objCurrentObject);
                            objObjectStack.Push(objCurrentObject);
                        }

                        break;

                    case (ValueRecordType):

                        string strValueName = objKeyManager.Element.FindValueByKey(objBinaryReader.ReadString());
                        SerializedValueType enuValueType = (SerializedValueType)objBinaryReader.ReadByte();
                        bool blnValueEncrypted = objBinaryReader.ReadBoolean();
                        bool blnIsNull = objBinaryReader.ReadBoolean();
                        object objValue = null;

                        if (blnIsNull == false)
                        {
                            switch (enuValueType)
                            {
                                case (SerializedValueType.Boolean):
                                    objValue = objBinaryReader.ReadBoolean();
                                    break;

                                case (SerializedValueType.Byte):
                                    objValue = objBinaryReader.ReadByte();
                                    break;

                                case (SerializedValueType.ByteArray):

                                    int intByteCount = objBinaryReader.ReadInt32();
                                    objValue = objBinaryReader.ReadBytes(intByteCount);
                                    break;

                                case (SerializedValueType.Char):
                                    objValue = objBinaryReader.ReadChar();
                                    break;

                                case (SerializedValueType.DateTime):
                                    objValue = DateTime.FromBinary(objBinaryReader.ReadInt64());
                                    break;

                                case (SerializedValueType.Decimal):
                                    objValue = objBinaryReader.ReadDecimal();
                                    break;

                                case (SerializedValueType.Double):
                                    objValue = objBinaryReader.ReadDouble();
                                    break;

                                case (SerializedValueType.Enum):
                                    objValue = objBinaryReader.ReadInt32();
                                    break;

                                case (SerializedValueType.Guid):
                                    int intGuidByteCount = objBinaryReader.ReadInt32();
                                    byte[] bytGuidBytes = objBinaryReader.ReadBytes(intGuidByteCount);
                                    objValue = new Guid(bytGuidBytes);
                                    break;

                                case (SerializedValueType.Int16):
                                    objValue = objBinaryReader.ReadInt16();
                                    break;

                                case (SerializedValueType.Int32):
                                    objValue = objBinaryReader.ReadInt32();
                                    break;

                                case (SerializedValueType.Int64):
                                    objValue = objBinaryReader.ReadInt64();
                                    break;

                                case (SerializedValueType.Null):
                                    objValue = null;
                                    break;

                                case (SerializedValueType.SByte):
                                    objValue = objBinaryReader.ReadSByte();
                                    break;

                                case (SerializedValueType.Single):
                                    objValue = objBinaryReader.ReadSingle();
                                    break;

                                case (SerializedValueType.String):
                                    string strValue = objBinaryReader.ReadString();
                                    if (blnValueEncrypted == true)
                                    {
                                        strValue = EncryptionBase.Instance.Decrypt(strValue);
                                    }
                                    objValue = strValue;
                                    break;

                                case (SerializedValueType.UInt16):
                                    objValue = objBinaryReader.ReadUInt16();
                                    break;

                                case (SerializedValueType.UInt32):
                                    objValue = objBinaryReader.ReadUInt32();
                                    break;

                                case (SerializedValueType.UInt64):
                                    objValue = objBinaryReader.ReadUInt64();
                                    break;

                                case (SerializedValueType.Timespan):
                                    objValue = TimeSpan.Parse(objBinaryReader.ReadString());
                                    break;

                                default:
                                    string strErrorMessage = string.Format("No case handler is defined for SerializedValueType.{0}", enuValueType);
                                    throw new Exception(strErrorMessage);
                            }

                        }

                        SerializedValue objSerializedValue = new SerializedValue(strValueName, objValue, enuValueType, false);
                        objCurrentObject.Values.Add(objSerializedValue);

                        break;

                    case (EndObjectRecordType):

                        objObjectStack.Pop();
                        break;
                }
            }

            return objRootObject;
        }
    }
}
