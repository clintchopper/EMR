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
    using ReLi.Framework.Library.Net;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Serialization;
    using System.ServiceModel.Channels;

    #endregion

    public class WcfBasicHttpBinding : ObjectBase, IWcfBinding
    {
        private TimeSpan _objCloseTimeout;
        private TimeSpan _objOpenTimeout;
        private TimeSpan _objReceiveTimeout;
        private TimeSpan _objSendTimeOut;
        private HostNameComparisonMode _enuHostNameComparisonMode;
        private long _lngMaxBufferPoolSize;
        private long _lngMaxReceivedMessageSize;
        private int _intMaxBufferSize;
        private WSMessageEncoding _enuMessageEncoding;
        private WcfReaderQuotas _objReaderQuotas;
        private BasicHttpSecurityMode _enuSecurityMode;
        private TransferMode _enuTransferMode;

        public WcfBasicHttpBinding()
            : this(DefaultCloseTimeout, DefaultOpenTimeout, DefaultReceiveTimeout, DefaultSendTimeout, DefaultHostNameComparisonMode,
            DefaultMaxBufferSize, DefaultMaxBufferPoolSize, DefaultMaxReceivedMessageSize, DefaultMessageEncoding, new WcfReaderQuotas(), DefaultSecurityMode, DefaultTransferMode)
        {}

        public WcfBasicHttpBinding(TimeSpan objCloseTimeout, TimeSpan objOpenTimeout, TimeSpan objReceiveTimeout,
            TimeSpan objSendTimeOut, HostNameComparisonMode enuHostNameComparisonMode, int intMaxBufferSize, long lngMaxBufferPoolSize, long lngMaxReceivedMessageSize,
            WSMessageEncoding enuMessageEncoding, WcfReaderQuotas objReaderQuotas, BasicHttpSecurityMode enuSecurityMode, TransferMode enuTransferMode)
            : base()
        {
            CloseTimeout = objCloseTimeout;
            OpenTimeout = objOpenTimeout;
            ReceiveTimeout = objReceiveTimeout;
            SendTimeout = objSendTimeOut;
            HostNameComparisonMode = enuHostNameComparisonMode;
            MaxBufferSize = intMaxBufferSize;
            MaxBufferPoolSize = lngMaxBufferPoolSize;
            MaxReceivedMessageSize = lngMaxReceivedMessageSize;
            MessageEncoding = enuMessageEncoding;
            ReaderQuotas = objReaderQuotas;
            SecurityMode = enuSecurityMode;
            TransferMode = enuTransferMode;
        }

        public WcfBasicHttpBinding(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public WcfBasicHttpBinding(BinaryReaderExtension objBinaryReader)
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

        public BasicHttpSecurityMode SecurityMode
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

        #region IWcfBinding Members

        public Binding Binding
        {
            get
            {
                /// Make sure the proxy information has been set for the application.
                /// This should have been done at startup, but just in case...
                /// 
                ProxySettings.Initialize();

                BasicHttpBinding objBinding = new BasicHttpBinding(SecurityMode);
                objBinding.CloseTimeout = CloseTimeout;
                objBinding.OpenTimeout = OpenTimeout;
                objBinding.ReceiveTimeout = ReceiveTimeout;
                objBinding.SendTimeout = SendTimeout;
                objBinding.HostNameComparisonMode = HostNameComparisonMode;
                objBinding.MaxBufferSize = MaxBufferSize;
                objBinding.MaxBufferPoolSize = MaxBufferPoolSize;
                objBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
                objBinding.MessageEncoding = MessageEncoding;
                objBinding.TransferMode = TransferMode;
                objBinding.TextEncoding = Encoding.UTF8;
                objBinding.ReaderQuotas.MaxDepth = ReaderQuotas.MaxDepth;
                objBinding.ReaderQuotas.MaxStringContentLength = ReaderQuotas.MaxStringContentLength;
                objBinding.ReaderQuotas.MaxArrayLength = ReaderQuotas.MaxArrayLength;
                objBinding.ReaderQuotas.MaxBytesPerRead = ReaderQuotas.MaxBytesPerRead;
                objBinding.ReaderQuotas.MaxNameTableCharCount = ReaderQuotas.MaxNameTableCharCount;
                objBinding.UseDefaultWebProxy = false;

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
            objSerializedObject.Values.Add("MaxBufferSize", MaxBufferSize);
            objSerializedObject.Values.Add("MaxBufferPoolSize", MaxBufferPoolSize);
            objSerializedObject.Values.Add("MaxReceivedMessageSize", MaxReceivedMessageSize);
            objSerializedObject.Values.Add("MessageEncoding", MessageEncoding);
            objSerializedObject.Values.Add("SecurityMode", SecurityMode);
            objSerializedObject.Values.Add("TransferMode", TransferMode);
            objSerializedObject.Objects.Add("ReaderQuotas", ReaderQuotas);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            CloseTimeout = objSerializedObject.Values.GetValue<TimeSpan>("CloseTimeout", DefaultCloseTimeout);
            OpenTimeout = objSerializedObject.Values.GetValue<TimeSpan>("OpenTimeout", DefaultOpenTimeout);
            ReceiveTimeout = objSerializedObject.Values.GetValue<TimeSpan>("ReceiveTimeout", DefaultReceiveTimeout);
            SendTimeout = objSerializedObject.Values.GetValue<TimeSpan>("SendTimeout", DefaultSendTimeout);
            HostNameComparisonMode = objSerializedObject.Values.GetValue<HostNameComparisonMode>("HostNameComparisonMode", DefaultHostNameComparisonMode);
            MaxBufferSize = objSerializedObject.Values.GetValue<int>("MaxBufferSize",DefaultMaxBufferSize);
            MaxBufferPoolSize = objSerializedObject.Values.GetValue<long>("MaxBufferPoolSize", DefaultMaxBufferPoolSize);
            MaxReceivedMessageSize = objSerializedObject.Values.GetValue<long>("MaxReceivedMessageSize", DefaultMaxReceivedMessageSize);
            MessageEncoding = objSerializedObject.Values.GetValue<WSMessageEncoding>("MessageEncoding", DefaultMessageEncoding);
            SecurityMode = objSerializedObject.Values.GetValue<BasicHttpSecurityMode>("SecurityMode", DefaultSecurityMode);
            TransferMode = objSerializedObject.Values.GetValue<TransferMode>("TransferMode", DefaultTransferMode);
            ReaderQuotas = objSerializedObject.Objects.GetObject<WcfReaderQuotas>("ReaderQuotas", null);
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
            objBinaryWriter.Write(MaxBufferSize);
            objBinaryWriter.Write(MaxBufferPoolSize);
            objBinaryWriter.Write(MaxReceivedMessageSize);
            objBinaryWriter.WriteEnum(MessageEncoding);
            objBinaryWriter.WriteEnum(SecurityMode);
            objBinaryWriter.WriteEnum(TransferMode);
            objBinaryWriter.WriteTransportableObject(ReaderQuotas);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            CloseTimeout = objBinaryReader.ReadTimeSpan();
            OpenTimeout = objBinaryReader.ReadTimeSpan();
            ReceiveTimeout = objBinaryReader.ReadTimeSpan();
            SendTimeout = objBinaryReader.ReadTimeSpan();
            HostNameComparisonMode = objBinaryReader.ReadEnum<HostNameComparisonMode>();
            MaxBufferSize = objBinaryReader.ReadInt32();
            MaxBufferPoolSize = objBinaryReader.ReadInt64();
            MaxReceivedMessageSize = objBinaryReader.ReadInt64();
            MessageEncoding = objBinaryReader.ReadEnum<WSMessageEncoding>();
            SecurityMode = objBinaryReader.ReadEnum<BasicHttpSecurityMode>();
            TransferMode = objBinaryReader.ReadEnum<TransferMode>();
            ReaderQuotas = objBinaryReader.ReadTransportableObject<WcfReaderQuotas>();
        }

        #endregion

        #region Static Members

        public static readonly TimeSpan DefaultCloseTimeout = new TimeSpan(0, 1, 0);
        public static readonly TimeSpan DefaultOpenTimeout = new TimeSpan(0, 1, 0);
        public static readonly TimeSpan DefaultReceiveTimeout = new TimeSpan(0, 10, 0);
        public static readonly TimeSpan DefaultSendTimeout = new TimeSpan(0, 1, 0);
        public static readonly HostNameComparisonMode DefaultHostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
        public static readonly long DefaultMaxBufferPoolSize = 524288;
        public static readonly int DefaultMaxBufferSize = 65536;
        public static readonly long DefaultMaxReceivedMessageSize = 65536;
        public static readonly WSMessageEncoding DefaultMessageEncoding = WSMessageEncoding.Text;
        public static readonly BasicHttpSecurityMode DefaultSecurityMode = BasicHttpSecurityMode.None;
        public static readonly TransferMode DefaultTransferMode = default(TransferMode);

        #endregion
    }
}
