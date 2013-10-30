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

    public class WcfServiceBehavior : ObjectBase
    {
        private string _strPerformanceCounters;
        private ConcurrencyMode _enuConcurrencyMode;
        private InstanceContextMode _enuInstanceContextMode;

        public WcfServiceBehavior()
            : this(DefaultPerformanceCounters, DefaultConcurrencyMode, DefaultInstanceContextMode)
        { }

        public WcfServiceBehavior(string strPerformanceCounters, ConcurrencyMode enuConcurrencyMode, InstanceContextMode enuInstanceContextMode)
            : base()
        {
            PerformanceCounters = strPerformanceCounters;
            ConcurrencyMode = enuConcurrencyMode;
            InstanceContextMode = enuInstanceContextMode;
        }

        public WcfServiceBehavior(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public WcfServiceBehavior(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        public string PerformanceCounters
        {
            get
            {
                return _strPerformanceCounters;
            }
            set
            {
                _strPerformanceCounters = value;
            }
        }

        public ConcurrencyMode ConcurrencyMode
        {
            get
            {
                return _enuConcurrencyMode;
            }
            set
            {
                _enuConcurrencyMode = value;
            }
        }

        public InstanceContextMode InstanceContextMode
        {
            get
            {
                return _enuInstanceContextMode;
            }
            set
            {
                _enuInstanceContextMode = value;
            }
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("PerformanceCounters", PerformanceCounters);
            objSerializedObject.Values.Add("ConcurrencyMode", ConcurrencyMode);
            objSerializedObject.Values.Add("InstanceContextMode", InstanceContextMode);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            PerformanceCounters = objSerializedObject.Values.GetValue<string>("PerformanceCounters", DefaultPerformanceCounters);
            ConcurrencyMode = objSerializedObject.Values.GetValue<ConcurrencyMode>("ConcurrencyMode", DefaultConcurrencyMode);
            InstanceContextMode = objSerializedObject.Values.GetValue<InstanceContextMode>("InstanceContextMode", DefaultInstanceContextMode);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(PerformanceCounters);
            objBinaryWriter.WriteEnum(ConcurrencyMode);
            objBinaryWriter.WriteEnum(InstanceContextMode);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            PerformanceCounters = objBinaryReader.ReadString();
            ConcurrencyMode = objBinaryReader.ReadEnum<ConcurrencyMode>();
            InstanceContextMode = objBinaryReader.ReadEnum<InstanceContextMode>();
        }

        #endregion

        #region Static Members

        public static readonly string DefaultPerformanceCounters = "Off";
        public static readonly ConcurrencyMode DefaultConcurrencyMode = ConcurrencyMode.Multiple;
        public static readonly InstanceContextMode DefaultInstanceContextMode = InstanceContextMode.Single;

        #endregion
    }
}
