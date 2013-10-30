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
        
	public class UploadManager
    {
        private JobResult _objJobResult = null;

        public UploadManager()
        { }

        public UploadResultList UploadWithoutDialog(UploadRequest objUploadRequest)
        {
            if (objUploadRequest == null)
            {
                throw new ArgumentNullException("objUploadRequest", "A valid non-null UploadRequest is expected");
            }

            UploadRequestList objUploadRequests = new UploadRequestList();
            objUploadRequests.Add(objUploadRequest);

            return UploadWithoutDialog(objUploadRequests);
        }

        public UploadResultList UploadWithoutDialog(UploadRequestList objUploadRequests)
        {
            if (objUploadRequests == null)
            {
                throw new ArgumentNullException("objUploadRequests", "A valid non-null UploadRequestList is expected");
            }

            _objJobResult = null;
            IEnumerable<ITask> objTasks = objUploadRequests.GetTasks();

            Job objUploadJob = new Job(objTasks);
            objUploadJob.JobEnd += new Job.OnJobEndDelegate(objUploadJob_JobEnd);
            objUploadJob.Start();
            objUploadJob.Wait();

            UploadResultList objUploadResults = new UploadResultList();
            if (_objJobResult != null)
            {
                foreach (ITaskResult objTaskResult in _objJobResult.TaskResults)
                {
                    UploadResult objUploadResult = (UploadResult)objTaskResult;
                    objUploadResults.Add(objUploadResult);
                }
            }

            return objUploadResults;
        }

        public UploadResultList UploadWithDialog(UploadRequest objUploadRequest)
        {
            return UploadWithDialog(objUploadRequest, new UploadDialogSettings());
        }

        public UploadResultList UploadWithDialog(UploadRequest objUploadRequest, UploadDialogSettings objUploadDialogSettings)
        {
            if (objUploadRequest == null)
            {
                throw new ArgumentNullException("objUploadRequest", "A valid non-null UploadRequest is expected");
            }
            if (objUploadDialogSettings == null)
            {
                throw new ArgumentNullException("objUploadDialogSettings", "A valid non-null UploadDialogSettings is expected");
            }

            UploadRequestList objUploadRequests = new UploadRequestList();
            objUploadRequests.Add(objUploadRequest);

            return UploadWithDialog(objUploadRequests, objUploadDialogSettings);
        }

        public UploadResultList UploadWithDialog(UploadRequestList objUploadRequests)
        {
            return UploadWithDialog(objUploadRequests, new UploadDialogSettings());
        }

        public UploadResultList UploadWithDialog(UploadRequestList objUploadRequests, UploadDialogSettings objUploadDialogSettings)
        {
            if (objUploadRequests == null)
            {
                throw new ArgumentNullException("objUploadRequests", "A valid non-null UploadRequestList is expected");
            }

            UploadResultList objUploadResults = UploadDialog.ShowDialog(objUploadRequests, objUploadDialogSettings);
            return objUploadResults;
        }

        private void objUploadJob_JobEnd(JobResult objJobResult)
        {
            _objJobResult = objJobResult;
        }
    }
}
