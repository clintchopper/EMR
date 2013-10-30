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

    public class DownloadRequestSession
    {
        private JobTicket _objJobTicket;
        private DownloadRequest _objDownloadRequest;
        private DownloadStats _objDownloadStats;

        public DownloadRequestSession(DownloadRequest objDownloadRequest, JobTicket objJobTicket)
            : base()
        {
            DownloadRequest = objDownloadRequest;
            JobTicket = objJobTicket;
            DownloadStats = new DownloadStats(objDownloadRequest, objJobTicket.StartTime);
        }

        public DownloadRequest DownloadRequest
        {
            get
            {
                return _objDownloadRequest;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("DownloadRequest", "A valid non-null DownloadRequest is required.");
                }

                _objDownloadRequest = value;
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

        public DownloadStats DownloadStats
        {
            get
            {
                return _objDownloadStats;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("DownloadStats", "A valid non-null DownloadStats is required.");
                }

                _objDownloadStats = value;
            }
        }

        public void UpdateProgess()
        {
            if (JobTicket.TaskProcessChangedDelegate != null)
            {
                JobTicket.TaskProcessChangedDelegate(DownloadStats);
            }
        }
    }
}
