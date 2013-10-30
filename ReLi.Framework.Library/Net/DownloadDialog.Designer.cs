namespace ReLi.Framework.Library.Net
{
    partial class DownloadDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.progressBarOverall = new System.Windows.Forms.ProgressBar();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.labelLine1 = new System.Windows.Forms.Label();
            this.progressBarCurrent = new System.Windows.Forms.ProgressBar();
            this.labelSource = new System.Windows.Forms.Label();
            this.textSizeInBytes = new System.Windows.Forms.Label();
            this.labelSpeed = new System.Windows.Forms.Label();
            this.labelSizeInBytes = new System.Windows.Forms.Label();
            this.textSpeed = new System.Windows.Forms.Label();
            this.labelTarget = new System.Windows.Forms.Label();
            this.labelDuration = new System.Windows.Forms.Label();
            this.labelBytesReceived = new System.Windows.Forms.Label();
            this.labelTimeRemaining = new System.Windows.Forms.Label();
            this.textDuration = new System.Windows.Forms.Label();
            this.labelBytesRemaining = new System.Windows.Forms.Label();
            this.textTimeRemaining = new System.Windows.Forms.Label();
            this.textBytesRemaining = new System.Windows.Forms.Label();
            this.textBytesReceived = new System.Windows.Forms.Label();
            this.buttonDetails = new System.Windows.Forms.Button();
            this.timerUpdateProgress = new System.Windows.Forms.Timer(this.components);
            this.listViewStatus = new System.Windows.Forms.ListView();
            this.columnHeaderTarget = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonPauseContinue = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.panelLeftBorder = new System.Windows.Forms.Panel();
            this.panelRightBorder = new System.Windows.Forms.Panel();
            this.pathLabelSource = new ReLi.Framework.Library.IO.PathLabelControl();
            this.pathLabelTarget = new ReLi.Framework.Library.IO.PathLabelControl();
            this.labelOverall = new ReLi.Framework.Library.IO.PathLabelControl();
            this.groupBoxDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBarOverall
            // 
            this.progressBarOverall.Location = new System.Drawing.Point(27, 43);
            this.progressBarOverall.Name = "progressBarOverall";
            this.progressBarOverall.Size = new System.Drawing.Size(371, 21);
            this.progressBarOverall.TabIndex = 0;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(256, 393);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 27);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBoxDetails
            // 
            this.groupBoxDetails.Controls.Add(this.labelLine1);
            this.groupBoxDetails.Controls.Add(this.progressBarCurrent);
            this.groupBoxDetails.Controls.Add(this.labelSource);
            this.groupBoxDetails.Controls.Add(this.textSizeInBytes);
            this.groupBoxDetails.Controls.Add(this.labelSpeed);
            this.groupBoxDetails.Controls.Add(this.labelSizeInBytes);
            this.groupBoxDetails.Controls.Add(this.textSpeed);
            this.groupBoxDetails.Controls.Add(this.labelTarget);
            this.groupBoxDetails.Controls.Add(this.labelDuration);
            this.groupBoxDetails.Controls.Add(this.labelBytesReceived);
            this.groupBoxDetails.Controls.Add(this.labelTimeRemaining);
            this.groupBoxDetails.Controls.Add(this.textDuration);
            this.groupBoxDetails.Controls.Add(this.labelBytesRemaining);
            this.groupBoxDetails.Controls.Add(this.textTimeRemaining);
            this.groupBoxDetails.Controls.Add(this.textBytesRemaining);
            this.groupBoxDetails.Controls.Add(this.textBytesReceived);
            this.groupBoxDetails.Controls.Add(this.pathLabelSource);
            this.groupBoxDetails.Controls.Add(this.pathLabelTarget);
            this.groupBoxDetails.Location = new System.Drawing.Point(28, 177);
            this.groupBoxDetails.Name = "groupBoxDetails";
            this.groupBoxDetails.Size = new System.Drawing.Size(452, 201);
            this.groupBoxDetails.TabIndex = 6;
            this.groupBoxDetails.TabStop = false;
            this.groupBoxDetails.Visible = false;
            // 
            // labelLine1
            // 
            this.labelLine1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelLine1.Location = new System.Drawing.Point(14, 76);
            this.labelLine1.Name = "labelLine1";
            this.labelLine1.Size = new System.Drawing.Size(423, 2);
            this.labelLine1.TabIndex = 0;
            // 
            // progressBarCurrent
            // 
            this.progressBarCurrent.Location = new System.Drawing.Point(14, 162);
            this.progressBarCurrent.Name = "progressBarCurrent";
            this.progressBarCurrent.Size = new System.Drawing.Size(423, 21);
            this.progressBarCurrent.TabIndex = 3;
            // 
            // labelSource
            // 
            this.labelSource.AutoSize = true;
            this.labelSource.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelSource.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelSource.Location = new System.Drawing.Point(14, 21);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(53, 14);
            this.labelSource.TabIndex = 51;
            this.labelSource.Text = "Source :";
            // 
            // textSizeInBytes
            // 
            this.textSizeInBytes.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.textSizeInBytes.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textSizeInBytes.Location = new System.Drawing.Point(100, 91);
            this.textSizeInBytes.Name = "textSizeInBytes";
            this.textSizeInBytes.Size = new System.Drawing.Size(109, 15);
            this.textSizeInBytes.TabIndex = 29;
            this.textSizeInBytes.Text = "0 bytes";
            this.textSizeInBytes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSpeed
            // 
            this.labelSpeed.AutoSize = true;
            this.labelSpeed.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelSpeed.Location = new System.Drawing.Point(264, 113);
            this.labelSpeed.Name = "labelSpeed";
            this.labelSpeed.Size = new System.Drawing.Size(50, 14);
            this.labelSpeed.TabIndex = 47;
            this.labelSpeed.Text = "Speed :";
            // 
            // labelSizeInBytes
            // 
            this.labelSizeInBytes.AutoSize = true;
            this.labelSizeInBytes.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelSizeInBytes.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelSizeInBytes.Location = new System.Drawing.Point(53, 91);
            this.labelSizeInBytes.Name = "labelSizeInBytes";
            this.labelSizeInBytes.Size = new System.Drawing.Size(36, 14);
            this.labelSizeInBytes.TabIndex = 15;
            this.labelSizeInBytes.Text = "Size :";
            // 
            // textSpeed
            // 
            this.textSpeed.AutoSize = true;
            this.textSpeed.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.textSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textSpeed.Location = new System.Drawing.Point(323, 113);
            this.textSpeed.Name = "textSpeed";
            this.textSpeed.Size = new System.Drawing.Size(57, 14);
            this.textSpeed.TabIndex = 48;
            this.textSpeed.Text = "0 MB/sec";
            // 
            // labelTarget
            // 
            this.labelTarget.AutoSize = true;
            this.labelTarget.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelTarget.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelTarget.Location = new System.Drawing.Point(19, 43);
            this.labelTarget.Name = "labelTarget";
            this.labelTarget.Size = new System.Drawing.Size(52, 14);
            this.labelTarget.TabIndex = 53;
            this.labelTarget.Text = "Target :";
            // 
            // labelDuration
            // 
            this.labelDuration.AutoSize = true;
            this.labelDuration.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelDuration.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelDuration.Location = new System.Drawing.Point(253, 91);
            this.labelDuration.Name = "labelDuration";
            this.labelDuration.Size = new System.Drawing.Size(61, 14);
            this.labelDuration.TabIndex = 45;
            this.labelDuration.Text = "Duration :";
            // 
            // labelBytesReceived
            // 
            this.labelBytesReceived.AutoSize = true;
            this.labelBytesReceived.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelBytesReceived.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelBytesReceived.Location = new System.Drawing.Point(24, 113);
            this.labelBytesReceived.Name = "labelBytesReceived";
            this.labelBytesReceived.Size = new System.Drawing.Size(64, 14);
            this.labelBytesReceived.TabIndex = 37;
            this.labelBytesReceived.Text = "Received :";
            // 
            // labelTimeRemaining
            // 
            this.labelTimeRemaining.AutoSize = true;
            this.labelTimeRemaining.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelTimeRemaining.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelTimeRemaining.Location = new System.Drawing.Point(239, 136);
            this.labelTimeRemaining.Name = "labelTimeRemaining";
            this.labelTimeRemaining.Size = new System.Drawing.Size(70, 14);
            this.labelTimeRemaining.TabIndex = 49;
            this.labelTimeRemaining.Text = "Remaining :";
            // 
            // textDuration
            // 
            this.textDuration.AutoSize = true;
            this.textDuration.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.textDuration.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textDuration.Location = new System.Drawing.Point(323, 91);
            this.textDuration.Name = "textDuration";
            this.textDuration.Size = new System.Drawing.Size(57, 14);
            this.textDuration.TabIndex = 46;
            this.textDuration.Text = "00:00:00";
            // 
            // labelBytesRemaining
            // 
            this.labelBytesRemaining.AutoSize = true;
            this.labelBytesRemaining.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelBytesRemaining.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelBytesRemaining.Location = new System.Drawing.Point(14, 137);
            this.labelBytesRemaining.Name = "labelBytesRemaining";
            this.labelBytesRemaining.Size = new System.Drawing.Size(70, 14);
            this.labelBytesRemaining.TabIndex = 40;
            this.labelBytesRemaining.Text = "Remaining :";
            // 
            // textTimeRemaining
            // 
            this.textTimeRemaining.AutoSize = true;
            this.textTimeRemaining.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.textTimeRemaining.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textTimeRemaining.Location = new System.Drawing.Point(323, 136);
            this.textTimeRemaining.Name = "textTimeRemaining";
            this.textTimeRemaining.Size = new System.Drawing.Size(57, 14);
            this.textTimeRemaining.TabIndex = 50;
            this.textTimeRemaining.Text = "00:00:00";
            // 
            // textBytesRemaining
            // 
            this.textBytesRemaining.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.textBytesRemaining.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBytesRemaining.Location = new System.Drawing.Point(100, 137);
            this.textBytesRemaining.Name = "textBytesRemaining";
            this.textBytesRemaining.Size = new System.Drawing.Size(109, 15);
            this.textBytesRemaining.TabIndex = 44;
            this.textBytesRemaining.Text = "0 bytes";
            this.textBytesRemaining.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBytesReceived
            // 
            this.textBytesReceived.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.textBytesReceived.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBytesReceived.Location = new System.Drawing.Point(100, 113);
            this.textBytesReceived.Name = "textBytesReceived";
            this.textBytesReceived.Size = new System.Drawing.Size(109, 15);
            this.textBytesReceived.TabIndex = 43;
            this.textBytesReceived.Text = "0 bytes";
            this.textBytesReceived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonDetails
            // 
            this.buttonDetails.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.buttonDetails.Location = new System.Drawing.Point(404, 40);
            this.buttonDetails.Name = "buttonDetails";
            this.buttonDetails.Size = new System.Drawing.Size(75, 27);
            this.buttonDetails.TabIndex = 7;
            this.buttonDetails.Text = "Details";
            this.buttonDetails.UseVisualStyleBackColor = true;
            this.buttonDetails.Click += new System.EventHandler(this.buttonDetails_Click);
            // 
            // timerUpdateProgress
            // 
            this.timerUpdateProgress.Tick += new System.EventHandler(this.timerUpdateProgress_Tick);
            // 
            // listViewStatus
            // 
            this.listViewStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTarget,
            this.columnHeaderStatus});
            this.listViewStatus.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.listViewStatus.FullRowSelect = true;
            this.listViewStatus.GridLines = true;
            this.listViewStatus.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewStatus.HideSelection = false;
            this.listViewStatus.Location = new System.Drawing.Point(27, 80);
            this.listViewStatus.MultiSelect = false;
            this.listViewStatus.Name = "listViewStatus";
            this.listViewStatus.Size = new System.Drawing.Size(452, 89);
            this.listViewStatus.TabIndex = 8;
            this.listViewStatus.UseCompatibleStateImageBehavior = false;
            this.listViewStatus.View = System.Windows.Forms.View.Details;
            this.listViewStatus.Visible = false;
            // 
            // columnHeaderTarget
            // 
            this.columnHeaderTarget.Text = "File Name";
            this.columnHeaderTarget.Width = 323;
            // 
            // columnHeaderStatus
            // 
            this.columnHeaderStatus.Text = "Status";
            this.columnHeaderStatus.Width = 104;
            // 
            // buttonPauseContinue
            // 
            this.buttonPauseContinue.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonPauseContinue.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.buttonPauseContinue.Location = new System.Drawing.Point(175, 393);
            this.buttonPauseContinue.Name = "buttonPauseContinue";
            this.buttonPauseContinue.Size = new System.Drawing.Size(75, 27);
            this.buttonPauseContinue.TabIndex = 9;
            this.buttonPauseContinue.Text = "Pause";
            this.buttonPauseContinue.UseVisualStyleBackColor = true;
            this.buttonPauseContinue.Click += new System.EventHandler(this.buttonPauseContinue_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(12, 439);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(483, 12);
            this.panel1.TabIndex = 111;
            // 
            // panelTitle
            // 
            this.panelTitle.BackColor = System.Drawing.Color.White;
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(12, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(483, 12);
            this.panelTitle.TabIndex = 108;
            // 
            // panelLeftBorder
            // 
            this.panelLeftBorder.BackColor = System.Drawing.Color.White;
            this.panelLeftBorder.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeftBorder.Location = new System.Drawing.Point(0, 0);
            this.panelLeftBorder.Name = "panelLeftBorder";
            this.panelLeftBorder.Size = new System.Drawing.Size(12, 451);
            this.panelLeftBorder.TabIndex = 109;
            // 
            // panelRightBorder
            // 
            this.panelRightBorder.BackColor = System.Drawing.Color.White;
            this.panelRightBorder.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRightBorder.Location = new System.Drawing.Point(495, 0);
            this.panelRightBorder.Name = "panelRightBorder";
            this.panelRightBorder.Size = new System.Drawing.Size(12, 451);
            this.panelRightBorder.TabIndex = 110;
            // 
            // pathLabelSource
            // 
            this.pathLabelSource.AutoEllipsis = ((ReLi.Framework.Library.IO.EllipsisFormat)((ReLi.Framework.Library.IO.EllipsisFormat.End | ReLi.Framework.Library.IO.EllipsisFormat.Start)));
            this.pathLabelSource.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.pathLabelSource.Location = new System.Drawing.Point(78, 21);
            this.pathLabelSource.Name = "pathLabelSource";
            this.pathLabelSource.Size = new System.Drawing.Size(347, 15);
            this.pathLabelSource.TabIndex = 55;
            // 
            // pathLabelTarget
            // 
            this.pathLabelTarget.AutoEllipsis = ((ReLi.Framework.Library.IO.EllipsisFormat)((ReLi.Framework.Library.IO.EllipsisFormat.End | ReLi.Framework.Library.IO.EllipsisFormat.Start)));
            this.pathLabelTarget.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.pathLabelTarget.Location = new System.Drawing.Point(78, 43);
            this.pathLabelTarget.Name = "pathLabelTarget";
            this.pathLabelTarget.Size = new System.Drawing.Size(347, 15);
            this.pathLabelTarget.TabIndex = 56;
            // 
            // labelOverall
            // 
            this.labelOverall.AutoEllipsis = ((ReLi.Framework.Library.IO.EllipsisFormat)((ReLi.Framework.Library.IO.EllipsisFormat.End | ReLi.Framework.Library.IO.EllipsisFormat.Start)));
            this.labelOverall.Location = new System.Drawing.Point(28, 24);
            this.labelOverall.Name = "labelOverall";
            this.labelOverall.Size = new System.Drawing.Size(370, 15);
            this.labelOverall.TabIndex = 112;
            this.labelOverall.Tag = "Downloading :  {0} ({1} or {2})";
            // 
            // DownloadDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(235)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(507, 451);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelTitle);
            this.Controls.Add(this.panelLeftBorder);
            this.Controls.Add(this.panelRightBorder);
            this.Controls.Add(this.buttonDetails);
            this.Controls.Add(this.progressBarOverall);
            this.Controls.Add(this.buttonPauseContinue);
            this.Controls.Add(this.groupBoxDetails);
            this.Controls.Add(this.listViewStatus);
            this.Controls.Add(this.labelOverall);
            this.Controls.Add(this.buttonCancel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DownloadDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReLi Download Manager";
            this.groupBoxDetails.ResumeLayout(false);
            this.groupBoxDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarOverall;
        protected System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxDetails;
        internal System.Windows.Forms.Label labelSizeInBytes;
        private System.Windows.Forms.ProgressBar progressBarCurrent;
        internal System.Windows.Forms.Label textSizeInBytes;
        internal System.Windows.Forms.Label labelBytesRemaining;
        internal System.Windows.Forms.Label labelBytesReceived;
        internal System.Windows.Forms.Label textTimeRemaining;
        internal System.Windows.Forms.Label labelTimeRemaining;
        internal System.Windows.Forms.Label textSpeed;
        internal System.Windows.Forms.Label labelSpeed;
        internal System.Windows.Forms.Label labelDuration;
        internal System.Windows.Forms.Label textDuration;
        internal System.Windows.Forms.Label textBytesRemaining;
        internal System.Windows.Forms.Label textBytesReceived;
        internal System.Windows.Forms.Label labelSource;
        internal System.Windows.Forms.Label labelTarget;
        protected System.Windows.Forms.Button buttonDetails;
        private System.Windows.Forms.Label labelLine1;
        private System.Windows.Forms.Timer timerUpdateProgress;
        private ReLi.Framework.Library.IO.PathLabelControl pathLabelSource;
        private ReLi.Framework.Library.IO.PathLabelControl pathLabelTarget;
        private System.Windows.Forms.ListView listViewStatus;
        private System.Windows.Forms.ColumnHeader columnHeaderTarget;
        private System.Windows.Forms.ColumnHeader columnHeaderStatus;
        protected System.Windows.Forms.Button buttonPauseContinue;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Panel panelLeftBorder;
        private System.Windows.Forms.Panel panelRightBorder;
        private IO.PathLabelControl labelOverall;
    }
}