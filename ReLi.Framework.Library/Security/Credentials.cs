namespace ReLi.Framework.Library.Security
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Security.Principal;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Security.Encryption;

    #endregion
        
    public class Credentials : ObjectBase
    {
        private string _strUserName;
        private string _strPassword;
        private string _strDoamin;

        public Credentials(NetworkCredential objCredentials)
            : base()
        {
            UserName = objCredentials.UserName;
            Password = objCredentials.Password;
            Domain = objCredentials.Domain;
        }

        public Credentials()
            : this(string.Empty, string.Empty, string.Empty)
        {}

        public Credentials(string strUserName)
            : this(strUserName, string.Empty, string.Empty)
        {}

        public Credentials(string strUserName, string strPassword)
            : this(strUserName, strPassword, string.Empty)
        {}

        public Credentials(string strUserName, string strPassword, string strDomain)
            : base()
        {
            UserName = strUserName;
            Password = strPassword;
            Domain = strDomain;
        }

        public Credentials(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public Credentials(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string UserName
        {
            get
            {
                return _strUserName;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("UserName", "A valid non-null string is required.");
                }

                _strUserName = value;
            }
        }

        public string Password
        {
            get
            {
                return _strPassword;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Password", "A valid non-null string is required.");
                }

                _strPassword = value;
            }
        }

        public string Domain
        {
            get
            {
                return _strDoamin;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Domain", "A valid non-null string is required.");
                }

                _strDoamin = value;
            }
        }

        public string FullUserName
        {
            get
            {
                string strFullUserName = UserName;
                if (Domain.Length > 0)
                {
                    strFullUserName = Domain + "\\" + UserName;
                }

                return strFullUserName;
            }
        }

        public string GetBasicAuthenticationString()
        {
            string strCredentials = String.Format("{0}:{1}", UserName, Password);
            byte[] byteCredentials = System.Text.ASCIIEncoding.ASCII.GetBytes(strCredentials);
            string strEncodedCredentials = Convert.ToBase64String(byteCredentials);

            string strBasicAuthenticationString = String.Format("Basic {0}", strEncodedCredentials);
            return strBasicAuthenticationString;
        }

        public NetworkCredential CreateNetworkCredentials()
        {
            return new NetworkCredential(UserName, Password, Domain);
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("UserName", UserName, true);
            objSerializedObject.Values.Add("Password", Password, true);
            objSerializedObject.Values.Add("Domain", Domain, true);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            UserName = objSerializedObject.Values.GetValue<string>("UserName", string.Empty);
            Password = objSerializedObject.Values.GetValue<string>("Password", string.Empty);
            Domain = objSerializedObject.Values.GetValue<string>("Domain", string.Empty);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteEncryptedString(UserName);
            objBinaryWriter.WriteEncryptedString(Password);
            objBinaryWriter.WriteEncryptedString(Domain);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            UserName = objBinaryReader.ReadEncryptedString();
            Password = objBinaryReader.ReadEncryptedString();
            Domain = objBinaryReader.ReadEncryptedString();
        }

        #endregion

        #region Static Members 

        private static object _objSyncObject = new object();
        private static Credentials _objEmptyCredentials = null;

        public static Credentials CreateFromBasicAuthenticationString(string strBasicAuthentication)
        {
            Credentials objCredentials = null;

            if ((strBasicAuthentication != null) && (strBasicAuthentication.Length > 0))
            {
                string[] strAuthenticationParts = strBasicAuthentication.Split(new char[]{' '});
                if (strAuthenticationParts.Length > 1)
                {
                    string strEncodedCredentials = strAuthenticationParts[1];
                    byte[] bytCredentials = Convert.FromBase64String(strEncodedCredentials);
                    string strCredentials = System.Text.ASCIIEncoding.ASCII.GetString(bytCredentials);
                    
                    string[] strCredentialParts = strCredentials.Split(new char[] { ':' });
                    if (strCredentialParts.Length == 2)
                    {
                        objCredentials = new Credentials(strCredentialParts[0], strCredentialParts[1]);
                    }
                }
            }

            return objCredentials;
        }
                
        public static Credentials Empty
        {
            get
            {
                if (_objEmptyCredentials == null)
                {
                    lock (_objSyncObject)
                    {
                        if (_objEmptyCredentials == null)
                        {
                            _objEmptyCredentials = new Credentials();
                        }
                    }
                }

                return _objEmptyCredentials;
            }
        }

        #endregion
    }
}
