namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Reflection;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.XmlLite;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Security.Hash;
    using ReLi.Framework.Library.Security.Encryption;

    #endregion

    public class XmlFormatter : IFormatter 
    {
        private const string TypeAttributeName = "Type";
        private const string AssemblyAttributeName = "Assembly";
        private const string EncryptedAttributeName = "Encrypted";
        private const string IsNullAttributeName = "IsNull";

        public XmlFormatter()
        {}

        public void SerializeToFile(SerializedObject objSerializedObject, string strFilePath)
        {
            if (objSerializedObject == null)
            {
                throw new ArgumentNullException("objSerializedObject", "A valid non-null SerializedObject is required.");
            }

            XmlLiteElement objRootElement = new XmlLiteElement(objSerializedObject.Name);
            Serialize(objSerializedObject, objRootElement);

            if (File.Exists(strFilePath) == true)
            {
                File.Delete(strFilePath);
            }

            XmlLiteDocument objDocument = new XmlLiteDocument(objRootElement);
            objDocument.ExportToFile(strFilePath);
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

            XmlLiteElement objRootElement = new XmlLiteElement(objSerializedObject.Name);
            Serialize(objSerializedObject, objRootElement);

            XmlLiteDocument objDocument = new XmlLiteDocument(objRootElement);
            objDocument.ExportToStream(objStream);
        }

        public byte[] SerializeToByteArray(SerializedObject objSerializedObject)
        {
            if (objSerializedObject == null)
            {
                throw new ArgumentNullException("objSerializedObject", "A valid non-null SerializedObject is required.");
            }

            XmlLiteElement objRootElement = new XmlLiteElement(objSerializedObject.Name);
            Serialize(objSerializedObject, objRootElement);

            byte[] bytData = null;
            XmlLiteDocument objDocument = new XmlLiteDocument(objRootElement);
            using (MemoryStream objMemoryStream = new MemoryStream())
            {
                objDocument.ExportToStream(objMemoryStream);
                bytData = objMemoryStream.ToArray();
            }

            return bytData;
        }

        private void Serialize(SerializedObject objSerializedObject, XmlLiteElement objXmlElement)
        {
            SerializedTypeInfo objTypeInfo = objSerializedObject.TypeInfo;

            if (objTypeInfo.TypeName.Length > 0)
            {
                objXmlElement.Attributes.Add(TypeAttributeName, objTypeInfo.TypeName);
            }
            if (objTypeInfo.AssemblyName.Length > 0)
            {
                objXmlElement.Attributes.Add(AssemblyAttributeName, objTypeInfo.AssemblyName);
            }

            foreach (SerializedValue objChildValue in objSerializedObject.Values.ToArray())
            {
                string strValue = string.Empty;

                if (objChildValue.IsNull == false)
                {
                    switch (objChildValue.ValueType)
                    {
                        case (SerializedValueType.Boolean):
                            strValue = objChildValue.GetBoolean().ToString();
                            break;

                        case (SerializedValueType.Byte):
                            strValue = objChildValue.GetByte().ToString();
                            break;

                        case (SerializedValueType.ByteArray):

                            byte[] objBytes = objChildValue.GetByteArray();
                            StringBuilder objValue = new StringBuilder();
                            foreach (byte bytValue in objBytes)
                            {
                                objValue.AppendFormat(", {0}", bytValue);
                            }

                            strValue = objValue.ToString();
                            break;

                        case (SerializedValueType.Char):
                            strValue = objChildValue.GetChar().ToString();
                            break;

                        case (SerializedValueType.DateTime):
                            strValue = objChildValue.GetDateTime().ToString();
                            break;

                        case (SerializedValueType.Decimal):
                            strValue = objChildValue.GetDecimal().ToString();
                            break;

                        case (SerializedValueType.Double):
                            strValue = objChildValue.GetDouble().ToString();
                            break;

                        case (SerializedValueType.Guid):
                            strValue = objChildValue.GetGuid().ToString();
                            break;

                        case (SerializedValueType.Int16):
                            strValue = objChildValue.GetInt16().ToString();
                            break;

                        case (SerializedValueType.Int32):
                            strValue = objChildValue.GetInt32().ToString();
                            break;

                        case (SerializedValueType.Int64):
                            strValue = objChildValue.GetInt64().ToString();
                            break;

                        case (SerializedValueType.Null):
                            strValue = string.Empty;
                            break;

                        case (SerializedValueType.SByte):
                            strValue = objChildValue.GetSByte().ToString();
                            break;

                        case (SerializedValueType.Single):
                            strValue = objChildValue.GetSingle().ToString();
                            break;

                        case (SerializedValueType.String):
                            strValue = objChildValue.GetString();
                            break;

                        case (SerializedValueType.Enum):
                            strValue = objChildValue.Value.ToString();
                            break;

                        case (SerializedValueType.UInt16):
                            strValue = objChildValue.GetUInt16().ToString();
                            break;

                        case (SerializedValueType.UInt32):
                            strValue = objChildValue.GetUInt32().ToString();
                            break;

                        case (SerializedValueType.UInt64):
                            strValue = objChildValue.GetUInt64().ToString();
                            break;

                        case (SerializedValueType.Timespan):
                            strValue = objChildValue.GetTimespan().ToString();
                            break;

                        default:
                            string strErrorMessage = string.Format("No case handler is defined for SerializedValueType.{0}", objChildValue.ValueType);
                            throw new Exception(strErrorMessage);
                    }
                }

                if (objChildValue.Encrypted == true)
                {
                    strValue = EncryptionBase.Instance.Encrypt(strValue);
                }
                
                XmlLiteElement objChildElement = new XmlLiteElement(objChildValue.Name, strValue); 
                objChildElement.Attributes.Add(TypeAttributeName, objChildValue.ValueType.ToString());
                
                if (objChildValue.IsNull == true)
                {
                    objChildElement.Attributes.Add(IsNullAttributeName, "true");
                }

                if (objChildValue.Encrypted == true)
                {
                    objChildElement.Attributes.Add(EncryptedAttributeName, "true");
                }

                objXmlElement.Elements.Add(objChildElement);
            }
            foreach (SerializedObject objChildObject in objSerializedObject.Objects.ToArray())
            {
                XmlLiteElement objChildElement = new XmlLiteElement(objChildObject.Name);
                Serialize(objChildObject, objChildElement);
                objXmlElement.Elements.Add(objChildElement);
            }
        }

        public SerializedObject DeserializeFromFile(string strFilePath)
        {
            if (strFilePath == null)
            {
                throw new ArgumentNullException("strFilePath", "A valid non-null string is required.");
            }
            if (FileManager.Exists(strFilePath) == false)
            {
                throw new FileNotFoundException("The file to be deserialized does not exist.", strFilePath);
            }

            XmlLiteDocument objXmlDocument = XmlLiteDocument.LoadFromFile(strFilePath);
            XmlLiteElement objXmlElement = objXmlDocument.Root;

            bool blnContainsAttribute = objXmlElement.Attributes.Exists(AssemblyAttributeName);
            string strAssemblyName = ((blnContainsAttribute == true) ? objXmlElement.Attributes[AssemblyAttributeName].Value : string.Empty);

            blnContainsAttribute = objXmlElement.Attributes.Exists(TypeAttributeName);
            string strTypeName = ((blnContainsAttribute == true) ? objXmlElement.Attributes[TypeAttributeName].Value : string.Empty);

            SerializedObject objSerializedObject = new SerializedObject(objXmlElement.Name, new SerializedTypeInfo(strTypeName, strAssemblyName));
            Deserialize(objXmlElement, objSerializedObject);

            return objSerializedObject;
        }

        public SerializedObject DeserializeFromStream(Stream objStream)
        {
            if (objStream == null)
            {
                throw new ArgumentNullException("objStream", "A valid non-null Stream is required.");
            }

            XmlLiteDocument objXmlDocument = XmlLiteDocument.LoadFromStream(objStream);
            XmlLiteElement objXmlElement = objXmlDocument.Root;

            bool blnContainsAttribute = objXmlElement.Attributes.Exists(AssemblyAttributeName);
            string strAssemblyName = ((blnContainsAttribute == true) ? objXmlElement.Attributes[AssemblyAttributeName].Value : string.Empty);

            blnContainsAttribute = objXmlElement.Attributes.Exists(TypeAttributeName);
            string strTypeName = ((blnContainsAttribute == true) ? objXmlElement.Attributes[TypeAttributeName].Value : string.Empty);

            SerializedObject objSerializedObject = new SerializedObject(objXmlElement.Name, new SerializedTypeInfo(strTypeName, strAssemblyName));
            Deserialize(objXmlElement, objSerializedObject);

            return objSerializedObject;
        }

        public SerializedObject DeserializeFromByteArray(byte[] bytData)
        {
            if (bytData == null)
            {
                throw new ArgumentNullException("bytData", "A valid non-null byte[] is required.");
            }

            XmlLiteDocument objXmlDocument = null;

            using (MemoryStream objMemoryStream = new MemoryStream(bytData))
            {
                objXmlDocument = XmlLiteDocument.LoadFromStream(objMemoryStream);
            }

            XmlLiteElement objXmlElement = objXmlDocument.Root;

            bool blnContainsAttribute = objXmlElement.Attributes.Exists(AssemblyAttributeName);
            string strAssemblyName = ((blnContainsAttribute == true) ? objXmlElement.Attributes[AssemblyAttributeName].Value : string.Empty);

            blnContainsAttribute = objXmlElement.Attributes.Exists(TypeAttributeName);
            string strTypeName = ((blnContainsAttribute == true) ? objXmlElement.Attributes[TypeAttributeName].Value : string.Empty);

            SerializedObject objSerializedObject = new SerializedObject(objXmlElement.Name, new SerializedTypeInfo(strTypeName, strAssemblyName));
            Deserialize(objXmlElement, objSerializedObject);

            return objSerializedObject;
        }

        private void Deserialize(XmlLiteElement objXmlElement, SerializedObject objSerializedObject)
        {
            foreach (XmlLiteElement objChildElement in objXmlElement.Elements.ToArray())
            {
                string strAssemblyName = objChildElement.Attributes.GetValue(AssemblyAttributeName, string.Empty);
                string strTypeName = objChildElement.Attributes.GetValue(TypeAttributeName, string.Empty);

                if ((strTypeName.Length > 0) && (strAssemblyName.Length > 0))
                {
                    SerializedTypeInfo objTypeInfo = new SerializedTypeInfo(strTypeName, strAssemblyName);

                    SerializedObject objChildObject = new SerializedObject(objChildElement.Name, objTypeInfo);
                    Deserialize(objChildElement, objChildObject);
                    objSerializedObject.Objects.Add(objChildObject);
                }
                else if (strTypeName.Length > 0)
                {
                    XmlLiteAttribute objChildAttribute = objChildElement.Attributes[TypeAttributeName];
                    SerializedValueType enuValueType = (SerializedValueType)Enum.Parse(typeof(SerializedValueType), objChildAttribute.Value);

                    bool blnIsNull = Convert.ToBoolean(objChildElement.Attributes.GetValue(IsNullAttributeName, "false"));
                    bool blnIsEncrypted = Convert.ToBoolean(objChildElement.Attributes.GetValue(EncryptedAttributeName, "false"));

                    string strValue = ((blnIsNull == true) ? null : objChildElement.Value);
                    if (blnIsEncrypted == true)
                    {
                        strValue = EncryptionBase.Default.Decrypt(objChildElement.Value);
                    }
                    
                    object objValue = null;
                    switch (enuValueType)
                    {
                        case (SerializedValueType.Boolean):
                            objValue = Convert.ToBoolean(strValue);
                            break;

                        case (SerializedValueType.Byte):
                            objValue = Convert.ToByte(strValue);
                            break;

                        case (SerializedValueType.ByteArray):

                            List<byte> objBytes = new List<byte>();                            
                            string[] strBytes = strValue.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string strByte in strBytes)
                            {
                                objBytes.Add(Convert.ToByte(strByte));
                            }
                            objValue = objBytes.ToArray();
                            break;

                        case (SerializedValueType.Char):
                            objValue = Convert.ToChar(strValue);
                            break;

                        case (SerializedValueType.DateTime):
                            objValue = Convert.ToDateTime(strValue);
                            break;

                        case (SerializedValueType.Decimal):
                            objValue = Convert.ToDecimal(strValue);
                            break;

                        case (SerializedValueType.Double):
                            objValue = Convert.ToDouble(strValue);
                            break;

                        case (SerializedValueType.Guid):
                            objValue = new Guid(strValue);
                            break;

                        case (SerializedValueType.Int16):
                            objValue = Convert.ToInt16(strValue);
                            break;

                        case (SerializedValueType.Int32):

                            // For backwards compatability
                            //
                            int intResult = 0;
                            if (Int32.TryParse(strValue, out intResult) == true)
                            {
                                objValue = Convert.ToInt32(strValue);
                            }
                            else
                            {
                                objValue = strValue;
                            }
                            break;

                        case (SerializedValueType.Int64):
                            objValue = Convert.ToInt64(strValue);
                            break;

                        case (SerializedValueType.Null):
                            objValue = null;
                            break;

                        case (SerializedValueType.SByte):
                            objValue = Convert.ToSByte(strValue);
                            break;

                        case (SerializedValueType.Single):
                            objValue = Convert.ToSingle(strValue);
                            break;

                        case (SerializedValueType.String):
                        case (SerializedValueType.Enum):
                            objValue = strValue;
                            break;

                        case (SerializedValueType.UInt16):
                            objValue = Convert.ToUInt16(strValue);
                            break;

                        case (SerializedValueType.UInt32):
                            objValue = Convert.ToUInt32(strValue);
                            break;

                        case (SerializedValueType.UInt64):
                            objValue = Convert.ToUInt64(strValue);
                            break;

                        case (SerializedValueType.Timespan):
                            objValue = TimeSpan.Parse(strValue);
                            break;

                        default:
                            string strErrorMessage = string.Format("No case handler is defined for SerializedValueType.{0}", enuValueType);
                            throw new Exception(strErrorMessage);
                    }

                    SerializedValue objSerializedValue = new SerializedValue(objChildElement.Name, objValue, enuValueType, blnIsEncrypted);
                    objSerializedObject.Values.Add(objSerializedValue);
                }
                else // This element is just an empty type / container.
                {
                    SerializedObject objChildObject = new SerializedObject(objChildElement.Name, new SerializedTypeInfo());
                    Deserialize(objChildElement, objChildObject);
                    objSerializedObject.Objects.Add(objChildObject);
                }
            }
        }
    }
}
