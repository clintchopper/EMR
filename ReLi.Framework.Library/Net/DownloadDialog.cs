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

    public partial class DownloadDialog : Form
    {
        private int _intTaskIndex = 0;
        private int _intTaskCount = 0;
        private bool _blnExpanded = false;
        private bool _blnUpdatePercentages = false;
        private bool _blnPauseContinueButtonRunning = false;
        private Job _objJob = null;
        private JobResult _objJobResult = null;
        private DownloadRequestList _objDownloadRequests = null;
        private DownloadResultList _objDownloadResults = null;
        private DownloadStats _objCurrentDownloadStats = null;
        private DownloadDialogSettings _objDownloadDialogSettings;
        private Thread _objDownloadThread = null;

        private DownloadDialog()
        {
            InitializeComponent();

            ReLi.Framework.Library.WinAPI.User32.RemoveCloseButton(this.Handle);
            this.Icon = Properties.Resources.resIconNetwork;

            Expand(false);
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
                catch(Exception objException)
                {
                    Console.WriteLine(objException.ToString());
                }
            }
        }

        protected DownloadResultList ShowDialogWithResult(DownloadRequestList objDownloadRequests, DownloadDialogSettings objDownloadDialogSettings)
        {
            _objDownloadRequests = objDownloadRequests;
            _objDownloadDialogSettings = objDownloadDialogSettings;

            ThreadStart objThreadStart = new ThreadStart(DownloadThread);
            _objDownloadThread = new Thread(objThreadStart);
            _objDownloadThread.IsBackground = true;
            _objDownloadThread.Start();

            this.ShowDialog();

            return _objDownloadResults;
        }

        private void DownloadThread()
        {
            Thread.Sleep(10);

            this.ThreadSafe(new MethodInvoker(delegate()
            {
                listViewStatus.Items.Clear();
                foreach (DownloadRequest objDownloadRequest in _objDownloadRequests)
                {
                    string strFileName = Path.GetFileName(objDownloadRequest.Source);
                    ListViewItem objListViewItem = new ListViewItem(objDownloadRequest.Destination);
                    objListViewItem.SubItems.Add("Queued");
                    listViewStatus.Items.Add(objListViewItem);
                }

                buttonCancel.Enabled = _objDownloadDialogSettings.AllowCancel;
                buttonPauseContinue.Enabled = _objDownloadDialogSettings.AllowPause;
                this.Refresh();
            }));

            _intTaskIndex = 0;
            _intTaskCount = _objDownloadRequests.Count;
            _objJobResult = null;

            IEnumerable<ITask> objTasks = _objDownloadRequests.GetTasks();
            _objJob = new Job(objTasks);
            _objJob.TaskBegin += new Job.OnTaskBeginDelegate(objDownloadJob_TaskBegin);
            _objJob.TaskEnd += new Job.OnTaskEndDelegate(objDownloadJob_TaskEnd);
            _objJob.JobEnd += new Job.OnJobEndDelegate(objDownloadJob_JobEnd);
            _objJob.TaskProgressChanged += new Job.TaskProgressChangedDelegate(objDownloadJob_TaskProgressChanged);
            _objJob.Start();
            _objJob.Wait();

            _objDownloadResults = new DownloadResultList();
            if (_objJobResult != null)
            {
                _objDownloadResults = new DownloadResultList(_objJobResult.TaskResults);
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
        }

        private TaskActionType objDownloadJob_TaskEnd(ITaskResult objTaskResult, int intIndex)
        {
            timerUpdateProgress.Enabled = false;

            TaskActionType enuTaskActionType = TaskActionType.Continue;
            if (objTaskResult.Result == TaskResultType.Failed)
            {
                enuTaskActionType = DownloadFailedMessage.ShowMessage(objTaskResult);
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

            _objCurrentDownloadStats = null;

            return enuTaskActionType;
        }

        private void objDownloadJob_TaskBegin(ITask objTask, int intTaskIndex, int intTaskTotal)
        {
            _intTaskIndex = intTaskIndex + 1;

            DownloadRequest objDownloadRequest = (DownloadRequest)objTask;

            this.ThreadSafe(new MethodInvoker(delegate()
            {
                labelOverall.Text = String.Format(labelOverall.Tag.ToString(), objDownloadRequest.SourceFileName, _intTaskIndex.ToString(), _intTaskCount.ToString());

                ListViewItem objListViewItem = listViewStatus.Items[_intTaskIndex - 1];
                if (objListViewItem != null)
                {
                    _blnUpdatePercentages = true;
                    objListViewItem.SubItems[1].Text = "Downloading";
                    listViewStatus.EnsureVisible(objListViewItem.Index);
                }

                UpdateCurrentProgress(0, 0);

                pathLabelSource.Text = objDownloadRequest.Source;
                pathLabelTarget.Text = objDownloadRequest.Destination;

                timerUpdateProgress.Enabled = true;
            }));

        }

        private void objDownloadJob_TaskProgressChanged(ITaskStats objTaskStats)
        {
            IDownloadStats objStats = (IDownloadStats)objTaskStats;
            _objCurrentDownloadStats = (DownloadStats)objStats;
        }

        private void objDownloadJob_JobEnd(JobResult objJobResult)
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

            DialogResult enuDialogResult = MessageBox.Show("Are you sure you want to cancel the download?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (enuDialogResult == DialogResult.Yes)
            {
                buttonCancel.Enabled = false;
                buttonPauseContinue.Enabled = false;
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

            buttonCancel.Enabled = _objDownloadDialogSettings.AllowCancel;                
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
                        buttonCancel.Enabled = _objDownloadDialogSettings.AllowCancel;
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
            if (_objCurrentDownloadStats != null)
            {
                this.ThreadSafe(new MethodInvoker(delegate()
                {
                    textSizeInBytes.Text = String.Format("{0:n0} bytes", _objCurrentDownloadStats.GetFormattedSize(TransferSizeType.Byte));
                    textBytesReceived.Text = String.Format("{0:n0} bytes", _objCurrentDownloadStats.GetFormattedBytesReceived(TransferSizeType.Byte));
                    textBytesRemaining.Text = String.Format("{0:n0} bytes", _objCurrentDownloadStats.GetFormattedBytesRemaining(TransferSizeType.Byte));

                    textDuration.Text = _objCurrentDownloadStats.FormattedDuration;
                    textSpeed.Text = _objCurrentDownloadStats.GetFormattedTransferRate(TransferSizeType.Kilobyte, TransferTimeType.Second);
                    textTimeRemaining.Text = _objCurrentDownloadStats.TimeRemaining;

                    int intMaximum = Convert.ToInt32(_objCurrentDownloadStats.Size);
                    int intSize = Convert.ToInt32(_objCurrentDownloadStats.BytesReceived);
                    UpdateCurrentProgress(intMaximum, intSize);

                    if (_blnUpdatePercentages == true)
                    {
                        double dblPercentage = (Convert.ToDouble((double)intSize / (double)intMaximum) * 100);
                        if (double.IsNaN(dblPercentage) == false)
                        {
                            int intPercentage = Convert.ToInt32(dblPercentage);

                            ListViewItem objListViewItem = listViewStatus.Items[_intTaskIndex - 1];
                            if (objListViewItem != null)
                            {
                                objListViewItem.SubItems[1].Text = intPercentage.ToString() + "%";
                            }
                        }
                    }
                }));
            }
        }        

        #region Static Members

        public static DownloadResultList ShowDialog(DownloadRequestList objDownloadRequests, DownloadDialogSettings objDownloadDialogSettings)
        {
            if (objDownloadRequests == null)
            {
                throw new ArgumentNullException("objDownloadRequests", "A valid non-null DownloadRequestList is expected");
            }
            if (objDownloadDialogSettings == null)
            {
                throw new ArgumentNullException("objDownloadDialogSettings", "A valid non-null DownloadDialogSettings is expected");
            }

            DownloadResultList objDownloadResults = null;
            using (DownloadDialog objFileDownloadDialog = new DownloadDialog())
            {
                objDownloadResults = objFileDownloadDialog.ShowDialogWithResult(objDownloadRequests, objDownloadDialogSettings);
            }

            return objDownloadResults;
        }

        #endregion
    }
}
