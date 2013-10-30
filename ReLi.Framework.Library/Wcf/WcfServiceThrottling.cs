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

    public class WcfServiceThrottling : ObjectBase
    {
        private int _intMaxConcurrentCalls;
        private int _intMaxConcurrentInstances;
        private int _intMaxConcurrentSessions;

        public WcfServiceThrottling()
            : this(DefaultMaxConcurrentCalls, DefaultMaxConcurrentInstances, DefaultMaxConcurrentSessions)
        { }

        public WcfServiceThrottling(int intMaxConcurrentCalls, int intMaxConcurrentInstances, int intMaxConcurrentSessions)
            : base()
        {
            MaxConcurrentCalls = intMaxConcurrentCalls;
            MaxConcurrentInstances = intMaxConcurrentInstances;
            MaxConcurrentSessions = intMaxConcurrentSessions;
        }

        public WcfServiceThrottling(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public WcfServiceThrottling(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        public int MaxConcurrentCalls
        {
            get
            {
                return _intMaxConcurrentCalls;
            }
            set
            {
                _intMaxConcurrentCalls = value;
            }
        }

        public int MaxConcurrentInstances
        {
            get
            {
                return _intMaxConcurrentInstances;
            }
            set
            {
                _intMaxConcurrentInstances = value;
            }
        }

        public int MaxConcurrentSessions
        {
            get
            {
                return _intMaxConcurrentSessions;
            }
            set
            {
                _intMaxConcurrentSessions = value;
            }
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("MaxConcurrentCalls", MaxConcurrentCalls);
            objSerializedObject.Values.Add("MaxConcurrentInstances", MaxConcurrentInstances);
            objSerializedObject.Values.Add("MaxConcurrentSessions", MaxConcurrentSessions);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            MaxConcurrentCalls = objSerializedObject.Values.GetValue<int>("MaxConcurrentCalls", DefaultMaxConcurrentCalls);
            MaxConcurrentInstances = objSerializedObject.Values.GetValue<int>("MaxConcurrentInstances", DefaultMaxConcurrentInstances);
            MaxConcurrentSessions = objSerializedObject.Values.GetValue<int>("MaxConcurrentSessions", DefaultMaxConcurrentSessions);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(MaxConcurrentCalls);
            objBinaryWriter.Write(MaxConcurrentInstances);
            objBinaryWriter.Write(MaxConcurrentSessions);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            MaxConcurrentCalls = objBinaryReader.ReadInt32();
            MaxConcurrentInstances = objBinaryReader.ReadInt32();
            MaxConcurrentSessions = objBinaryReader.ReadInt32();
        }

        #endregion

        #region Static Members

        public static readonly int DefaultMaxConcurrentCalls = 50;
        public static readonly int DefaultMaxConcurrentInstances = 100;
        public static readonly int DefaultMaxConcurrentSessions = 500;

        #endregion
    }
}
