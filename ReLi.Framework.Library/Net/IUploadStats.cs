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

    public interface IUploadStats : ITaskStats
    {
        UploadRequest UploadRequest
        {
            get;
        }

        long Size
        {
            get;
        }

        long BytesSent
        {
            get;
        }

        long BytesRemaining
        {
            get;
        }

        double TransferRate
        {
            get;
        }

        string TimeRemaining
        {
            get;
        }

        long GetFormattedSize(TransferSizeType enuSizeType);

        string GetFormattedTransferRate(TransferSizeType enuSizeType, TransferTimeType enuTimeType);

        long GetFormattedBytesSent(TransferSizeType enuTransferSizeType);

        long GetFormattedBytesRemaining(TransferSizeType enuTransferSizeType);
    }
}
