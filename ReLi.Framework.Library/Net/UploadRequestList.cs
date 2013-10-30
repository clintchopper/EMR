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
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Threading;
    using ReLi.Framework.Library.Serialization;

    #endregion

    public class UploadRequestList : ObjectListBase<UploadRequest>
    {
        public UploadRequestList()
            : base()
        { }

        public UploadRequestList(IEnumerable<UploadRequest> objUploadRequests)
            : base(objUploadRequests)
        { }

        public UploadRequestList(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public UploadRequestList(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        public IEnumerable<ITask> GetTasks()
        {
            List<ITask> objTasks = new List<ITask>();
            foreach (UploadRequest objUploadRequest in this)
            {
                objTasks.Add(objUploadRequest);
            }

            return objTasks.ToArray();
        }

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
