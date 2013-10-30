namespace ReLi.Framework.Library.Threading
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class JobResult : ObjectBase
    {
        private Job _objJob;
        private JobResultType _enuResult;
        private DateTime _dtStartTime;
        private DateTime _dtEndTime;
        private TaskResultList _objTaskResults;

        public JobResult(Job objJob, JobResultType enuResult, DateTime dtStartTime, DateTime dtEndTime)
            : this(objJob, enuResult, dtStartTime, dtEndTime, new TaskResultList())
        {}

        public JobResult(Job objJob, JobResultType enuResult, DateTime dtStartTime, DateTime dtEndTime,
        TaskResultList objTaskResults)
        {
            Job = objJob;
            Result = enuResult;
            StartTime = dtStartTime;
            EndTime = dtEndTime;
            TaskResults = objTaskResults;
        }

        public JobResult(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public JobResult(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public Job Job
        {
            get
            {
                return _objJob;
            }
            private set
            {
                _objJob = value;
            }
        }

        public JobResultType Result
        {
            get
            {
                return _enuResult;
            }
            private set
            {
                _enuResult = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _dtStartTime;
            }
            private set
            {
                _dtStartTime = value;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _dtEndTime;
            }
            private set
            {
                _dtEndTime = value;
            }
        }

        public TaskResultList TaskResults
        {
            get
            {
                return _objTaskResults;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("TaskResults", "A valid non-null ITaskResultReadOnlyList instance is required");
                }

                _objTaskResults = value;
            }
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Objects.Add("Job", Job);
            objSerializedObject.Values.Add("StartTime", StartTime);
            objSerializedObject.Values.Add("EndTime", EndTime);
            objSerializedObject.Values.Add("Result", Result);
            objSerializedObject.Objects.Add("TaskResults", TaskResults);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Job = objSerializedObject.Objects.GetObject<Job>("Job", null);
            StartTime = objSerializedObject.Values.GetValue<DateTime>("StartTime", DateTime.MinValue);
            EndTime = objSerializedObject.Values.GetValue<DateTime>("EndTime", DateTime.MinValue);
            Result = objSerializedObject.Values.GetValue<JobResultType>("Result", JobResultType.Unknown);
            TaskResults = objSerializedObject.Objects.GetObject<TaskResultList>("TaskResults", null);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTransportableObject(Job);
            objBinaryWriter.WriteDateTime(StartTime);
            objBinaryWriter.WriteDateTime(EndTime);
            objBinaryWriter.Write((byte)Result);
            objBinaryWriter.WriteTransportableObject(TaskResults);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Job = objBinaryReader.ReadTransportableObject<Job>();
            StartTime = objBinaryReader.ReadDateTime();
            EndTime = objBinaryReader.ReadDateTime();
            Result = (JobResultType)objBinaryReader.ReadByte();
            TaskResults = objBinaryReader.ReadTransportableObject<TaskResultList>();
        }

        #endregion
    }
}
