namespace ReLi.Framework.Library.Net
{
    #region Using Declarations

    using System;
    using System.Net;
    using System.IO;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Security;

    #endregion

    public class ProxySettings : ObjectBase
    {
        private bool _blnEnabled;
        private string _strAddress;
        private int _intPort;
        private bool _blnBypassProxyOnLocal;
        private bool _blnUseDefaultCredentials;
        private Credentials _objCredentials;

        public ProxySettings(string strAddress, int intPort)
            : this(strAddress, intPort, true, false)
        { }

        public ProxySettings(string strAddress, int intPort, bool blnBypassProxyOnLocal, bool blnUseDefaultCredentials)
            : this(strAddress, intPort, blnBypassProxyOnLocal, blnUseDefaultCredentials, ((blnUseDefaultCredentials == false) ? null : System.Net.CredentialCache.DefaultNetworkCredentials))
        {}

        public ProxySettings(string strAddress, int intPort, bool blnBypassProxyOnLocal, bool blnUseDefaultCredentials, System.Net.NetworkCredential objCredentials)
            : base()
        {
            Enabled = true;
            Address = strAddress;
            Port = intPort;
            BypassProxyOnLocal = blnBypassProxyOnLocal;
            UseDefaultCredentials = blnUseDefaultCredentials;

            if (objCredentials != null)
            {
                Credentials = new Security.Credentials(objCredentials);
            }
        }

        public ProxySettings(string strAddress, int intPort, bool blnBypassProxyOnLocal, bool blnUseDefaultCredentials, Credentials objCredentials)
            : base()
        {
            Enabled = true;
            Address = strAddress;
            Port = intPort;
            BypassProxyOnLocal = blnBypassProxyOnLocal;
            UseDefaultCredentials = blnUseDefaultCredentials;
            Credentials = objCredentials;
        }

        public ProxySettings(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public ProxySettings(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public bool Enabled
        {
            get
            {
                return _blnEnabled;
            }
            set
            {
                _blnEnabled = value;
            }
        }

        public string Address
        {
            get
            {
                return _strAddress;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Address", "A valid non-null string is required.");
                }

                _strAddress = value;
            }
        }

        public int Port
        {
            get
            {
                return _intPort;
            }
            set
            {
                _intPort = value;
            }
        }

        public Credentials Credentials
        {
            get
            {
                return _objCredentials;
            }
            set
            {
                _objCredentials = value;
            }
        }

        public bool BypassProxyOnLocal
        {
            get
            {
                return _blnBypassProxyOnLocal;
            }
            set
            {
                _blnBypassProxyOnLocal = value;
            }
        }

        public bool UseDefaultCredentials
        {
            get
            {
                return _blnUseDefaultCredentials;
            }
            set
            {
                _blnUseDefaultCredentials = value;
            }
        }

        public System.Net.WebProxy CreateWebProxy()
        {
            System.Net.WebProxy objWebProxy = new System.Net.WebProxy(Address, Port);
            objWebProxy.BypassProxyOnLocal = BypassProxyOnLocal;
            objWebProxy.UseDefaultCredentials = UseDefaultCredentials;
            if (UseDefaultCredentials == true)
            {
                objWebProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }
            else
            {
                objWebProxy.Credentials = ((Credentials != null) ? Credentials.CreateNetworkCredentials() : null);
            }

            return objWebProxy;
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Enabled", Enabled);
            objSerializedObject.Values.Add("Address", Address);
            objSerializedObject.Values.Add("Port", Port);
            objSerializedObject.Objects.Add("Credentials", Credentials);
            objSerializedObject.Values.Add("BypassProxyOnLocal", BypassProxyOnLocal);
            objSerializedObject.Values.Add("UseDefaultCredentials", UseDefaultCredentials);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Enabled = objSerializedObject.Values.GetValue<bool>("Enabled", true);
            Address = objSerializedObject.Values.GetValue<string>("Address", string.Empty);
            Port = objSerializedObject.Values.GetValue<int>("Port", 0);
            Credentials = objSerializedObject.Objects.GetObject<Credentials>("Credentials", null);
            BypassProxyOnLocal = objSerializedObject.Values.GetValue<bool>("BypassProxyOnLocal", false);
            UseDefaultCredentials = objSerializedObject.Values.GetValue<bool>("UseDefaultCredentials", false);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Enabled);
            objBinaryWriter.Write(Address);
            objBinaryWriter.Write(Port);
            objBinaryWriter.WriteTransportableObject(Credentials);
            objBinaryWriter.Write(BypassProxyOnLocal);
            objBinaryWriter.Write(UseDefaultCredentials);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Enabled = objBinaryReader.ReadBoolean();
            Address = objBinaryReader.ReadString();
            Port = objBinaryReader.ReadInt32();
            Credentials = objBinaryReader.ReadTransportableObject<Credentials>();
            BypassProxyOnLocal = objBinaryReader.ReadBoolean();
            UseDefaultCredentials = objBinaryReader.ReadBoolean();
        }

        #endregion

        #region Static Members

        private static volatile bool _blnHasBeenInitialized = false;
        private static ProxySettings _objDefaultProxySettings = null;
        private static System.Net.WebProxy _objDefaultWebProxy = null;

        static ProxySettings()
        {
            Initialize();
        }

        public static void Initialize()
        {
            if (_blnHasBeenInitialized == false)
            {
                System.Net.WebRequest.DefaultWebProxy = null;
                System.Net.ServicePointManager.Expect100Continue = false;
                System.Net.ServicePointManager.CheckCertificateRevocationList = false;

                _objDefaultProxySettings = Load();

                _objDefaultWebProxy = null;
                if ((_objDefaultProxySettings != null) && (_objDefaultProxySettings.Enabled == true))
                {
                    _objDefaultWebProxy = _objDefaultProxySettings.CreateWebProxy();
                }

                WebRequest.DefaultWebProxy = _objDefaultWebProxy;
                _blnHasBeenInitialized = true;
            }

        }

        public static ProxySettings Load()
        {
            string strSettingsFileName = "proxy.settings.xml";
            string strSettingsFilePath = Path.Combine(ApplicationManager.ApplicationDirectory, strSettingsFileName);

            return Load(strSettingsFilePath);
        }

        public static ProxySettings Load(string strSettingsFilePath)
        {
            if ((strSettingsFilePath == null) || (strSettingsFilePath.Length == 0))
            {
                throw new ArgumentOutOfRangeException("strSettingsFilePath", "A valid non-null, non-empty string is required.");
            }
         
            ProxySettings objSettings = null;
            if (FileManager.Exists(strSettingsFilePath) == true)
            {
                objSettings = ProxySettings.DeserializeFromFile<ProxySettings>(FormatterManager.XmlFormatter, strSettingsFilePath);
            }

            return objSettings;
        }

        public static ProxySettings DefaultProxySettings
        {
            get
            {
                return _objDefaultProxySettings;
            }
        }

        public static System.Net.WebProxy DefaultProxy
        {
            get
            {
                return _objDefaultWebProxy;
            }
        }

        #endregion
    }
}
