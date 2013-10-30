namespace ReLi.Framework.Library.Threading
{
    #region Using Declarations

    using System;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Diagnostics;
    using ReLi.Framework.Library.Serialization;

    #endregion
    
	public class JobTicket
    {
        private DateTime _objStartTime;
        private Job.HasBeenCancelledDelegate _objHasBeenCancelledDelegate;
        private Job.TaskProgressChangedDelegate _objTaskProcessChangedDelegate;

        public JobTicket(DateTime objStartTime, Job.HasBeenCancelledDelegate objHasBeenCancelledDelegate, Job.TaskProgressChangedDelegate objTaskProcessChangedDelegate)
        {
            StartTime = objStartTime;
            HasBeenCancelledDelegate = objHasBeenCancelledDelegate;
            TaskProcessChangedDelegate = objTaskProcessChangedDelegate;
        }

        public DateTime StartTime
        {
            get
            {
                return _objStartTime;
            }
            private set
            {
                _objStartTime = value;
            }
        }

        public bool Cancelled
        {
            get
            {
                return HasBeenCancelledDelegate();
            }
        }

        public Job.HasBeenCancelledDelegate HasBeenCancelledDelegate
        {
            get
            {
                return _objHasBeenCancelledDelegate;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("HasBeenCancelledDelegate", "A valid non-null Job.HasBeenCancelledDelegate is required.");
                }

                _objHasBeenCancelledDelegate = value;
            }
        }

        public Job.TaskProgressChangedDelegate TaskProcessChangedDelegate
        {
            get
            {
                return _objTaskProcessChangedDelegate;
            }
            private set
            {
                _objTaskProcessChangedDelegate = value;
            }
        }
    }
}
