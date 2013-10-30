namespace ReLi.Framework.Library.Threading
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class TaskResult : ObjectBase, ITaskResult
    {
        private ITask _objTask;
        private ITaskStats _objStats;
        private TaskResultType _enuResult;
        private string _strDetails;
        
        public TaskResult(ITask objTask, ITaskStats objStats, TaskResultType enuResult)
            : this(objTask, objStats, enuResult, string.Empty)
        {}

        public TaskResult(ITask objTask, ITaskStats objStats, TaskResultType enuResult, string strDetails)
            : base()
        {
            Task = objTask;
            Stats = objStats;
            Result = enuResult;
            Details = strDetails;
        }

        public TaskResult(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public TaskResult(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public ITask Task
        {
            get
            {
                return _objTask;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Task", "A valid non-null ITask is required.");
                }

                _objTask = value;
            }
        }

        public TaskResultType Result
        {
            get
            {
                return _enuResult;
            }
            protected set
            {
                _enuResult = value;
            }
        }

        public ITaskStats Stats
        {
            get
            {
                return _objStats;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Stats", "A valid non-null ITaskStats is required.");
                }

                _objStats = value;
            }
        }

        public string Details
        {
            get
            {
                return _strDetails;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Details", "A valid non-null string instance is required.");
                }

                _strDetails = value;
            }
        }

        public override string ToString()
        {
            StringBuilder objString = new StringBuilder();

            objString.AppendLine("Result: " + Result.ToString());
            objString.AppendLine("Details: " + _strDetails);

            return objString.ToString();
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Objects.Add("Task", Task);
            objSerializedObject.Objects.Add("Stats", Stats);
            objSerializedObject.Values.Add("Result", Result);
            objSerializedObject.Values.Add("Details", Details);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Task = objSerializedObject.Objects.GetObject<ITask>("Task", null);
            Stats = objSerializedObject.Objects.GetObject<ITaskStats>("Stats", null);
            Result = objSerializedObject.Values.GetValue<TaskResultType>("Result", TaskResultType.Unknown);
            Details = objSerializedObject.Values.GetValue<string>("Details", string.Empty);
        }

        #endregion        

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTransportableObject(Task);
            objBinaryWriter.WriteTransportableObject(Stats);
            objBinaryWriter.Write((byte)Result);
            objBinaryWriter.Write(Details);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Task = objBinaryReader.ReadTransportableObject<ITask>();
            Stats = objBinaryReader.ReadTransportableObject<ITaskStats>();
            Result = (TaskResultType)objBinaryReader.ReadByte();
            Details = objBinaryReader.ReadString();
        }

        #endregion
    }
}
