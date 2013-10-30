namespace ReLi.Framework.Library.Net.Mail
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
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;
    using NetMail = System.Net.Mail;

    #endregion
        
	public class SmtpClient : ObjectBase
    {
        #region Constant Declarations

        public static string DefaultSettingsName = "SmtpClient";
        public static string DefaultHost = string.Empty;
        public static int DefaultPort = 25;
        public static bool DefaultEnableSsl = false;
        public static int DefaultTimeOut = 100000;
        public static NetMail.SmtpDeliveryMethod DefaultDeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
        public static bool DefaultUseDefaultCredentials = false;

        #endregion

        private string _strHost;
        private int _intPort;
        private bool _blnEnableSsl;
        private int _intTimeOut;
        private NetMail.SmtpDeliveryMethod _enuDeliveryMethod;
        private bool _blnUseDefaultCredentials;
        private Credentials _objCredentials;

        public SmtpClient()
            : this(DefaultHost, DefaultPort)
        {}

        public SmtpClient(string strHost)
            : this(strHost, DefaultPort)
        {}

        public SmtpClient(string strHost, int intPort)
            : base()
        {
            Host = strHost;
            Port = intPort;
            EnableSsl = DefaultEnableSsl;
            TimeOut = DefaultTimeOut;
            DeliveryMethod = DefaultDeliveryMethod;
            UseDefaultCredentials = DefaultUseDefaultCredentials;
            Credentials = null; 
        }

        public SmtpClient(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public SmtpClient(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string Host
        {
            get
            {
                return _strHost;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Host", "A valid non-null string is required.");
                }

                _strHost = value;
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

        public bool EnableSsl
        {
            get
            {
                return _blnEnableSsl;
            }
            set
            {
                _blnEnableSsl = value;
            }
        }

        public int TimeOut
        {
            get
            {
                return _intTimeOut;
            }
            set
            {
                _intTimeOut = value;
            }
        }

        public NetMail.SmtpDeliveryMethod DeliveryMethod
        {
            get
            {
                return _enuDeliveryMethod;
            }
            set
            {
                _enuDeliveryMethod = value;
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

        protected NetMail.SmtpClient CreateSmtpClient()
        {
            NetMail.SmtpClient objSmtpClient = new NetMail.SmtpClient(Host, Port);
            objSmtpClient.EnableSsl = EnableSsl;
            objSmtpClient.Timeout = TimeOut;
            objSmtpClient.DeliveryMethod = DeliveryMethod;
            objSmtpClient.UseDefaultCredentials = UseDefaultCredentials;

            if (Credentials != null)
            {
                objSmtpClient.Credentials = new NetworkCredential(Credentials.UserName, Credentials.Password, Credentials.Domain);
            }
            else
            {
                objSmtpClient.Credentials = null;
            }

            return objSmtpClient;
        }

        public bool SendMessage(MailMessage objMailMessage, bool blnThrowOnError)
        {
            bool blnResult = true;

            try
            {
                if (objMailMessage == null)
                {
                    throw new ArgumentNullException("objMailMessage", "A valid non-null MailMessage is required.");
                }
                
                NetMail.SmtpClient objSmtpClient = CreateSmtpClient();
                objSmtpClient.Send(objMailMessage.CreateMailMessage());
            }
            catch
            {
                blnResult = false;
                if (blnThrowOnError == true)
                {
                    throw;
                }
            }

            return blnResult;
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Host", Host);
            objSerializedObject.Values.Add("Port", Port);
            objSerializedObject.Values.Add("EnableSsl", EnableSsl);
            objSerializedObject.Values.Add("TimeOut", TimeOut);
            objSerializedObject.Values.Add("DeliveryMethod", DeliveryMethod);
            objSerializedObject.Values.Add("UseDefaultCredentials", UseDefaultCredentials);
            objSerializedObject.Objects.Add("Credentials", Credentials);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Host = objSerializedObject.Values.GetValue<string>("Host", DefaultHost);
            Port = objSerializedObject.Values.GetValue<int>("Port", DefaultPort);
            EnableSsl = objSerializedObject.Values.GetValue<bool>("EnableSsl", DefaultEnableSsl);
            TimeOut = objSerializedObject.Values.GetValue<int>("TimeOut", DefaultTimeOut);
            DeliveryMethod = objSerializedObject.Values.GetValue<NetMail.SmtpDeliveryMethod>("DeliveryMethod", DefaultDeliveryMethod);
            UseDefaultCredentials = objSerializedObject.Values.GetValue<bool>("UseDefaultCredentials", DefaultUseDefaultCredentials);
            Credentials = objSerializedObject.Objects.GetObject<Credentials>("Credentials", null);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Host);
            objBinaryWriter.Write(Port);
            objBinaryWriter.Write(EnableSsl);
            objBinaryWriter.Write(TimeOut);
            objBinaryWriter.Write((byte)DeliveryMethod);
            objBinaryWriter.Write(UseDefaultCredentials);
            objBinaryWriter.WriteTransportableObject(Credentials);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Host = objBinaryReader.ReadString();
            Port = objBinaryReader.ReadInt32();
            EnableSsl = objBinaryReader.ReadBoolean();
            TimeOut = objBinaryReader.ReadInt32();
            DeliveryMethod = (NetMail.SmtpDeliveryMethod)objBinaryReader.ReadByte();
            UseDefaultCredentials = objBinaryReader.ReadBoolean();
            Credentials = objBinaryReader.ReadTransportableObject<Credentials>();
        }

        #endregion

        #region Static Members

        private static object _objSyncObject = new object();
        private static SmtpClient _objInstance = null;

        private static object SyncObject
        {
            get
            {
                return _objSyncObject;
            }
        }

        public static SmtpClient Default
        {
            get
            {
                if (_objInstance == null)
                {
                    lock (SyncObject)
                    {
                        if (_objInstance == null)
                        {
                            _objInstance = ApplicationManager.Settings.GetValue<SmtpClient>(SmtpClient.DefaultSettingsName, null);
                        }
                    }
                }
                return _objInstance;
            }
            set
            {
                _objInstance = value;
            }
        }

        public static SmtpClient LoadFromSettings(string strSettingsName)
        {
            if (Default == null)
            {
                Default = ApplicationManager.Settings.GetValue<SmtpClient>(strSettingsName, null);
            }

            return Default;
        }

        #endregion
    }
}
