namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using ReLi.Framework.Library.XmlLite;

    #endregion

    public class BinaryFormatterKeyManager
    {
        private const byte TypeRecordType = 100;
        private const byte AssemblyRecordType = 101;
        private const byte ElementRecordType = 102;
        private const byte EndDataType = 104;

        private BinaryFormatterKeyList _objType;
        private BinaryFormatterKeyList _objAssembly;
        private BinaryFormatterKeyList _objElement;

        public BinaryFormatterKeyManager()
        {
            Type = new BinaryFormatterKeyList();
            Assembly = new BinaryFormatterKeyList();
            Element = new BinaryFormatterKeyList();
        }

        public BinaryFormatterKeyManager(BinaryReader objBinaryReader)
            : this()
        {
            if (objBinaryReader == null)
            {
                throw new ArgumentNullException("objBinaryReader", "The binary reader must represent a valid instance.");
            }

            byte objDataType = 0;
            while ((objDataType = objBinaryReader.ReadByte()) != EndDataType)
            {
                string strKey = objBinaryReader.ReadString();
                string strValue = objBinaryReader.ReadString();

                switch (objDataType)
                {
                    case (TypeRecordType):
                        Type.Add(strKey, strValue);
                        break;

                    case (AssemblyRecordType):
                        Assembly.Add(strKey, strValue);
                        break;

                    case (ElementRecordType):
                        Element.Add(strKey, strValue);
                        break;

                     default:
                        break;
                }
            }
        }

        public BinaryFormatterKeyList Type
        {
            get
            {
                return _objType;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Type", "A valid non-null CompressedXmlKeyList is required.");
                }

                _objType = value;
            }
        }

        public BinaryFormatterKeyList Assembly
        {
            get
            {
                return _objAssembly;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Assembly", "A valid non-null CompressedXmlKeyList is required.");
                }

                _objAssembly = value;
            }
        }

        public BinaryFormatterKeyList Element
        {
            get
            {
                return _objElement;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Element", "A valid non-null CompressedXmlKeyList is required.");
                }

                _objElement = value;
            }
        }

        public void SerializeToStream(BinaryWriter objBinaryWriter)
        {
            foreach (KeyValuePair<string, string> objKeyValuePair in Type.GetKeyValuePairs())
            {
                objBinaryWriter.Write(TypeRecordType);
                objBinaryWriter.Write(objKeyValuePair.Key);
                objBinaryWriter.Write(objKeyValuePair.Value);
            }
            foreach (KeyValuePair<string, string> objKeyValuePair in Assembly.GetKeyValuePairs())
            {
                objBinaryWriter.Write(AssemblyRecordType);
                objBinaryWriter.Write(objKeyValuePair.Key);
                objBinaryWriter.Write(objKeyValuePair.Value);
            }
            foreach (KeyValuePair<string, string> objKeyValuePair in Element.GetKeyValuePairs())
            {
                objBinaryWriter.Write(ElementRecordType);
                objBinaryWriter.Write(objKeyValuePair.Key);
                objBinaryWriter.Write(objKeyValuePair.Value);
            }
            
            objBinaryWriter.Write(EndDataType);
        }
    }
}
