namespace ReLi.Framework.Library.Net
{
    #region Using Declarations

    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Threading;


    #endregion

    public partial class UploadFailedMessage : Form
    {
        private TaskActionType _enuTaskActionType;

        private UploadFailedMessage()
        {
            InitializeComponent();
            //this.Icon = Properties.Resources.resIconNetwork;
        }

        private TaskActionType ShowDialogWithResult(ITaskResult objTaskResult)
        {
            if (objTaskResult is UploadResult)
            {
                UploadResult objUploadResult = (UploadResult)objTaskResult;
                textBoxDetails.Text = objUploadResult.ToString();                
            }
            else
            {
                StringBuilder objText = new StringBuilder();
                objText.AppendLine("Result: " + objTaskResult.Result.ToString());
                objText.AppendLine("Details: " + objTaskResult.Details);
                textBoxDetails.Text = objText.ToString();
            }

            this.ShowDialog();
            return _enuTaskActionType;
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            _enuTaskActionType = TaskActionType.Retry;
            this.Close();
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            _enuTaskActionType = TaskActionType.Continue;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _enuTaskActionType = TaskActionType.Cancel;
            this.Close();
        }

        #region Static Members

        public static TaskActionType ShowMessage(ITaskResult objTaskResult)
        {
            TaskActionType enuResult = default(TaskActionType);

            using (UploadFailedMessage objDialog = new UploadFailedMessage())
            {
                enuResult = objDialog.ShowDialogWithResult(objTaskResult);
            }

            return enuResult;
        }

        #endregion      
    }
}
