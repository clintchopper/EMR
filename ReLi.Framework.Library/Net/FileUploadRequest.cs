namespace ReLi.Framework.Library.Net
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Collections.Specialized;
    using System.Runtime.Serialization;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Threading;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Security;

    #endregion
        
	public class FileUploadRequest : UploadRequest
    {
        private const int DefaultBufferSize = 65536;

        public FileUploadRequest(string strSourceFilePath, string strDestinationFilePath)
            : this(strSourceFilePath, strDestinationFilePath, null)
        {}

        public FileUploadRequest(string strSourceFilePath, string strDestinationFilePath, Credentials objCredentials)
            : base(strSourceFilePath, strDestinationFilePath, objCredentials)
        {}

         public FileUploadRequest(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

         public FileUploadRequest(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        #region ITask Members

        public override TaskResultType Execute(UploadRequestSession objUploadRequestSession)
        {
            TaskResultType enuTaskResult = TaskResultType.Completed;

            bool blnFileExists = FileManager.Exists(base.Source);
            if (blnFileExists == false)
            {
                string strFailedMessage = "The source file '" + base.Source + "' does not exist.";
                throw new Exception(strFailedMessage);
            }

            objUploadRequestSession.UploadStats.Size = FileManager.Size(base.Source);
            objUploadRequestSession.UpdateProgess();

            FileWebRequest objWebRequest = (FileWebRequest)WebRequest.Create(base.Destination);
            objWebRequest.Credentials = base.Credentials.CreateNetworkCredentials();
            objWebRequest.Timeout = base.TimeOut;
            objWebRequest.Method = WebRequestMethods.File.UploadFile;
            objWebRequest.Proxy = ProxySettings.DefaultProxy;

            using (Stream objReguestStream = objWebRequest.GetRequestStream())
            {
                using (FileStream objFileStream = new FileStream(base.Source, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytBuffer = new byte[DefaultBufferSize];
                    int intBytesRead = objFileStream.Read(bytBuffer, 0, bytBuffer.Length);

                    while ((intBytesRead != 0) && (objUploadRequestSession.JobTicket.Cancelled == false))
                    {
                        objReguestStream.Write(bytBuffer, 0, intBytesRead);
                        objUploadRequestSession.UploadStats.BytesSent += intBytesRead;
                        intBytesRead = objFileStream.Read(bytBuffer, 0, bytBuffer.Length);

                        objUploadRequestSession.UpdateProgess();
                    }
                }

                objUploadRequestSession.UploadStats.EndTime = DateTime.Now;
                objUploadRequestSession.UpdateProgess();
            }

            enuTaskResult = ((objUploadRequestSession.JobTicket.Cancelled == true) ? TaskResultType.Cancelled : TaskResultType.Completed);
            return enuTaskResult;
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
