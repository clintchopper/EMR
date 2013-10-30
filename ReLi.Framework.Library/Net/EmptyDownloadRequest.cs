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

    /// <summary>
    /// Represents an undefined, empty download request.
    /// </summary>
    public sealed class EmptyDownloadRequest : DownloadRequest 
    {
        public EmptyDownloadRequest()
            : base(string.Empty, string.Empty, null)
        {}

        public EmptyDownloadRequest(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public EmptyDownloadRequest(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        #region ITask Members


        public override TaskResultType Execute(DownloadRequestSession objDownloadRequestSession)
        {
            throw new NotSupportedException("The 'EmptyDownloadRequest' class cannot be executed.");
        }

        #endregion

        #region SerializableObject Members
        
        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);
        }

        #endregion

    }
}
