namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Serialization;
    
    #endregion
        
	public class LogManager : ObjectBase, IDisposable
    {
        private bool _blnIsDisposed;
        private object _objSyncObject;
        private LogBaseCollection _objLogs;
        private FileLog _objInternalFileLog;
        private ListBase<string> _objActiveMessageTypes;
                
        public LogManager()
        {
            Logs = new LogBaseCollection();
            InternalFileLog = CreateInternalFileLog();
        }
        
        public LogManager(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public LogManager(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}
        
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
            set
            {
                _objSyncObject = value;
            }
        }

        private LogBaseCollection Logs
        {
            get
            {
                return _objLogs;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Logs", "A valid LogBaseCollection is required.");
                }

                _objLogs = value;
            }
        }

        private FileLog InternalFileLog
        {
            get
            {
                return _objInternalFileLog;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("InternalFileLog", "A valid FileLog is required.");
                }

                _objInternalFileLog = value;
            }
        }

        private ListBase<string> ActiveMessageTypes
        {
            get
            {
                if (_objActiveMessageTypes == null)
                {
                    _objActiveMessageTypes = LoadActiveMessageTypes();
                }
                return _objActiveMessageTypes;
            }
            set
            {
                _objActiveMessageTypes = value;
            }
        }

        public void RegisterLog(LogBase objLog)
        {
            lock (SyncObject)
            {
                Logs.Add(objLog);
                objLog.MessageTypeAdded += new MessageTypeAddedEventHandler(Logs_MessageTypeAdded);
                objLog.MessageTypeRemoved += new MessageTypeRemovedEventHandler(Logs_MessageTypeRemoved);
                objLog.WriteMessageFailed += new WriteMessageFailedHandler(Logs_WriteMessageFailed);
                objLog.MessageTypeChanged += new MessageTypePropertyChangedEventHandler(Logs_MessageTypeChanged);
            }

            OnLogSettingsChanged();
        }

        public void UnRegisterLog(LogBase objLog)
        {
            lock (SyncObject)
            {
                Logs.Remove(objLog);
                objLog.MessageTypeAdded -= new MessageTypeAddedEventHandler(Logs_MessageTypeAdded);
                objLog.MessageTypeRemoved -= new MessageTypeRemovedEventHandler(Logs_MessageTypeRemoved);
                objLog.WriteMessageFailed -= new WriteMessageFailedHandler(Logs_WriteMessageFailed);
                objLog.MessageTypeChanged -= new MessageTypePropertyChangedEventHandler(Logs_MessageTypeChanged);
            }

            OnLogSettingsChanged();
        }

        public void UnRegisterLogs()
        {
            lock (SyncObject)
            {
                while (Logs.Count > 0)
                {
                    LogBase objLog = Logs[0];

                    UnRegisterLog(objLog);
                    objLog.MessageTypeAdded -= new MessageTypeAddedEventHandler(Logs_MessageTypeAdded);
                    objLog.MessageTypeRemoved -= new MessageTypeRemovedEventHandler(Logs_MessageTypeRemoved);
                    objLog.WriteMessageFailed -= new WriteMessageFailedHandler(Logs_WriteMessageFailed);
                    objLog.MessageTypeChanged -= new MessageTypePropertyChangedEventHandler(Logs_MessageTypeChanged);
                }
            }

            OnLogSettingsChanged();
        }

        public void RegisterLogs()
        {
            lock (SyncObject)
            {
                foreach(LogBase objLog in Logs)
                {
                    objLog.MessageTypeAdded += new MessageTypeAddedEventHandler(Logs_MessageTypeAdded);
                    objLog.MessageTypeRemoved += new MessageTypeRemovedEventHandler(Logs_MessageTypeRemoved);
                    objLog.WriteMessageFailed += new WriteMessageFailedHandler(Logs_WriteMessageFailed);
                    objLog.MessageTypeChanged += new MessageTypePropertyChangedEventHandler(Logs_MessageTypeChanged);
                }
            }

            OnLogSettingsChanged();
        }
 
        public bool IsMessageTypeAllowed<TMessageType>()
            where TMessageType : MessageBase
        {
            string strFullName = typeof(TMessageType).FullName;
            bool blnResult = ActiveMessageTypes.Contains(strFullName);

            return blnResult;
        }

        public bool IsMessageTypeAllowed(MessageBase objMessage)
        {
            string strFullName = objMessage.GetType().FullName;

            bool blnResult = ActiveMessageTypes.Contains(strFullName);
            return blnResult;
        }

        public void WriteMessage(MessageBase objMessage)
        {
            if (objMessage == null)
            {
                throw new ArgumentNullException("objMessage", "A valid non-null MessageBase is required.");
            }

            bool blnIsMessageTypeAllowed = IsMessageTypeAllowed(objMessage);
            if (blnIsMessageTypeAllowed == true)
            {
                WriteMessageToLogsDelegate objWriteMessageToLogs = new WriteMessageToLogsDelegate(WriteMessageToLogs);
                AsyncHelper.FireAndForget(objWriteMessageToLogs, objMessage);
            }
        }
             
        private void WriteMessageToLogs(MessageBase objMessage)
        {
            lock(SyncObject)
            {
                foreach (LogBase objLog in Logs)
                {
                    if ((objLog != null) && (objLog.Enabled == true))
                    {
                        try
                        {
                            //WriteMessageToLogDelegate objWriteMessageToLog = new WriteMessageToLogDelegate(WriteMessageToLog);
                            objLog.WriteMessage(objMessage);
                        }
                        catch (Exception objException)
                        {
                            Logs_WriteMessageFailed(objLog, new WriteMessageFailedEventArgs(objMessage, objException));
                        }
                    }
                }
            }
        }

        private void WriteMessageToLog(MessageBase objMessage, LogBase objLog)
        {
            try
            {
                if ( (objLog != null) && (objLog.Enabled == true))
                {
                    objLog.WriteMessage(objMessage);
                }
            }
            catch (Exception objException)
            {
                Logs_WriteMessageFailed(objLog, new WriteMessageFailedEventArgs(objMessage, objException));
            }
        }

        private void Logs_MessageTypeRemoved(object objSender, MessageTypeRemovedEventArgs objArguments)
        {
            OnLogSettingsChanged();
        }

        private void Logs_MessageTypeAdded(object objSender, MessageTypeAddedEventArgs objArguments)
        {
            OnLogSettingsChanged();
        }

        private void Logs_MessageTypeChanged(object objSender, MessageTypePropertyChangedEventArgs objArguments)
        {
            OnLogSettingsChanged();
        }

        private void Logs_WriteMessageFailed(object objSender, WriteMessageFailedEventArgs objArguments)
        {
            try
            {
                ErrorMessage objErrorMessage = new ErrorMessage(objArguments.Exception);
                if (objArguments.Message != null)
                {
                    objErrorMessage.Details = "Original Message: " + objArguments.Message.Content;
                }

                _objInternalFileLog.WriteMessage(objErrorMessage);
            }
            catch
            { }
        }

        private ListBase<string> LoadActiveMessageTypes()
        {
            ListBase<string> objActiveMessageTypes = new ListBase<string>();
            foreach (LogBase objLog in Logs)
            {
                foreach (MessageType objMessageType in objLog.MessageTypes)
                {
                    if (objMessageType.Enabled == true)
                    {
                        bool blnExists = objActiveMessageTypes.Contains(objMessageType.FullName);
                        if (blnExists == false)
                        {
                            objActiveMessageTypes.Add(objMessageType.FullName);
                        }
                    }
                }
            }

            return objActiveMessageTypes;
        }

        private FileLog CreateInternalFileLog()
        {
            /// Create an EventLog instance that will record any exceptions that are 
            /// encountered during the logging operations.
            /// 
            FileLog objFileLog = new FileLog();
            objFileLog.MessageTypes.Add(new MessageType(typeof(ErrorMessage)));
            objFileLog.FileName = "Error_[DATE][TIME].txt";

            return objFileLog;
        }

        #region Event Declarations

        public event LogSettingsChangedHandler LogSettingsChanged;
        protected void OnLogSettingsChanged()
        {
            ActiveMessageTypes = null;

            if (base.Initializing == false)
            {
                LogSettingsChangedHandler objHandler = LogSettingsChanged;
                if (objHandler != null)
                {
                    objHandler();
                }
            }
        }
        
        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            if (_blnIsDisposed == false)
            {
                if (InternalFileLog != null)
                {
                    InternalFileLog.Dispose();
                    InternalFileLog = null;
                }

                GC.SuppressFinalize(this);

                _blnIsDisposed = true;
            }
        }

        #endregion

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            SerializedObjectCollection objObjects = objSerializedObject.Objects;

            objObjects.Add("InternalFileLog", InternalFileLog);
            objObjects.Add("Logs", Logs);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            SerializedObjectCollection objObjects = objSerializedObject.Objects;

            FileLog objInternalFileLog = objObjects.GetObject<FileLog>("InternalFileLog", null);
            if (objInternalFileLog == null)
            {
                objInternalFileLog = CreateInternalFileLog();
            }
            InternalFileLog = objInternalFileLog;

            LogBaseCollection objLogs = objObjects.GetObject<LogBaseCollection>("Logs", null);
            if (objLogs == null)
            {
                objLogs = new LogBaseCollection();
            }
            Logs = objLogs;

            SyncObject = null;
            ActiveMessageTypes = null;

            RegisterLogs();
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTransportableObject(InternalFileLog);
            objBinaryWriter.WriteTransportableObject(Logs);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            InternalFileLog = objBinaryReader.ReadTransportableObject<FileLog>();
            Logs = objBinaryReader.ReadTransportableObject<LogBaseCollection>();

            SyncObject = null;
            ActiveMessageTypes = null;

            RegisterLogs();
        }

        #endregion

    }
}
