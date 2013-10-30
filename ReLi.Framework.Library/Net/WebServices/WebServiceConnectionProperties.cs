namespace ReLi.Framework.Library.Net.WebServices
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Principal;
    using System.Net;
    using System.Net.Security;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Security.Encryption;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;

    #endregion
        
	public class WebServiceConnectionProperties : ObjectBase  
    {
        protected string _strUrl;
        protected Credentials _objCredentials;

        public WebServiceConnectionProperties(string strUrl)
            : this(strUrl, new Credentials())
        {}

        public WebServiceConnectionProperties(string strUrl, string strUserName, string strPassword)
            : this(strUrl, new Credentials(strUserName, strPassword))
        {}

        public WebServiceConnectionProperties(string strUrl, string strUserName, string strPassword, string strDomain)
            : this(strUrl, new Credentials(strUserName, strPassword, strDomain))
        {}

        public WebServiceConnectionProperties(string strUrl, Credentials objCredentials)
            : base()
        {
            Url = strUrl;
            Credentials = objCredentials;
        }

        public WebServiceConnectionProperties(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public WebServiceConnectionProperties(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string Url
        {
            get
            {
                return _strUrl;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Url", "A valid non-null string is required.");
                }

                _strUrl = value;
            }
        }

        public Credentials Credentials
        {
            get
            {
                return _objCredentials;    
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Credentials", "A valid non-null NetworkCredential is required.");
                }

                _objCredentials = value;
            } 
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Url", Url);
            objSerializedObject.Objects.Add("Credentials", Credentials);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Url = objSerializedObject.Values.GetValue<string>("Url", string.Empty);
            Credentials = objSerializedObject.Objects.GetObject<Credentials>("Credentials", null);
        }

        #endregion       

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Url);
            objBinaryWriter.WriteTransportableObject(Credentials);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Url = objBinaryReader.ReadString();
            Credentials = objBinaryReader.ReadTransportableObject<Credentials>();
        }

        #endregion
    }
}
