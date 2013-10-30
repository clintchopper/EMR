namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Diagnostics;
    using System.Security.Principal;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class EventLog : LogBase
    {
        #region Constant Declarations

        public const string DefaultLog = "ReLi";
        public const string DefaultSource = "EMR";

        #endregion

        private string _strLog;
        private string _strSource;
        private object _objSyncObject;

        public EventLog()
            : this(DefaultLog, DefaultSource, new MessageTypeCollection())
        {}

        public EventLog(string strLog, string strSource, MessageTypeCollection objMessageTypes)
            : base(objMessageTypes)
        {
            Log = strLog;
            Source = strSource;            
        }

        public EventLog(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public EventLog(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}
 
        public string Source
        {
            get
            {
                return _strSource;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Source", "A valid non-null string is required.");
                }

                _strSource = value;
            }
        }

        public string Log
        {
            get
            {
                return _strLog;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Log", "A valid non-null string is required.");
                }

                _strLog = value;
            }
        }

        private object SyncObject
        {
            get
            {
                if (_objSyncObject == null)
                {
                    _objSyncObject = new object();
                }

                return _objSyncObject;
            }
        }
        
        protected override void OnWriteMessage(MessageBase objMessage)
        {
            string strMessage = objMessage.ToString();
            EventLogEntryType enuEntryType = ((objMessage is ErrorMessage) ? EventLogEntryType.Error : EventLogEntryType.Information);

            lock (SyncObject)
            {
                if (System.Diagnostics.EventLog.SourceExists(Source) == false)
                {                     
                    System.Diagnostics.EventLog.CreateEventSource(Source, Log);
                }
            }
                        
            System.Diagnostics.EventLog.WriteEntry(Source, strMessage, enuEntryType);
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Source", Source);
            objSerializedObject.Values.Add("Log", Log);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            _objSyncObject = null;
            Log = objSerializedObject.Values.GetValue<string>("Log", DefaultLog);
            Source = objSerializedObject.Values.GetValue<string>("Source", DefaultSource);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Source);
            objBinaryWriter.Write(Log);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Source = objBinaryReader.ReadString();
            Log = objBinaryReader.ReadString();
        }

        #endregion

    }
}
