namespace ReLi.Framework.Library.Net
{
    #region Using Declarations

    using System;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Diagnostics;
    using ReLi.Framework.Library.Threading;
    using ReLi.Framework.Library.Serialization;

    #endregion

    public class UploadRequestSession
    {
        private JobTicket _objJobTicket;
        private UploadRequest _objUploadRequest;
        private UploadStats _objUploadStats;

        public UploadRequestSession(UploadRequest objUploadRequest, JobTicket objJobTicket)
            : base()
        {
            UploadRequest = objUploadRequest;
            JobTicket = objJobTicket;
            UploadStats = new UploadStats(objUploadRequest, objJobTicket.StartTime);
        }

        public UploadRequest UploadRequest
        {
            get
            {
                return _objUploadRequest;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("UploadRequest", "A valid non-null UploadRequest is required.");
                }

                _objUploadRequest = value;
            }
        }

        public JobTicket JobTicket
        {
            get
            {
                return _objJobTicket;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("JobTicket", "A valid non-null JobTicket is required.");
                }

                _objJobTicket = value;
            }
        }

        public UploadStats UploadStats
        {
            get
            {
                return _objUploadStats;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("UploadStats", "A valid non-null UploadStats is required.");
                }

                _objUploadStats = value;
            }
        }

        public void UpdateProgess()
        {
            if (JobTicket.TaskProcessChangedDelegate != null)
            {
                JobTicket.TaskProcessChangedDelegate(UploadStats);
            }
        }
    }
}
