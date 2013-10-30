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

    public class UploadStats : ObjectBase, IUploadStats
    {
        private long _lngSize;
        private long _lngBytesSent;
        private long _lngBytesRemaining;
        private double _dblTransferRate;
        private DateTime _objStartTime;
        private DateTime _objEndTime;
        private UploadRequest _objUploadRequest;

        public UploadStats(UploadRequest objUploadRequest)
            : this(objUploadRequest, DateTime.Now)
        { }

        public UploadStats(UploadRequest objUploadRequest, DateTime dtStartTime)
            : base()
        {
            if (objUploadRequest == null)
            {
                throw new ArgumentNullException("objUploadRequest", "A valid non-null UploadRequest instance is expected.");
            }

            _objUploadRequest = objUploadRequest;
            _objStartTime = dtStartTime;
            _objEndTime = DateTime.MaxValue;
            _lngSize = 0;
            _lngBytesSent = 0;
            _lngBytesRemaining = 0;
            _dblTransferRate = 0;
        }

        public UploadStats(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public UploadStats(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        private long CalculateBytesRemaining(long lngSize, long lngBytesSent)
        {
            long lngBytesRemaining = (lngSize - lngBytesSent);
            if (lngBytesRemaining < 0)
            {
                lngBytesRemaining = 0;
            }

            return lngBytesRemaining;
        }

        private double CalculateTransferRate(TimeSpan objElapsedTime, long lngBytesSent)
        {
            double dblTransferRate = lngBytesSent / objElapsedTime.TotalSeconds;
            return dblTransferRate;
        }

        #region IUploadStats Members

        public UploadRequest UploadRequest
        {
            get
            {
                return _objUploadRequest;
            }
            private set
            {
                _objUploadRequest = value;
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
                _lngBytesRemaining = CalculateBytesRemaining(_lngSize, _lngBytesSent);
            }
        }

        public long BytesSent
        {
            get
            {
                return _lngBytesSent;
            }
            set
            {
                _lngBytesSent = value;
                _lngBytesRemaining = CalculateBytesRemaining(_lngSize, _lngBytesSent);
                _dblTransferRate = CalculateTransferRate(Duration, _lngBytesSent);
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

        public long GetFormattedBytesSent(TransferSizeType enuTransferSizeType)
        {
            long lngSent = BytesSent;
            switch (enuTransferSizeType)
            {
                case (TransferSizeType.Kilobyte):
                    lngSent = Convert.ToInt64(lngSent / 1024);
                    break;

                case (TransferSizeType.Megabyte):
                    lngSent = Convert.ToInt64(lngSent / (1024 * 1024));
                    break;

                case (TransferSizeType.Gigabyte):
                    lngSent = Convert.ToInt64(lngSent / (1024 * 1024 * 1024));
                    break;
            }

            return lngSent;
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
                return _objUploadRequest;
            }

        }

        #endregion

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Size", _lngSize);
            objSerializedObject.Values.Add("BytesSent", _lngBytesSent);
            objSerializedObject.Values.Add("BytesRemaining", _lngBytesRemaining);
            objSerializedObject.Values.Add("StartTime", _objStartTime);
            objSerializedObject.Values.Add("EndTime", _objEndTime);
            objSerializedObject.Objects.Add("UploadRequest", _objUploadRequest);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            _lngSize = objSerializedObject.Values.GetValue<long>("Size", 0);
            _lngBytesSent = objSerializedObject.Values.GetValue<long>("BytesSent", 0);
            _lngBytesRemaining = objSerializedObject.Values.GetValue<long>("BytesRemaining", 0);
            _objStartTime = objSerializedObject.Values.GetValue<DateTime>("StartTime", DateTime.MinValue);
            _objEndTime = objSerializedObject.Values.GetValue<DateTime>("EndTime", DateTime.MinValue);

            _objUploadRequest = objSerializedObject.Objects.GetObject<UploadRequest>("UploadRequest", null);
            if (_objUploadRequest == null)
            {
                _objUploadRequest = UploadRequest.Empty;
            }
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Size);
            objBinaryWriter.Write(BytesSent);
            objBinaryWriter.WriteDateTime(StartTime);
            objBinaryWriter.WriteDateTime(EndTime);
            objBinaryWriter.WriteTransportableObject(UploadRequest);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Size = objBinaryReader.ReadInt64();
            BytesSent = objBinaryReader.ReadInt64();
            StartTime = objBinaryReader.ReadDateTime();
            EndTime = objBinaryReader.ReadDateTime();
            UploadRequest = objBinaryReader.ReadTransportableObject<UploadRequest>();
        }

        #endregion

        #region Static Members

        private static object _objLock = new object();
        private static UploadStats _objEmpty = null;

        public static UploadStats Empty
        {
            get
            {
                if (_objEmpty == null)
                {
                    lock (_objLock)
                    {
                        if (_objEmpty == null)
                        {
                            _objEmpty = new UploadStats(UploadRequest.Empty);
                        }
                    }
                }

                return _objEmpty;
            }
        }

        #endregion

    }
}
