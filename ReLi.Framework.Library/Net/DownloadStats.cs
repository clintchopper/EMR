namespace ReLi.Framework.Library.Net
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Threading;
    using ReLi.Framework.Library.Serialization;

    #endregion
           
	public class DownloadStats : ObjectBase, IDownloadStats  
    {        
        private long _lngSize;
        private long _lngBytesReceived;
        private long _lngBytesRemaining;
        private double _dblTransferRate;
        private DateTime _objStartTime;
        private DateTime _objEndTime;
        private DownloadRequest _objDownloadRequest;

        public DownloadStats(DownloadRequest objDownloadRequest)
            : this(objDownloadRequest, DateTime.Now)
        {}

        public DownloadStats(DownloadRequest objDownloadRequest, DateTime dtStartTime)
            : base()
        {
            if (objDownloadRequest == null)
            {
                throw new ArgumentNullException("objDownloadRequest", "A valid non-null DownloadRequest instance is expected.");
            }

            _objDownloadRequest = objDownloadRequest;
            _objStartTime = dtStartTime;
            _objEndTime = DateTime.MaxValue;
            _lngSize = 0;
            _lngBytesReceived = 0;
            _lngBytesRemaining = 0;
            _dblTransferRate = 0;
        }

        public DownloadStats(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public DownloadStats(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        private long CalculateBytesRemaining(long lngSize, long lngBytesReceived)
        {
            long lngBytesRemaining = (lngSize - lngBytesReceived);
            if (lngBytesRemaining < 0)
            {
                lngBytesRemaining = 0;
            }

            return lngBytesRemaining;
        }

        private double CalculateTransferRate(TimeSpan objElapsedTime, long lngBytesReceived)
        {
            double dblTransferRate = lngBytesReceived / objElapsedTime.TotalSeconds;
            return dblTransferRate;
        }

        #region IDownloadStats Members

        public DownloadRequest DownloadRequest
        {
            get
            {
                return _objDownloadRequest;
            }
            private set
            {
                _objDownloadRequest = value;
            }
        }

        public long Size
        {
            get
            {
                return _lngSize;
            }
            set
            {
                _lngSize = value;
                _lngBytesRemaining = CalculateBytesRemaining(_lngSize, _lngBytesReceived);
            }
        }

        public long BytesReceived
        {
            get
            {
                return _lngBytesReceived;
            }
            set
            {
                _lngBytesReceived = value;
                _lngBytesRemaining = CalculateBytesRemaining(_lngSize, _lngBytesReceived);
                _dblTransferRate = CalculateTransferRate(Duration, _lngBytesReceived);
            }
        }

        public long BytesRemaining
        {
            get
            {
                return _lngBytesRemaining;
            }
        }

        public double TransferRate
        {
            get
            {
                return _dblTransferRate;
            }
        }

        public string TimeRemaining
        {
            get
            {
                string strTimeRemaining = "00:00:00";

                double dblTransferRate = TransferRate;
                double dblBytesRemaining = BytesRemaining;
                if ((dblTransferRate > 0) && (dblBytesRemaining > 0))
                {
                    int intSecondsRemaining = Convert.ToInt32(dblBytesRemaining / dblTransferRate);
                    TimeSpan objTimeRemaining = new TimeSpan(0, 0, intSecondsRemaining);

                    if (objTimeRemaining.Days > 0)
                    {
                        string strDurationFormat = "{0} day(s) {1:00}:{2:00}:{3:00}";
                        strTimeRemaining = String.Format(strDurationFormat, objTimeRemaining.Days, objTimeRemaining.Hours, objTimeRemaining.Minutes, objTimeRemaining.Seconds);
                    }
                    else
                    {
                        string strDurationFormat = "{0:00}:{1:00}:{2:00}";
                        strTimeRemaining = String.Format(strDurationFormat, objTimeRemaining.Hours, objTimeRemaining.Minutes, objTimeRemaining.Seconds);
                    }
                }

                return strTimeRemaining;
            }
        }

        public long GetFormattedSize(TransferSizeType enuTransferSizeType)
        {
            long lngSize = Size;
            switch (enuTransferSizeType)
            {
                case (TransferSizeType.Kilobyte):
                    lngSize = Convert.ToInt64(lngSize / 1024);
                    break;

                case (TransferSizeType.Megabyte):
                    lngSize = Convert.ToInt64(lngSize / (1024 * 1024));
                    break;

                case (TransferSizeType.Gigabyte):
                    lngSize = Convert.ToInt64(lngSize / (1024 * 1024 * 1024));
                    break;
            }

            return lngSize;
        }

        public long GetFormattedBytesReceived(TransferSizeType enuTransferSizeType)
        {
            long lngReceived = BytesReceived;
            switch (enuTransferSizeType)
            {
                case (TransferSizeType.Kilobyte):
                    lngReceived = Convert.ToInt64(lngReceived / 1024);
                    break;

                case (TransferSizeType.Megabyte):
                    lngReceived = Convert.ToInt64(lngReceived / (1024 * 1024));
                    break;

                case (TransferSizeType.Gigabyte):
                    lngReceived = Convert.ToInt64(lngReceived / (1024 * 1024 * 1024));
                    break;
            }

            return lngReceived;
        }

        public long GetFormattedBytesRemaining(TransferSizeType enuTransferSizeType)
        {
            long lngRemaining = BytesRemaining;
            switch (enuTransferSizeType)
            {
                case (TransferSizeType.Kilobyte):
                    lngRemaining = Convert.ToInt64(lngRemaining / 1024);
                    break;

                case (TransferSizeType.Megabyte):
                    lngRemaining = Convert.ToInt64(lngRemaining / (1024 * 1024));
                    break;

                case (TransferSizeType.Gigabyte):
                    lngRemaining = Convert.ToInt64(lngRemaining / (1024 * 1024 * 1024));
                    break;
            }

            return lngRemaining;
        }

        public string GetFormattedTransferRate(TransferSizeType enuSizeType, TransferTimeType enuTimeType)
        {
            string strTimePart = string.Empty;
            int intTimeMultiplier = 1;
            switch (enuTimeType)
            {
                case (TransferTimeType.Second):
                    intTimeMultiplier = 1;
                    strTimePart = "sec";
                    break;
                case (TransferTimeType.Minute):
                    intTimeMultiplier = 60;
                    strTimePart = "min";
                    break;
                case (TransferTimeType.Hour):
                    intTimeMultiplier = 3600;
                    strTimePart = "hr";
                    break;

            }

            string strSizePart = string.Empty;
            double dblSizeMultiplier = 1;
            switch (enuSizeType)
            {
                case (TransferSizeType.Byte):
                    dblSizeMultiplier = 1;
                    strSizePart = "b";
                    break;
                case (TransferSizeType.Kilobyte):
                    dblSizeMultiplier = ((double)1 / 1024);
                    strSizePart = "kb";
                    break;
                case (TransferSizeType.Megabyte):
                    dblSizeMultiplier = ((double)1 / (1024 * 1024));
                    strSizePart = "mb";
                    break;
                case (TransferSizeType.Gigabyte):
                    dblSizeMultiplier = ((double)1 / (1024 * 1024 * 1024));
                    strSizePart = "gb";
                    break;
            }

            /// The original value is in bytes / second.
            /// 
            double dblTransferRate = this.TransferRate * dblSizeMultiplier * intTimeMultiplier;
            string strTransferRate = dblTransferRate.ToString("0.00") + " " + strSizePart + "/" + strTimePart;

            return strTransferRate;
        }
        
        #endregion

        #region ITaskStats Interface

        public DateTime StartTime
        {
            get
            {
                return _objStartTime;
            }
            set
            {
                _objStartTime = value;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _objEndTime;
            }
            set
            {
                _objEndTime = value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                DateTime objEndTime = ((_objEndTime == DateTime.MaxValue) ? DateTime.Now : _objEndTime);
                TimeSpan objDuration = objEndTime - _objStartTime;

                return objDuration;
            }
        }

        public string FormattedDuration
        {
            get
            {
                string strFormattedDuration = string.Empty;
                TimeSpan objDuration = Duration;

                if (objDuration == TimeSpan.MinValue)
                {
                    strFormattedDuration = "00:00:00";
                }
                else if (objDuration.Days > 0)
                {
                    string strDurationFormat = "{0} day(s) {1:00}:{2:00}:{3:00}";
                    strFormattedDuration = String.Format(strDurationFormat, objDuration.Days, objDuration.Hours, objDuration.Minutes, objDuration.Seconds);
                }
                else
                {
                    string strDurationFormat = "{0:00}:{1:00}:{2:00}";
                    strFormattedDuration = String.Format(strDurationFormat, objDuration.Hours, objDuration.Minutes, objDuration.Seconds);
                }

                return strFormattedDuration;
            }
        }

        public string Details
        {
            get
            {
                string strDetails = "";

                return strDetails;
            }
        }

        public ITask Task
        {
            get
            {
                return _objDownloadRequest;
            }

        }

        #endregion

        #region SerializableObject Members
            
        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Size", _lngSize);
            objSerializedObject.Values.Add("BytesReceived", _lngBytesReceived);
            objSerializedObject.Values.Add("BytesRemaining", _lngBytesRemaining);
            objSerializedObject.Values.Add("StartTime", _objStartTime);
            objSerializedObject.Values.Add("EndTime", _objEndTime);
            objSerializedObject.Objects.Add("DownloadRequest", _objDownloadRequest);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            _lngSize = objSerializedObject.Values.GetValue<long>("Size", 0);
            _lngBytesReceived = objSerializedObject.Values.GetValue<long>("BytesReceived", 0);
            _lngBytesRemaining = objSerializedObject.Values.GetValue<long>("BytesRemaining", 0);
            _objStartTime = objSerializedObject.Values.GetValue<DateTime>("StartTime", DateTime.MinValue);
            _objEndTime = objSerializedObject.Values.GetValue<DateTime>("EndTime", DateTime.MinValue);
            
            _objDownloadRequest = objSerializedObject.Objects.GetObject<DownloadRequest>("DownloadRequest", null);
            if (_objDownloadRequest == null)
            {
                _objDownloadRequest = DownloadRequest.Empty;
            }
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Size);
            objBinaryWriter.Write(BytesReceived);
            objBinaryWriter.WriteDateTime(StartTime);
            objBinaryWriter.WriteDateTime(EndTime);
            objBinaryWriter.WriteTransportableObject(DownloadRequest);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Size = objBinaryReader.ReadInt64();
            BytesReceived = objBinaryReader.ReadInt64();
            StartTime = objBinaryReader.ReadDateTime();
            EndTime = objBinaryReader.ReadDateTime();
            DownloadRequest = objBinaryReader.ReadTransportableObject<DownloadRequest>();
        }

        #endregion

        #region Static Members

        private static object _objLock = new object();
        private static DownloadStats _objEmpty = null;

        public static DownloadStats Empty
        {
            get
            {
                if (_objEmpty == null)
                {
                    lock (_objLock)
                    {
                        if (_objEmpty == null)
                        {
                            _objEmpty = new DownloadStats(DownloadRequest.Empty);
                        }
                    }
                }

                return _objEmpty;
            }
        }

        #endregion

    }
}
