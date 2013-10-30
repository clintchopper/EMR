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
    using ReLi.Framework.Library.Threading;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class DownloadResult : ObjectBase, ITaskResult
    {
        private DownloadRequest _objDownloadRequest;
        private TaskResultType _enuResultType;
        private IDownloadStats _objDownloadStats;
        private string _strDetails;

        public DownloadResult(DownloadRequest objDownloadRequest, TaskResultType enuResultType, IDownloadStats objDownloadStats)
            : this(objDownloadRequest, enuResultType, objDownloadStats, string.Empty)
        { }

        public DownloadResult(DownloadRequest objDownloadRequest, TaskResultType enuResultType, IDownloadStats objDownloadStats, string strDetails)
            : base()
        {
            if (objDownloadRequest == null)
            {
                throw new ArgumentNullException("objDownloadRequest", "A valid non-null DownloadRequest is expected");
            }
            if (objDownloadStats == null)
            {
                throw new ArgumentNullException("objDownloadStats", "A valid non-null IDownloadStats is expected");
            }
            if (strDetails == null)
            {
                throw new ArgumentNullException("strDetails", "A valid non-null string is expected");
            }

            _objDownloadRequest = objDownloadRequest;
            _objDownloadStats = objDownloadStats;
            _enuResultType = enuResultType;
            _strDetails = strDetails;
        }

        public DownloadResult(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public DownloadResult(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public DownloadRequest DownloadRequest
        {
            get
            {
                return _objDownloadRequest;
            }
            private set
            {
                _objDownloadRequest = value;
            }
        }

        public IDownloadStats DownloadStats
        {
            get
            {
                return _objDownloadStats;
            }
            private set
            {
                _objDownloadStats = value;
            }
        }

        #region ITaskResult Members

        public ITask Task
        {
            get
            {
                return _objDownloadRequest;
            }
        }

        public TaskResultType Result
        {
            get
            {
                return _enuResultType;
            }
            private set
            {
                _enuResultType = value;
            }
        }

        public ITaskStats Stats
        {
            get
            {
                return _objDownloadStats;
            }
        }

        public string Details
        {
            get
            {
                return _strDetails;
            }
            private set
            {
                _strDetails = value;
            }
        }

        public override string ToString()
        {
            StringBuilder objString = new StringBuilder();

            objString.AppendLine("Source: " + _objDownloadRequest.Source);
            objString.AppendLine("Target: " + _objDownloadRequest.Destination);
            objString.AppendLine("Result: " + _enuResultType.ToString());
            objString.AppendLine("Details: " + _strDetails);

            return objString.ToString();
        }

        #endregion

        #region SerializableObject Members
          
        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Objects.Add("DownloadRequest", _objDownloadRequest);
            objSerializedObject.Values.Add("ResultType", _enuResultType);
            objSerializedObject.Objects.Add("DownloadStats", _objDownloadStats);
            objSerializedObject.Values.Add("Details", _strDetails);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            _enuResultType = objSerializedObject.Values.GetValue<TaskResultType>("ResultType", TaskResultType.Unknown);
            _strDetails = objSerializedObject.Values.GetValue<string>("Details", string.Empty);

            _objDownloadRequest = objSerializedObject.Objects.GetObject<DownloadRequest>("DownloadRequest", null);
            if (_objDownloadRequest == null)
            {
                _objDownloadRequest = DownloadRequest.Empty;
            }

            _objDownloadStats = objSerializedObject.Objects.GetObject<IDownloadStats>("DownloadStats", null);
            if (_objDownloadStats == null)
            {
                _objDownloadStats = ReLi.Framework.Library.Net.DownloadStats.Empty;
            }
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTransportableObject(DownloadRequest);
            objBinaryWriter.Write((byte)Result);
            objBinaryWriter.WriteTransportableObject(DownloadStats);
            objBinaryWriter.Write(Details);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            DownloadRequest = objBinaryReader.ReadTransportableObject<DownloadRequest>();
            Result = (TaskResultType)objBinaryReader.ReadByte();
            DownloadStats = objBinaryReader.ReadTransportableObject<DownloadStats>();
            Details = objBinaryReader.ReadString();
        }

        #endregion

    }
}
