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
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Threading;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public abstract class DownloadRequest : ObjectBase, ITask
    {
        public const int DefaultTimeOut = 300000; /// 5 minutes

        private int _intTimeOut;
        private string _strSource;
        private string _strDestination;
        private Credentials _objCredentials;

        public DownloadRequest(string strSource, string strDestination)
            : this(strSource, strDestination, new Credentials(CredentialCache.DefaultNetworkCredentials))
        {}

        public DownloadRequest(string strSource, string strDestination, Credentials objCredentials)
            : base()
        {
            this.Source = strSource;
            this.Destination = strDestination;
            this.Credentials = objCredentials;
            this.TimeOut = DefaultTimeOut;
        }

        public DownloadRequest(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public DownloadRequest(BinaryReaderExtension objBinaryReader)
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
            set
            {
                if (value == null)
                {
                    throw new ArgumentOutOfRangeException("Destination", "A valid non-null string is expected.");
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

        public override string ToString()
        {
            StringBuilder objString = new StringBuilder();
            objString.AppendLine("Source : " + Source);
            objString.AppendLine("Destination : " + Destination);

            return objString.ToString();
        }

        public abstract TaskResultType Execute(DownloadRequestSession objDownloadRequestSession);

        #region ITask Members

        public ITaskResult Execute(JobTicket objJobTicket)
        {
            TaskResultType enuTaskResult = TaskResultType.Completed;
            DownloadResult objDownloadResult = null;
            DownloadRequestSession objDownloadRequestSession = new DownloadRequestSession(this, objJobTicket);

            try
            {
                enuTaskResult = Execute(objDownloadRequestSession);
                objDownloadResult = new DownloadResult(this, enuTaskResult, objDownloadRequestSession.DownloadStats);
            }
            catch (Exception objException)
            {
                ErrorMessage objErrorMessage = new ErrorMessage(objException);
                ApplicationManager.Logs.WriteMessage(objErrorMessage);

                string strErrorMessage = objException.ToString();
                objDownloadResult = new DownloadResult(this, TaskResultType.Failed, objDownloadRequestSession.DownloadStats, strErrorMessage);
            }

            return objDownloadResult;
        }

        #endregion

        #region SerializableObject Members
               
        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Source", this.Source);
            objSerializedObject.Values.Add("Destination", this.Destination);
            objSerializedObject.Objects.Add("Credentials", Credentials);
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
        private static EmptyDownloadRequest _objEmpty;

        public static EmptyDownloadRequest Empty
        {
            get
            {
                if (_objEmpty == null)
                {
                    lock (_objLock)
                    {
                        if (_objEmpty == null)
                        {
                            _objEmpty = new EmptyDownloadRequest();
                        }
                    }
                }

                return _objEmpty;
            }
        }

        #endregion
    }
}
