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

    public class UploadResult : ObjectBase, ITaskResult
    {
        private UploadRequest _objUploadRequest;
        private TaskResultType _enuResultType;
        private IUploadStats _objUploadStats;
        private string _strDetails;

        public UploadResult(UploadRequest objUploadRequest, TaskResultType enuResultType, IUploadStats objUploadStats)
            : this(objUploadRequest, enuResultType, objUploadStats, string.Empty)
        { }

        public UploadResult(UploadRequest objUploadRequest, TaskResultType enuResultType, IUploadStats objUploadStats, string strDetails)
            : base()
        {
            if (objUploadRequest == null)
            {
                throw new ArgumentNullException("objUploadRequest", "A valid non-null UploadRequest is expected");
            }
            if (objUploadStats == null)
            {
                throw new ArgumentNullException("objUploadStats", "A valid non-null IUploadStats is expected");
            }
            if (strDetails == null)
            {
                throw new ArgumentNullException("strDetails", "A valid non-null string is expected");
            }

            _objUploadRequest = objUploadRequest;
            _objUploadStats = objUploadStats;
            _enuResultType = enuResultType;
            _strDetails = strDetails;
        }

        public UploadResult(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public UploadResult(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        public UploadRequest UploadRequest
        {
            get
            {
                return _objUploadRequest;
            }
            private set
            {
                _objUploadRequest = value;
            }
        }

        public IUploadStats UploadStats
        {
            get
            {
                return _objUploadStats;
            }
            private set
            {
                _objUploadStats = value;
            }
        }

        #region ITaskResult Members

        public ITask Task
        {
            get
            {
                return _objUploadRequest;
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
                return _objUploadStats;
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

            objString.AppendLine("Source: " + _objUploadRequest.Source);
            objString.AppendLine("Target: " + _objUploadRequest.Destination);
            objString.AppendLine("Result: " + _enuResultType.ToString());
            objString.AppendLine("Details: " + _strDetails);

            return objString.ToString();
        }

        #endregion

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Objects.Add("UploadRequest", _objUploadRequest);
            objSerializedObject.Values.Add("ResultType", _enuResultType);
            objSerializedObject.Objects.Add("UploadStats", _objUploadStats);
            objSerializedObject.Values.Add("Details", _strDetails);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            _enuResultType = objSerializedObject.Values.GetValue<TaskResultType>("ResultType", TaskResultType.Unknown);
            _strDetails = objSerializedObject.Values.GetValue<string>("Details", string.Empty);

            _objUploadRequest = objSerializedObject.Objects.GetObject<UploadRequest>("UploadRequest", null);
            if (_objUploadRequest == null)
            {
                _objUploadRequest = UploadRequest.Empty;
            }

            _objUploadStats = objSerializedObject.Objects.GetObject<IUploadStats>("UploadStats", null);
            if (_objUploadStats == null)
            {
                _objUploadStats = ReLi.Framework.Library.Net.UploadStats.Empty;
            }
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTransportableObject(UploadRequest);
            objBinaryWriter.Write((byte)Result);
            objBinaryWriter.WriteTransportableObject(UploadStats);
            objBinaryWriter.Write(Details);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            UploadRequest = objBinaryReader.ReadTransportableObject<UploadRequest>();
            Result = (TaskResultType)objBinaryReader.ReadByte();
            UploadStats = objBinaryReader.ReadTransportableObject<UploadStats>();
            Details = objBinaryReader.ReadString();
        }

        #endregion

    }
}
