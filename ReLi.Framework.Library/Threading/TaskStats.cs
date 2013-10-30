namespace ReLi.Framework.Library.Threading
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class TaskStats : ObjectBase, ITaskStats
    {
        private ITask _objTask;
        private DateTime _dtStartTime;
        private DateTime _dtEndTime;
        private TimeSpan _objDuration;

        public TaskStats(ITask objTask, DateTime dtStartTime, DateTime dtEndTime)
            : base()
        {
            Task = objTask;
            StartTime = dtStartTime;
            EndTime = dtEndTime;
            Duration = EndTime - StartTime;
        }

        public TaskStats(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public TaskStats(BinaryReaderExtension objBinaryReader)
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
                    throw new ArgumentNullException("Task", "A valid non-null ITask instance is required.");
                }

                _objTask = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _dtStartTime;
            }
            protected set
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
            protected set
            {
                _dtEndTime = value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return _objDuration;
            }
            protected set
            {
                _objDuration = value;
            }
        }

        public virtual string FormattedDuration
        {
            get
            {
                string strFormattedDuration = string.Empty;
                TimeSpan objDuration = Duration;

                if (objDuration == TimeSpan.MinValue)
                {
                    strFormattedDuration = "00:00:00";
                }
                else if (objDuration.Days > 0)
                {
                    string strDurationFormat = "{0} day(s) {1:00}:{2:00}:{3:00}";
                    strFormattedDuration = String.Format(strDurationFormat, objDuration.Days, objDuration.Hours, objDuration.Minutes, objDuration.Seconds);
                }
                else
                {
                    string strDurationFormat = "{0:00}:{1:00}:{2:00}";
                    strFormattedDuration = String.Format(strDurationFormat, objDuration.Hours, objDuration.Minutes, objDuration.Seconds);
                }

                return strFormattedDuration;
            }
        }

        #region SerializableObject Members
        
        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Objects.Add("Task", Task);
            objSerializedObject.Values.Add("StartTime", StartTime);
            objSerializedObject.Values.Add("EndTime", EndTime);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Task = objSerializedObject.Objects.GetObject<ITask>("Task", null);
            StartTime = objSerializedObject.Values.GetValue<DateTime>("StartTime", DateTime.MinValue);
            EndTime = objSerializedObject.Values.GetValue<DateTime>("EndTime", DateTime.MaxValue);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTransportableObject(Task);
            objBinaryWriter.WriteDateTime(StartTime);
            objBinaryWriter.WriteDateTime(EndTime);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Task = objBinaryReader.ReadTransportableObject<ITask>();
            StartTime = objBinaryReader.ReadDateTime();
            EndTime = objBinaryReader.ReadDateTime();
        }

        #endregion
    }
}
