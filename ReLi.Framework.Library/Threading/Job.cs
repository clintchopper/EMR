namespace ReLi.Framework.Library.Threading
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Net;
    using System.Text;    
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Diagnostics;

    #endregion
        
	public class Job : ObjectBase 
    {
        public delegate void OnJobStatusChangedDelegate(Job objJob, JobStatusType enuJobStatus);
        public event OnJobStatusChangedDelegate JobStatusChanged;

        public delegate void OnJobBeginDelegate(Job objJob);
        public event OnJobBeginDelegate JobBegin;

        public delegate void OnJobEndDelegate(JobResult objJobResult);
        public event OnJobEndDelegate JobEnd;

        public delegate void OnTaskBeginDelegate(ITask objTask, int intIndex, int intTotal);
        public event OnTaskBeginDelegate TaskBegin;

        public delegate TaskActionType OnTaskEndDelegate(ITaskResult objTaskResult, int intIndex);
        public event OnTaskEndDelegate TaskEnd;

        public delegate void TaskProgressChangedDelegate(ITaskStats objTaskStats);
        public event TaskProgressChangedDelegate TaskProgressChanged;

        private string _strName;
        private ITaskList _objTasks;
        private JobStatusType _enuStatus;
        private Thread _objStartThread;
        private ManualResetEvent _objPauseEvent;                
        private Dictionary<JobStatusType, ManualResetEvent> _objWaitForStatusEvents;

        public Job(IEnumerable<ITask> objTasks)
            : this(string.Empty, objTasks)
        {}

        public Job(ITask objTask)
            : this(string.Empty, new ITask[] { objTask })
        {}

        public Job(ITask objTask, LogBase objLog)
            : this(string.Empty, new ITask[]{objTask})
        {}

        public Job(string strName, IEnumerable<ITask> objTasks)
            : base()
        {
            Name = strName;
            _objTasks = new ITaskList(objTasks);
        }

        public Job(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public Job(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string Name
        {
            get
            {
                return _strName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "A valid non-null string is expected.");
                }

                _strName = value;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            /// Create an ManualResetEvent for each of the job statuses that are currently 
            /// defined.  We will use these to signal threads whenever a particular status 
            /// has been set.
            /// 
            _objWaitForStatusEvents = new Dictionary<JobStatusType, ManualResetEvent>();
            foreach (JobStatusType enuJobStatus in Enum.GetValues(typeof(JobStatusType)))
            {
                _objWaitForStatusEvents.Add(enuJobStatus, new ManualResetEvent(false));
            }

            Status = JobStatusType.Queued;
            _objStartThread = null;
            _objPauseEvent = new ManualResetEvent(true);
        }

        private object _objStatusLock = new object();
        public JobStatusType Status
        {
            get
            {
                return _enuStatus;
            }
            protected set
            {
                lock (_objStatusLock)
                {
                    _enuStatus = value;                    
                    SignalStatusEvent();
                }

                OnJobStatusChanged();
            }
        }
                  
        public void Start()
        {
            switch (Status)
            {
                case (JobStatusType.Queued):
                    _objPauseEvent.Set();
                    _objStartThread = new Thread(new ThreadStart(StartThread));
                    _objStartThread.IsBackground = true;
                    _objStartThread.Start();
                    break;

                //    default:
                //        throw new Exception("The job's status must equal 'Queued' prior to calling 'Start'.");
            }
        }

        public void Stop()
        {
            switch (Status)
            {
                case (JobStatusType.Running):
                case (JobStatusType.Paused):
                    _objPauseEvent.Set();
                    Status = JobStatusType.Stopping;                    
                    break;

                case (JobStatusType.Stopping):
                case (JobStatusType.Completed):
                    break;

                //default:
                //    throw new Exception("The job's status must equal 'Running' or 'Paused' prior to calling 'Stop'.");

            }
        }

        public void Pause()
        {
            switch (Status)
            {
                case (JobStatusType.Running):
                case (JobStatusType.Pausing):
                    Status = JobStatusType.Pausing;
                    _objPauseEvent.Reset();
                    break;

                //case(JobStatusType.Paused):
                //    break;

                //default:
                //    throw new Exception("The job's status must equal 'Running' prior to calling 'Pause'.");

            }
        }

        public void Continue()
        {
            switch (Status)
            {
                case (JobStatusType.Paused):
                    Status = JobStatusType.Running;
                    _objPauseEvent.Set();
                    break;

                //default:
                //    throw new Exception("The job's status must equal 'Paused' prior to calling 'Continue'.");

            }
        }

        public void Reset()
        {
            if (Status != JobStatusType.Completed)
            {
                throw new Exception("The job's status must equal 'Completed' prior to calling 'Reset'.");
            }

            Status = JobStatusType.Queued;
        }

        protected void OnJobStatusChanged()
        {
            if (this.JobStatusChanged != null)
            {
                this.JobStatusChanged(this, Status);
            }
        }

        protected void OnJobBegin()
        {
            if (this.JobBegin != null)
            {
                this.JobBegin(this);
            }
        }

        protected void OnJobEnd(JobResult objJobResult)
        {
            if (this.JobEnd != null)
            {
                this.JobEnd(objJobResult);
            }
        }

        protected void OnTaskBegin(ITask objTask, int intIndex, int intTotal)
        {
            if (this.TaskBegin != null)
            {
                this.TaskBegin(objTask, intIndex, intTotal);
            }
        }

        protected TaskActionType OnTaskEnd(ITaskResult objTaskResult, int intIndex)
        {
            TaskActionType enuTaskActionType = TaskActionType.Continue;

            if (this.TaskEnd != null)
            {
                enuTaskActionType = this.TaskEnd(objTaskResult, intIndex);
            }

            return enuTaskActionType;
        }
      
        private void StartThread()
        {
            DateTime dtJobStartTime = DateTime.Now;
            TaskResultList objTaskResults = new TaskResultList();

            Status = JobStatusType.Running;
            OnJobBegin();

            JobResultType enuJobResult = JobResultType.Completed;

            int intTaskIndex = 0;
            int intTaskCount = _objTasks.Count;
            bool blnHasBeenCancelled = false;

            while (intTaskIndex < intTaskCount)
            {
                ITask objTask = _objTasks[intTaskIndex];

                blnHasBeenCancelled = HasBeenCancelled();
                if (blnHasBeenCancelled == true)
                {
                    enuJobResult = JobResultType.Cancelled;
                    break;
                }

                DateTime dtTaskStartTime = DateTime.Now;
                JobTicket objJobTicket = new JobTicket(dtTaskStartTime, HasBeenCancelled, TaskProgressChanged);
                ITaskResult objTaskResult = null;

                try
                {
                    OnTaskBegin(objTask, intTaskIndex, intTaskCount);
                    objTaskResult = objTask.Execute(objJobTicket);
                }
                catch (Exception objException)
                {
                    string strErrorMessage = objException.ToString();
                    TaskStats objTaskStats = new TaskStats(objTask, dtTaskStartTime, DateTime.Now);
                    objTaskResult = new TaskResult(objTask, objTaskStats, TaskResultType.Failed, strErrorMessage);
                }

                if (objTaskResult == null)
                {
                    string strErrorMessage = "A null value was returned by the task.";
                    TaskStats objTaskStats = new TaskStats(objTask, dtTaskStartTime, DateTime.Now);
                    objTaskResult = new TaskResult(objTask, objTaskStats, TaskResultType.Failed, strErrorMessage);
                }

                blnHasBeenCancelled = HasBeenCancelled();
                if (blnHasBeenCancelled == true)
                {
                    objTaskResults.Add(objTaskResult);
                    break;
                }
                else
                {
                    TaskActionType enuTaskActionType = OnTaskEnd(objTaskResult, intTaskIndex);
                    if (enuTaskActionType == TaskActionType.Retry)
                    {
                       continue;
                    }
                    else if (enuTaskActionType == TaskActionType.Continue)
                    {
                        objTaskResults.Add(objTaskResult);
                        intTaskIndex++;

                        if (objTaskResult.Result == TaskResultType.RebootRequired)
                        {
                            enuJobResult = JobResultType.RebootRequired;
                            break;
                        }

                        continue;
                    }
                    else if (enuTaskActionType == TaskActionType.Cancel)
                    {
                        objTaskResults.Add(objTaskResult);
                        enuJobResult = JobResultType.Cancelled;
                        break;
                    }
                }
            }

            if (blnHasBeenCancelled ==  true)
            {
                for (int intRemainingTaskIndex = intTaskIndex + 1; intRemainingTaskIndex < intTaskCount; intRemainingTaskIndex++)
                {
                    ITask objRemainingTask = _objTasks[intRemainingTaskIndex];
                    TaskStats objTaskStats = new TaskStats(objRemainingTask, dtJobStartTime, DateTime.Now);
                    ITaskResult objRemainingTaskResult = new TaskResult(objRemainingTask, objTaskStats, TaskResultType.Cancelled);

                    objTaskResults.Add(objRemainingTaskResult);
                }
            }

            Thread.Sleep(200);

            JobResult objJobResult = new JobResult(this, enuJobResult, dtJobStartTime, DateTime.Now, new TaskResultList(objTaskResults));
            OnJobEnd(objJobResult);

            Thread.Sleep(200);

            Status = JobStatusType.Completed;
        }

        public delegate bool HasBeenCancelledDelegate();
        protected bool HasBeenCancelled()
        {
            bool blnHasBeenCancelled = false;

            if (Status == JobStatusType.Pausing)
            {
                Status = JobStatusType.Paused;
                _objPauseEvent.WaitOne();
            }

            switch (Status)
            {
                case (JobStatusType.Running):
                    blnHasBeenCancelled = false;
                    break;

                case (JobStatusType.Stopping):
                    blnHasBeenCancelled = true;
                    break;
            }

            return blnHasBeenCancelled;
        }

        public void Wait()
        {
            WaitForStatus(JobStatusType.Completed);
        }

        public void WaitForStatus(JobStatusType enuJobStatus)
        {
            ManualResetEvent objManualResetEvent = _objWaitForStatusEvents[enuJobStatus];
            if (objManualResetEvent != null)
            {
                objManualResetEvent.WaitOne();
            }
        }

        private void SignalStatusEvent()
        {
            foreach (KeyValuePair<JobStatusType, ManualResetEvent> objWaitForStatusEvent in _objWaitForStatusEvents)
            {
                if (objWaitForStatusEvent.Key == Status)
                {
                    objWaitForStatusEvent.Value.Set();
                }
                else
                {
                    objWaitForStatusEvent.Value.Reset();
                }
            }
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Name", _strName);
            objSerializedObject.Values.Add("Status", _enuStatus);
            objSerializedObject.Objects.Add("Tasks", _objTasks);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Name = objSerializedObject.Values.GetValue<string>("Name", string.Empty);            
            
            _objTasks = objSerializedObject.Objects.GetObject <ITaskList>("Tasks", null);
            if (_objTasks == null)
            {
                _objTasks = new ITaskList();
            }

            Status = objSerializedObject.Values.GetValue<JobStatusType>("Status", JobStatusType.Queued);

            /// Create an ManualResetEvent for each of the job statuses that are currently 
            /// defined.  We will use these to signal threads whenever a particular status 
            /// has been set.
            /// 
            _objWaitForStatusEvents = new Dictionary<JobStatusType, ManualResetEvent>();
            foreach (JobStatusType enuJobStatus in Enum.GetValues(typeof(JobStatusType)))
            {
                _objWaitForStatusEvents.Add(enuJobStatus, new ManualResetEvent(false));
            }
                                    
            _objStartThread = null;
            _objPauseEvent = new ManualResetEvent(true);

        }
        
        #endregion  

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Name);
            objBinaryWriter.Write((byte)Status);
            objBinaryWriter.WriteTransportableObject(_objTasks);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            _strName = objBinaryReader.ReadString();
            _enuStatus = (JobStatusType)objBinaryReader.ReadByte();
            _objTasks = objBinaryReader.ReadTransportableObject<ITaskList>();
        }

        #endregion
    }
}
