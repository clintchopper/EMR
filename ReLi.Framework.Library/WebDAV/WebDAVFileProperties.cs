namespace ReLi.Framework.Library.WebDAV
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.XmlLite;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;

    #endregion

    #region WebDAVFileProperties Declarations

    public class WebDAVFileProperties : ObjectBase
    {
        private string _strPath;
        private string _strDisplayName;
        private long _lngSize;
        private DateTime _dtModifiedDate;

        public WebDAVFileProperties(string strRawResponse)
        {
            if (string.IsNullOrEmpty(strRawResponse) == true)
            {
                throw new ArgumentOutOfRangeException("strRawResponse", "A valid non-null, non-empty string is required.");
            }

            XmlLiteDocument objXmlLiteDocument = XmlLiteDocument.LoadFromXml(strRawResponse);

            XmlLiteElement objResponseElement = objXmlLiteDocument.Root.Elements.GetElement("response");
            if (objResponseElement == null)
            {
                throw new Exception("Unable to locate the child element 'response'.");
            }

            XmlLiteElement objHrefElement = objResponseElement.Elements.GetElement("href");
            if (objHrefElement == null)
            {
                throw new Exception("Unable to locate the child element 'response\\href'.");
            }

            XmlLiteElement objPropElement = objResponseElement.Elements.GetElement("propstat").Elements.GetElement("prop");
            if (objPropElement == null)
            {
                throw new Exception("Unable to locate the child element 'response\\propstat\\prop'.");
            }

            Path = objHrefElement.Value;
            DisplayName = objPropElement.Elements.GetElement("displayname").Value;
            Size = Convert.ToInt64(objPropElement.Elements.GetElement("getcontentlength").Value);
            ModifiedDate = Convert.ToDateTime(objPropElement.Elements.GetElement("getlastmodified").Value);

        }

        public WebDAVFileProperties(string strPath, string strDisplayName, long lngSize, DateTime dtModifiedDate)
        {
            Path = strPath;
            DisplayName = strDisplayName;
            Size = lngSize;
            ModifiedDate = dtModifiedDate;
        }

        public WebDAVFileProperties(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public WebDAVFileProperties(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        public string Path
        {
            get
            {
                return _strPath;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Path", "A valid non-null string is required.");
                }

                _strPath = value;
            }
        }

        public string WindowsPath
        {
            get
            {
                string strWindowsPath = Path.TrimStart(new char[] { '/' }).Replace("/", "\\");
                return strWindowsPath;
            }
        }

        public string DisplayName
        {
            get
            {
                return _strDisplayName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("DisplayName", "A valid non-null string is required.");
                }

                _strDisplayName = value;
            }
        }

        public long Size
        {
            get
            {
                return _lngSize;
            }
            set
            {
                _lngSize = value;
            }
        }

        public DateTime ModifiedDate
        {
            get
            {
                return _dtModifiedDate;
            }
            set
            {
                _dtModifiedDate = value;
            }
        }

        #region ICustomerSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Path", Path);
            objSerializedObject.Values.Add("DisplayName", DisplayName);
            objSerializedObject.Values.Add("Size", Size);
            objSerializedObject.Values.Add("ModifiedDate", ModifiedDate);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Path = objSerializedObject.Values.GetValue<string>("Path", null);
            DisplayName = objSerializedObject.Values.GetValue<string>("DisplayName", null);
            Size = objSerializedObject.Values.GetValue<long>("Size", 0);
            ModifiedDate = objSerializedObject.Values.GetValue<DateTime>("ModifiedDate", DateTime.MinValue);
        }

        #endregion

        #region ITransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Path);
            objBinaryWriter.Write(DisplayName);
            objBinaryWriter.Write(Size);
            objBinaryWriter.WriteDateTime(ModifiedDate);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Path = objBinaryReader.ReadString();
            DisplayName = objBinaryReader.ReadString();
            Size = objBinaryReader.ReadInt64();
            ModifiedDate = objBinaryReader.ReadDateTime();
        }

        #endregion
    }

    #endregion

    #region WebDAVFilePropertiesList Declarations

    public class WebDAVFilePropertiesList : ObjectListBase<WebDAVFileProperties>
    {
        public WebDAVFilePropertiesList()
            : base()
        { }

        public WebDAVFilePropertiesList(IEnumerable<WebDAVFileProperties> objItems)
            : base(objItems)
        { }

        public WebDAVFilePropertiesList(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public WebDAVFilePropertiesList(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);
        }

        #endregion
    }

    #endregion

}
