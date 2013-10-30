namespace ReLi.Framework.Library.Net
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Diagnostics;
    using ReLi.Framework.Library.Threading;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Security;

    #endregion
    
	public abstract class UploadRequest : ObjectBase, ITask
    {
        public const int DefaultTimeOut = 300000; /// 5 minutes

        private int _intTimeOut;
        private string _strSource;
        private string _strDestination;
        private Credentials _objCredentials;

        public UploadRequest(string strSource, string strDestination)
            : this(strSource, strDestination, new Credentials(CredentialCache.DefaultNetworkCredentials))
        { }

        public UploadRequest(string strSource, string strDestination, Credentials objCredentials)
            : base()
        {
            this.Source = strSource;
            this.Destination = strDestination;
            this.Credentials = objCredentials;
            this.TimeOut = DefaultTimeOut;
        }

        public UploadRequest(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public UploadRequest(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string Source
        {
            get
            {
                return _strSource;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentOutOfRangeException("Source", "A valid non-null string is expected.");
                }
                _strSource = value;
            }
        }

        public string SourceFileName
        {
            get
            {
                string strFileName = Path.GetFileName(_strSource);
                return strFileName;
            }
        }

        public string SourceDirectoryName
        {
            get
            {
                string strDirectoryName = Path.GetDirectoryName(_strSource);
                return strDirectoryName;
            }
        }

        public string Destination
        {
            get
            {
                return _strDestination;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentOutOfRangeException("Source", "A valid non-null string is expected.");
                }
                _strDestination = value;
            }
        }

        public string DestinationFileName
        {
            get
            {
                string strFileName = Path.GetFileName(_strDestination);
                return strFileName;
            }
        }

        public string DestinationDirectoryName
        {
            get
            {
                string strDirectoryName = Path.GetDirectoryName(_strDestination);
                return strDirectoryName;
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
                _objCredentials = value;
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

        public abstract TaskResultType Execute(UploadRequestSession objUploadRequestSession);

        #region ITask Members

        public ITaskResult Execute(JobTicket objJobTicket)
        {
            TaskResultType enuTaskResult = TaskResultType.Completed;
            UploadResult objUploadResult = null;
            UploadRequestSession objUploadRequestSession = new UploadRequestSession(this, objJobTicket);

            try
            {
                enuTaskResult = Execute(objUploadRequestSession);
                objUploadResult = new UploadResult(this, enuTaskResult, objUploadRequestSession.UploadStats);
            }
            catch (Exception objException)
            {
                ErrorMessage objErrorMessage = new ErrorMessage(objException);
                ApplicationManager.Logs.WriteMessage(objErrorMessage);

                string strErrorMessage = objException.ToString();
                objUploadResult = new UploadResult(this, TaskResultType.Failed, objUploadRequestSession.UploadStats, strErrorMessage);
            }

            return objUploadResult;
        }

        #endregion

        #region SerializableObject Members
              
        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Source", this.Source);
            objSerializedObject.Values.Add("Destination", this.Destination);
            objSerializedObject.Objects.Add("Credentials", new SerializedWrapperObject(this.Credentials));
            objSerializedObject.Values.Add("TimeOut", this.TimeOut);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            this.Source = objSerializedObject.Values.GetValue<string>("Source", string.Empty);
            this.Destination = objSerializedObject.Values.GetValue<string>("Destination", string.Empty);
            this.Credentials = objSerializedObject.Objects.GetObject<Credentials>("Credentials", null);
            this.TimeOut = objSerializedObject.Values.GetValue<int>("TimeOut", DefaultTimeOut);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Source);
            objBinaryWriter.Write(Destination);
            objBinaryWriter.WriteTransportableObject(Credentials);
            objBinaryWriter.Write(TimeOut);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Source = objBinaryReader.ReadString();
            Destination = objBinaryReader.ReadString();
            Credentials = objBinaryReader.ReadTransportableObject<Credentials>();
            TimeOut = objBinaryReader.ReadInt32();
        }

        #endregion

        #region Static Members

        private static object _objLock = new object();
        private static EmptyUploadRequest _objEmpty;

        public static EmptyUploadRequest Empty
        {
            get
            {
                if (_objEmpty == null)
                {
                    lock (_objLock)
                    {
                        if (_objEmpty == null)
                        {
                            _objEmpty = new EmptyUploadRequest();
                        }
                    }
                }

                return _objEmpty;
            }
        }

        #endregion
    }
}
