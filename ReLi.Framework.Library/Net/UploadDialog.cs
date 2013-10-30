namespace ReLi.Framework.Library.Net
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using System.Threading;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Threading;

    #endregion

    public partial class UploadDialog : Form
    {
        private int _intTaskIndex = 0;
        private int _intTaskCount = 0;
        private bool _blnExpanded = false;
        private bool _blnUpdatePercentages = false;
        private bool _blnPauseContinueButtonRunning = false;
        private Job _objJob = null;
        private JobResult _objJobResult = null;
        private UploadRequestList _objUploadRequests = null;
        private UploadResultList _objUploadResults = null;
        private UploadStats _objCurrentUploadStats = null;
        private UploadDialogSettings _objUploadDialogSettings;
        private Thread _objUploadThread = null;
        
        private UploadDialog()
        {
            InitializeComponent();

            ReLi.Framework.Library.WinAPI.User32.RemoveCloseButton(this.Handle);
            this.Icon = Properties.Resources.resIconNetwork;

            Expand(true);
        }

        protected virtual void ThreadSafe(MethodInvoker method)
        {
            if (base.IsDisposed == false)
            {
                try
                {
                    if (InvokeRequired)
                    {
                        Invoke(method);
                    }
                    else
                    {
                        method();
                    }
                }
                catch (Exception objException)
                {
                    Console.WriteLine(objException.ToString());
                }
            }
        }

        protected UploadResultList ShowDialogWithResult(UploadRequestList objUploadRequests, UploadDialogSettings objUploadDialogSettingss)
        {
            _objUploadRequests = objUploadRequests;
            _objUploadDialogSettings = objUploadDialogSettingss;

            ThreadStart objThreadStart = new ThreadStart(UploadThread);
            _objUploadThread = new Thread(objThreadStart);
            _objUploadThread.IsBackground = true;
            _objUploadThread.Start();

            this.ShowDialog();

            return _objUploadResults;
        }

        private void UploadThread()
        {
            Thread.Sleep(10);

            this.ThreadSafe(new MethodInvoker(delegate()
            {
                listViewStatus.Items.Clear();
                foreach (UploadRequest objUploadRequest in _objUploadRequests)
                {
                    string strFileName = Path.GetFileName(objUploadRequest.Source);
                    ListViewItem objListViewItem = new ListViewItem(objUploadRequest.Source);
                    objListViewItem.SubItems.Add("Queued");
                    listViewStatus.Items.Add(objListViewItem);
                }

                buttonCancel.Enabled = _objUploadDialogSettings.AllowCancel;
                buttonPauseContinue.Enabled = _objUploadDialogSettings.AllowPause;
                this.Refresh();
            }));

            _intTaskIndex = 0;
            _intTaskCount = _objUploadRequests.Count;
            _objJobResult = null;

            IEnumerable<ITask> objTasks = _objUploadRequests.GetTasks();
            _objJob = new Job(objTasks);
            _objJob.TaskBegin += new Job.OnTaskBeginDelegate(objUploadJob_TaskBegin);
            _objJob.TaskEnd += new Job.OnTaskEndDelegate(objUploadJob_TaskEnd);
            _objJob.JobEnd += new Job.OnJobEndDelegate(objUploadJob_JobEnd);
            _objJob.TaskProgressChanged += new Job.TaskProgressChangedDelegate(objUploadJob_TaskProgressChanged);
            _objJob.Start();
            _objJob.Wait();

            _objUploadResults = new UploadResultList();
            if (_objJobResult != null)
            {
                _objUploadResults = new UploadResultList(_objJobResult.TaskResults);
            }

            Thread.Sleep(10);

            this.ThreadSafe(new MethodInvoker(delegate()
            {
                this.Close();
            }));
        }

        private void UpdateOverallProgress(int intMaximum, int intValue)
        {
            if (_blnExpanded == true)
            {
                progressBarOverall.Maximum = intMaximum;
                progressBarOverall.Value = intValue;
                progressBarOverall.Refresh();
            }
        }

        private void UpdateCurrentProgress(int intMaximum, int intValue)
        {
            if (_blnExpanded == false)
            {
                progressBarOverall.Maximum = intMaximum;
                progressBarOverall.Value = intValue;
            }
            else
            {
                progressBarOverall.Maximum = _intTaskCount;
                progressBarOverall.Value = _intTaskIndex - 1;
            }
            progressBarOverall.Refresh();

            progressBarCurrent.Maximum = intMaximum;
            progressBarCurrent.Value = intValue;
            progressBarCurrent.Refresh();
        }

        private void Expand(bool blnExpand)
        {
            groupBoxDetails.Visible = blnExpand;
            listViewStatus.Visible = blnExpand;

            int intCollapsedHeight = 165;
            int intExpandedHeight = 480;

            this.Height = ((blnExpand == true) ? intExpandedHeight : intCollapsedHeight);

            Screen objActiveScreen = Screen.FromHandle(this.Handle);
            this.Top = Convert.ToInt32(((objActiveScreen.Bounds.Height - this.Height) / 2));

            _blnExpanded = blnExpand;
        }

        private TaskActionType objUploadJob_TaskEnd(ITaskResult objTaskResult, int intIndex)
        {
            timerUpdateProgress.Enabled = false;

            TaskActionType enuTaskActionType = TaskActionType.Continue;
            if (objTaskResult.Result == TaskResultType.Failed)
            {
                enuTaskActionType = UploadFailedMessage.ShowMessage(objTaskResult);
            }

            this.ThreadSafe(new MethodInvoker(delegate()
            {
                timerUpdateProgress.Enabled = false;

                UpdateOverallProgress(_intTaskCount, _intTaskIndex);
                ListViewItem objListViewItem = listViewStatus.Items[_intTaskIndex - 1];
                if (objListViewItem != null)
                {
                    objListViewItem.SubItems[1].Text = objTaskResult.Result.ToString();
                    listViewStatus.EnsureVisible(objListViewItem.Index);
                }

                UpdateCurrentProgress(progressBarCurrent.Maximum, progressBarCurrent.Maximum);
            }));

            _objCurrentUploadStats = null;
            return enuTaskActionType;
        }

        private void objUploadJob_TaskBegin(ITask objTask, int intTaskIndex, int intTaskTotal)
        {
            _intTaskIndex = intTaskIndex + 1;

            UploadRequest objUploadRequest = (UploadRequest)objTask;

            this.ThreadSafe(new MethodInvoker(delegate()
            {
                labelOverall.Text = String.Format(labelOverall.Tag.ToString(), objUploadRequest.SourceFileName, _intTaskIndex.ToString(), _intTaskCount.ToString());

                ListViewItem objListViewItem = listViewStatus.Items[_intTaskIndex - 1];
                if (objListViewItem != null)
                {
                    _blnUpdatePercentages = true;
                    objListViewItem.SubItems[1].Text = "Uploading";
                    listViewStatus.EnsureVisible(objListViewItem.Index);
                }

                UpdateCurrentProgress(0, 0);

                pathLabelSource.Text = objUploadRequest.Source;
                pathLabelTarget.Text = objUploadRequest.Destination;

                timerUpdateProgress.Enabled = true;
            }));

        }

        private void objUploadJob_TaskProgressChanged(ITaskStats objTaskStats)
        {
            IUploadStats objStats = (IUploadStats)objTaskStats;
            _objCurrentUploadStats = (UploadStats)objStats;
        }

        private void objUploadJob_JobEnd(JobResult objJobResult)
        {
            this.ThreadSafe(new MethodInvoker(delegate()
            {
                UpdateOverallProgress(progressBarOverall.Maximum, progressBarOverall.Maximum);
                Thread.Sleep(10);
            }));
            
            _objJobResult = objJobResult;
        }
        
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            buttonCancel.Enabled = false;
            buttonCancel.Refresh();

            timerUpdateProgress.Enabled = false;
            _objJob.Pause();

            DialogResult enuDialogResult = MessageBox.Show("Are you sure you want to cancel the upload?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (enuDialogResult == DialogResult.Yes)
            {
                buttonCancel.Enabled = false;
                buttonPauseContinue.Enabled = false;

                ListViewItem objListViewItem = listViewStatus.Items[_intTaskIndex - 1];
                if (objListViewItem != null)
                {
                    _blnUpdatePercentages = false;
                    objListViewItem.SubItems[1].Text = "Cancelling";
                    listViewStatus.EnsureVisible(objListViewItem.Index);
                }
                _objJob.Stop();
            }
            else
            {
                _objJob.Continue();
                timerUpdateProgress.Enabled = true;
            }

            buttonCancel.Enabled = _objUploadDialogSettings.AllowCancel;                
        }

        private void buttonPauseContinue_Click(object sender, EventArgs e)
        {
            if (_blnPauseContinueButtonRunning == false)
            {
                _blnPauseContinueButtonRunning = true;

                buttonPauseContinue.Enabled = false;
                buttonPauseContinue.Refresh();

                ListViewItem objListViewItem = listViewStatus.Items[_intTaskIndex - 1];

                switch (buttonPauseContinue.Text.ToLower())
                {
                    case ("pause"):

                        timerUpdateProgress.Enabled = false;

                        if (objListViewItem != null)
                        {
                            _blnUpdatePercentages = false;
                            objListViewItem.SubItems[1].Text = "Paused";
                            listViewStatus.EnsureVisible(objListViewItem.Index);
                        }

                        _objJob.Pause();

                        buttonPauseContinue.Text = "Continue";
                        buttonCancel.Enabled = false;
                        break;

                    case ("continue"):

                        if (objListViewItem != null)
                        {
                            _blnUpdatePercentages = true;
                            objListViewItem.SubItems[1].Text = "Downloading";
                            listViewStatus.EnsureVisible(objListViewItem.Index);
                        }

                        _objJob.Continue();

                        timerUpdateProgress.Enabled = true;

                        buttonPauseContinue.Text = "Pause";
                        buttonCancel.Enabled = _objUploadDialogSettings.AllowCancel;
                        break;
                }

                _blnPauseContinueButtonRunning = false;
                buttonPauseContinue.Enabled = true;
            }
        }
        
        private void buttonDetails_Click(object sender, EventArgs e)
        {
            _blnExpanded = !_blnExpanded;
            Expand(_blnExpanded);
        }

        private void timerUpdateProgress_Tick(object sender, EventArgs e)
        {
            if (_objCurrentUploadStats != null)
            {
                this.ThreadSafe(new MethodInvoker(delegate()
                {
                    textSizeInBytes.Text = String.Format("{0:n0} bytes", _objCurrentUploadStats.GetFormattedSize(TransferSizeType.Byte));
                    textBytesSent.Text = String.Format("{0:n0} bytes", _objCurrentUploadStats.GetFormattedBytesSent(TransferSizeType.Byte));
                    textBytesRemaining.Text = String.Format("{0:n0} bytes", _objCurrentUploadStats.GetFormattedBytesRemaining(TransferSizeType.Byte));

                    textDuration.Text = _objCurrentUploadStats.FormattedDuration;

                    textSpeed.Text = _objCurrentUploadStats.GetFormattedTransferRate(TransferSizeType.Kilobyte, TransferTimeType.Second);
                    textTimeRemaining.Text = _objCurrentUploadStats.TimeRemaining;

                    progressBarCurrent.Maximum = Convert.ToInt32(_objCurrentUploadStats.Size);
                    progressBarCurrent.Value = Convert.ToInt32(_objCurrentUploadStats.BytesSent);

                    if (_blnUpdatePercentages == true)
                    {
                        double dblPercentage = (Convert.ToDouble((double)progressBarCurrent.Value / (double)progressBarCurrent.Maximum) * 100);
                        int intPercentage = Convert.ToInt32(dblPercentage);

                        ListViewItem objListViewItem = listViewStatus.Items[_intTaskIndex - 1];
                        if (objListViewItem != null)
                        {
                            objListViewItem.SubItems[1].Text = intPercentage.ToString() + "%";
                        }
                    }
                }));
            }
        }

        #region Static Members

        public static UploadResultList ShowDialog(UploadRequestList objUploadRequests, UploadDialogSettings objUploadDialogSettingss)
        {
            if (objUploadRequests == null)
            {
                throw new ArgumentNullException("objUploadRequests", "A valid non-null UploadRequestList is expected");
            }
            if (objUploadDialogSettingss == null)
            {
                throw new ArgumentNullException("objUploadDialogSettingss", "A valid non-null UploadDialogSettings is expected");
            }

            UploadResultList objUploadResults = null;
            using (UploadDialog objFileUploadDialog = new UploadDialog())
            {
                objUploadResults = objFileUploadDialog.ShowDialogWithResult(objUploadRequests, objUploadDialogSettingss);
            }

            return objUploadResults;
        }

        #endregion        
     }
}
