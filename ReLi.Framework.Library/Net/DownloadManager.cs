namespace ReLi.Framework.Library.Net
{
    #region Using Declarations

    using System;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Threading;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class DownloadManager
    {
        private JobResult _objJobResult = null;

        public DownloadManager()
        {}

        public DownloadResultList DownloadWithoutDialog(DownloadRequest objDownloadRequest)
        {
            if (objDownloadRequest == null)
            {
                throw new ArgumentNullException("objDownloadRequest", "A valid non-null DownloadRequest is expected");
            }

            DownloadRequestList objDownloadRequests = new DownloadRequestList();
            objDownloadRequests.Add(objDownloadRequest);

            return DownloadWithoutDialog(objDownloadRequests);
        }

        public DownloadResultList DownloadWithoutDialog(DownloadRequestList objDownloadRequests)
        {
            if (objDownloadRequests == null)
            {
                throw new ArgumentNullException("objDownloadRequests", "A valid non-null DownloadRequestList is expected");
            }

            _objJobResult = null;
            IEnumerable<ITask> objTasks = objDownloadRequests.GetTasks();
                        
            Job objDownloadJob = new Job(objTasks);
            objDownloadJob.JobEnd += new Job.OnJobEndDelegate(objDownloadJob_JobEnd);
            objDownloadJob.Start();
            objDownloadJob.Wait();

            DownloadResultList objDownloadResults = new DownloadResultList();
            if (_objJobResult != null)
            {
                foreach (ITaskResult objTaskResult in _objJobResult.TaskResults)
                {
                    DownloadResult objDownloadResult = (DownloadResult)objTaskResult;
                    objDownloadResults.Add(objDownloadResult);
                }
            }

            return objDownloadResults;
        }

        public DownloadResultList DownloadWithDialog(DownloadRequest objDownloadRequest)
        {           
            return DownloadWithDialog(objDownloadRequest, new DownloadDialogSettings());
        }

        public DownloadResultList DownloadWithDialog(DownloadRequest objDownloadRequest, DownloadDialogSettings objDownloadDialogSettings)
        {
            if (objDownloadRequest == null)
            {
                throw new ArgumentNullException("objDownloadRequest", "A valid non-null DownloadRequest is expected");
            }
            if (objDownloadDialogSettings == null)
            {
                throw new ArgumentNullException("objDownloadDialogSettings", "A valid non-null DownloadDialogSettings is expected");
            }

            DownloadRequestList objDownloadRequests = new DownloadRequestList();
            objDownloadRequests.Add(objDownloadRequest);

            return DownloadWithDialog(objDownloadRequests, objDownloadDialogSettings);
        }

        public DownloadResultList DownloadWithDialog(DownloadRequestList objDownloadRequests)
        {
            return DownloadWithDialog(objDownloadRequests, new DownloadDialogSettings());
        }

        public DownloadResultList DownloadWithDialog(DownloadRequestList objDownloadRequests, DownloadDialogSettings objDownloadDialogSettings)
        {
            if (objDownloadRequests == null)
            {
                throw new ArgumentNullException("objDownloadRequests", "A valid non-null DownloadRequestList is expected");
            }
            if (objDownloadDialogSettings == null)
            {
                throw new ArgumentNullException("objDownloadDialogSettings", "A valid non-null DownloadDialogSettings is expected");
            }

            DownloadResultList objDownloadResults = DownloadDialog.ShowDialog(objDownloadRequests, objDownloadDialogSettings);
            return objDownloadResults;
        }

        private void objDownloadJob_JobEnd(JobResult objJobResult)
        {
            _objJobResult = objJobResult;
        }
    }
}
