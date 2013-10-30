namespace ReLi.Framework.Library.Wcf
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Net;
    using System.ServiceModel;
    using System.Security.Principal;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Serialization;
    using System.ServiceModel.Channels;

    #endregion

    public class WcfNetTcpBinding : ObjectBase, IWcfBinding
    {
        private TimeSpan _objCloseTimeout;
        private TimeSpan _objOpenTimeout;
        private TimeSpan _objReceiveTimeout;
        private TimeSpan _objSendTimeOut;
        private HostNameComparisonMode _enuHostNameComparisonMode;        
        private long _lngMaxBufferPoolSize;
        private long _lngMaxReceivedMessageSize;
        private bool _blnPortSharingEnabled;
        private bool _blnTransactionFlow;
        private int _intMaxBufferSize;
        private int _intMaxConnections;
        private int _intListenBacklog;
        private TransferMode _enuTransferMode;
        private WSMessageEncoding _enuMessageEncoding;
        private WcfReaderQuotas _objReaderQuotas;
        private WcfOptionalReliableSession _objReliableSession;
        private SecurityMode _enuSecurityMode;
        
        public WcfNetTcpBinding()
            : this(DefaultCloseTimeout, DefaultOpenTimeout, DefaultReceiveTimeout, DefaultSendTimeout, DefaultHostNameComparisonMode,
            DefaultMaxBufferPoolSize, DefaultMaxReceivedMessageSize, DefaultPortSharingEnabled, DefaultTransactionFlow, DefaultMaxBufferSize, 
            DefaultMaxConnections, DefaultListenBacklog, DefaultTransferMode, DefaultMessageEncoding, DefaultReaderQuotas, DefaultReliableSession, DefaultSecurityMode)
        {}

        public WcfNetTcpBinding(TimeSpan objCloseTimeout, TimeSpan objOpenTimeout, TimeSpan objReceiveTimeout,
            TimeSpan objSendTimeOut, HostNameComparisonMode enuHostNameComparisonMode, long lngMaxBufferPoolSize, long lngMaxReceivedMessageSize, bool blnPortSharingEnabled, 
            bool blnTransactionFlow, int intMaxBufferSize, int intMaxConnections, int intListenBacklog, TransferMode enuTransferMode, WSMessageEncoding enuMessageEncoding,
            WcfReaderQuotas objReaderQuotas, WcfOptionalReliableSession objReliableSession, SecurityMode enuSecurityMode)
            : base()
        {
            CloseTimeout = objCloseTimeout;
            OpenTimeout = objOpenTimeout;
            ReceiveTimeout = objReceiveTimeout;
            SendTimeout = objSendTimeOut;
            HostNameComparisonMode = enuHostNameComparisonMode;
            MaxBufferPoolSize = lngMaxBufferPoolSize;
            MaxReceivedMessageSize = lngMaxReceivedMessageSize;
            PortSharingEnabled = blnPortSharingEnabled;
            TransactionFlow = blnTransactionFlow;
            MaxBufferSize = intMaxBufferSize;
            MaxConnections = intMaxConnections;
            ListenBacklog = intListenBacklog;
            TransferMode = enuTransferMode;
            MessageEncoding = enuMessageEncoding;
            ReliableSession = objReliableSession;
            ReaderQuotas = objReaderQuotas;
        }

        public WcfNetTcpBinding(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public WcfNetTcpBinding(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}
              
        public TimeSpan CloseTimeout
        {
            get
            {
                return _objCloseTimeout;
            }
            set
            {
                _objCloseTimeout = value;
            }
        }

        public TimeSpan OpenTimeout
        {
            get
            {
                return _objOpenTimeout;
            }
            set
            {
                _objOpenTimeout = value;
            }
        }

        public TimeSpan ReceiveTimeout
        {
            get
            {
                return _objReceiveTimeout;
            }
            set
            {
                _objReceiveTimeout = value;
            }
        }

        public TimeSpan SendTimeout
        {
            get
            {
                return _objSendTimeOut;
            }
            set
            {
                _objSendTimeOut = value;
            }
        }

        public HostNameComparisonMode HostNameComparisonMode
        {
            get
            {
                return _enuHostNameComparisonMode;
            }
            set
            {
                _enuHostNameComparisonMode = value;
            }
        }

        public long MaxBufferPoolSize
        {
            get
            {
                return _lngMaxBufferPoolSize;
            }
            set
            {
                _lngMaxBufferPoolSize = value;
            }
        }

        public long MaxReceivedMessageSize
        {
            get
            {
                return _lngMaxReceivedMessageSize;
            }
            set
            {
                _lngMaxReceivedMessageSize = value;
            }
        }

        public bool PortSharingEnabled
        {
            get
            {
                return _blnPortSharingEnabled;
            }
            set
            {
                _blnPortSharingEnabled = value;
            }
        }

        public bool TransactionFlow
        {
            get
            {
                return _blnTransactionFlow;
            }
            set
            {
                _blnTransactionFlow = value;
            }
        }

        public int MaxBufferSize
        {
            get
            {
                return _intMaxBufferSize;
            }
            set
            {
                _intMaxBufferSize = value;
            }
        }

        public int MaxConnections
        {
            get
            {
                return _intMaxConnections;
            }
            set
            {
                _intMaxConnections = value;
            }
        }

        public int ListenBacklog
        {
            get
            {
                return _intListenBacklog;
            }
            set
            {
                _intListenBacklog = value;
            }
        }

        public TransferMode TransferMode
        {
            get
            {
                return _enuTransferMode;
            }
            set
            {
                _enuTransferMode = value;
            }
        }

        public WSMessageEncoding MessageEncoding
        {
            get
            {
                return _enuMessageEncoding;
            }
            set
            {
                _enuMessageEncoding = value;
            }
        }

        public WcfReaderQuotas ReaderQuotas
        {
            get
            {
                if (_objReaderQuotas == null)
                {
                    _objReaderQuotas = new WcfReaderQuotas();
                }

                return _objReaderQuotas;
            }
            private set
            {
                _objReaderQuotas = value;
            }
        }

        public WcfOptionalReliableSession ReliableSession
        {
            get
            {
                if (_objReliableSession == null)
                {
                    _objReliableSession = new WcfOptionalReliableSession();
                }

                return _objReliableSession;
            }
            private set
            {
                _objReliableSession = value;
            }
        }

        public SecurityMode SecurityMode
        {
            get
            {
                return _enuSecurityMode;
            }
            set
            {
                _enuSecurityMode = value;
            }
        }

        #region IWcfBinding Members

        public Binding Binding
        {
            get
            {
                NetTcpBinding objBinding = new NetTcpBinding(SecurityMode.None);
                
                objBinding.CloseTimeout = CloseTimeout;
                objBinding.OpenTimeout = OpenTimeout;
                objBinding.ReceiveTimeout = ReceiveTimeout;
                objBinding.SendTimeout = SendTimeout;
                objBinding.HostNameComparisonMode = HostNameComparisonMode;
                objBinding.MaxBufferPoolSize = MaxBufferPoolSize;
                objBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
                objBinding.MaxBufferSize = MaxBufferSize;
                objBinding.MaxConnections = MaxConnections;
                objBinding.PortSharingEnabled = PortSharingEnabled;
                objBinding.TransactionFlow = TransactionFlow;
                objBinding.TransferMode = TransferMode;
                objBinding.ListenBacklog = ListenBacklog;
                objBinding.ReaderQuotas.MaxDepth = ReaderQuotas.MaxDepth;
                objBinding.ReaderQuotas.MaxStringContentLength = ReaderQuotas.MaxStringContentLength;
                objBinding.ReaderQuotas.MaxArrayLength = ReaderQuotas.MaxArrayLength;
                objBinding.ReaderQuotas.MaxBytesPerRead = ReaderQuotas.MaxBytesPerRead;
                objBinding.ReaderQuotas.MaxNameTableCharCount = ReaderQuotas.MaxNameTableCharCount;
                objBinding.Security.Mode = SecurityMode;
                objBinding.ReliableSession.Enabled = ReliableSession.Enabled;
                objBinding.ReliableSession.Ordered = ReliableSession.Ordered;
                objBinding.PortSharingEnabled = true;

                return objBinding;
            }
        }

        #endregion

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("CloseTimeout", CloseTimeout);
            objSerializedObject.Values.Add("OpenTimeout", OpenTimeout);
            objSerializedObject.Values.Add("ReceiveTimeout", ReceiveTimeout);
            objSerializedObject.Values.Add("SendTimeout", SendTimeout);
            objSerializedObject.Values.Add("HostNameComparisonMode", HostNameComparisonMode);
            objSerializedObject.Values.Add("MaxBufferPoolSize", MaxBufferPoolSize);
            objSerializedObject.Values.Add("MaxReceivedMessageSize", MaxReceivedMessageSize);
            objSerializedObject.Values.Add("PortSharingEnabled", PortSharingEnabled);
            objSerializedObject.Values.Add("TransactionFlow", TransactionFlow);
            objSerializedObject.Values.Add("MaxBufferSize", MaxBufferSize);
            objSerializedObject.Values.Add("MaxConnections", MaxConnections);
            objSerializedObject.Values.Add("ListenBacklog", ListenBacklog);
            objSerializedObject.Values.Add("TransferMode", TransferMode);
            objSerializedObject.Values.Add("MessageEncoding", MessageEncoding);
            objSerializedObject.Values.Add("SecurityMode", SecurityMode);
            objSerializedObject.Objects.Add("ReaderQuotas", ReaderQuotas);
            objSerializedObject.Objects.Add("ReliableSession", ReliableSession);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            CloseTimeout = objSerializedObject.Values.GetValue<TimeSpan>("CloseTimeout", DefaultCloseTimeout);
            OpenTimeout = objSerializedObject.Values.GetValue<TimeSpan>("OpenTimeout", DefaultOpenTimeout);
            ReceiveTimeout = objSerializedObject.Values.GetValue<TimeSpan>("ReceiveTimeout", DefaultReceiveTimeout);
            SendTimeout = objSerializedObject.Values.GetValue<TimeSpan>("SendTimeout", DefaultSendTimeout);
            HostNameComparisonMode = objSerializedObject.Values.GetValue<HostNameComparisonMode>("HostNameComparisonMode", DefaultHostNameComparisonMode);
            MaxBufferPoolSize = objSerializedObject.Values.GetValue<long>("MaxBufferPoolSize", DefaultMaxBufferPoolSize);
            MaxReceivedMessageSize = objSerializedObject.Values.GetValue<long>("MaxReceivedMessageSize", DefaultMaxReceivedMessageSize);
            PortSharingEnabled = objSerializedObject.Values.GetValue<bool>("PortSharingEnabled", DefaultPortSharingEnabled);
            TransactionFlow = objSerializedObject.Values.GetValue<bool>("TransactionFlow", DefaultTransactionFlow);            
            MaxBufferSize = objSerializedObject.Values.GetValue<int>("MaxBufferSize", DefaultMaxBufferSize);
            MaxConnections = objSerializedObject.Values.GetValue<int>("MaxConnections", DefaultMaxConnections);
            ListenBacklog = objSerializedObject.Values.GetValue<int>("ListenBacklog", DefaultListenBacklog);
            TransferMode = objSerializedObject.Values.GetValue<TransferMode>("TransferMode", DefaultTransferMode);
            SecurityMode = objSerializedObject.Values.GetValue<SecurityMode>("SecurityMode", DefaultSecurityMode);
            MessageEncoding = objSerializedObject.Values.GetValue<WSMessageEncoding>("MessageEncoding", DefaultMessageEncoding);
            ReaderQuotas = objSerializedObject.Objects.GetObject<WcfReaderQuotas>("ReaderQuotas", DefaultReaderQuotas);
            ReliableSession = objSerializedObject.Objects.GetObject<WcfOptionalReliableSession>("ReliableSession", DefaultReliableSession);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTimeSpan(CloseTimeout);
            objBinaryWriter.WriteTimeSpan(OpenTimeout);
            objBinaryWriter.WriteTimeSpan(ReceiveTimeout);
            objBinaryWriter.WriteTimeSpan(SendTimeout);
            objBinaryWriter.WriteEnum(HostNameComparisonMode);
            objBinaryWriter.Write(MaxBufferPoolSize);
            objBinaryWriter.Write(MaxReceivedMessageSize);
            objBinaryWriter.Write(PortSharingEnabled);
            objBinaryWriter.Write(TransactionFlow);            
            objBinaryWriter.Write(MaxBufferSize);
            objBinaryWriter.Write(MaxConnections);
            objBinaryWriter.Write(ListenBacklog);
            objBinaryWriter.WriteEnum(TransferMode);
            objBinaryWriter.WriteEnum(SecurityMode);
            objBinaryWriter.WriteEnum(MessageEncoding);
            objBinaryWriter.WriteTransportableObject(ReaderQuotas);
            objBinaryWriter.WriteTransportableObject(ReliableSession);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            CloseTimeout = objBinaryReader.ReadTimeSpan();
            OpenTimeout = objBinaryReader.ReadTimeSpan();
            ReceiveTimeout = objBinaryReader.ReadTimeSpan();
            SendTimeout = objBinaryReader.ReadTimeSpan();
            HostNameComparisonMode = objBinaryReader.ReadEnum<HostNameComparisonMode>();
            MaxBufferPoolSize = objBinaryReader.ReadInt64();
            MaxReceivedMessageSize = objBinaryReader.ReadInt64();
            PortSharingEnabled = objBinaryReader.ReadBoolean();
            TransactionFlow = objBinaryReader.ReadBoolean();
            MaxBufferSize = objBinaryReader.ReadInt32();
            MaxConnections = objBinaryReader.ReadInt32();
            ListenBacklog = objBinaryReader.ReadInt32();
            TransferMode = objBinaryReader.ReadEnum<TransferMode>();
            SecurityMode = objBinaryReader.ReadEnum<SecurityMode>();
            MessageEncoding = objBinaryReader.ReadEnum<WSMessageEncoding>();
            ReaderQuotas = objBinaryReader.ReadTransportableObject<WcfReaderQuotas>();
            ReliableSession = objBinaryReader.ReadTransportableObject<WcfOptionalReliableSession>();
        }

        #endregion

        #region Static Members

        public static readonly TimeSpan DefaultCloseTimeout = new TimeSpan(0, 1, 0);
        public static readonly TimeSpan DefaultOpenTimeout = new TimeSpan(0, 1, 0);
        public static readonly TimeSpan DefaultReceiveTimeout = new TimeSpan(0, 10, 0);
        public static readonly TimeSpan DefaultSendTimeout = new TimeSpan(0, 1, 0);
        public static readonly HostNameComparisonMode DefaultHostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
        public static readonly long DefaultMaxBufferPoolSize = 524288;
        public static readonly long DefaultMaxReceivedMessageSize = 65536;
        public static readonly int DefaultMaxBufferSize = 65536;
        public static readonly int DefaultMaxConnections = 10;
        public static readonly int DefaultListenBacklog = 10;
        public static readonly bool DefaultPortSharingEnabled = false;
        public static readonly bool DefaultTransactionFlow = false;
        public static readonly TransferMode DefaultTransferMode = TransferMode.Buffered;
        public static readonly WSMessageEncoding DefaultMessageEncoding = WSMessageEncoding.Text;
        public static readonly SecurityMode DefaultSecurityMode = SecurityMode.None;
        public static readonly WcfReaderQuotas DefaultReaderQuotas = new WcfReaderQuotas();
        public static readonly WcfOptionalReliableSession DefaultReliableSession = new WcfOptionalReliableSession();

        #endregion
    }
}
