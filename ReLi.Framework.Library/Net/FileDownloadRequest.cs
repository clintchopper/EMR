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
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Threading;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.IO;

    #endregion
        
	public class FileDownloadRequest : DownloadRequest
    {
        private const int DefaultBufferSize = 65536;

        public FileDownloadRequest(string strSourceFilePath, string strDestinationFilePath)
            : this(strSourceFilePath, strDestinationFilePath, null)
        {}

        public FileDownloadRequest(string strSourceFilePath, string strDestinationFilePath, Credentials objCredentials)
            : base(strSourceFilePath, strDestinationFilePath, objCredentials)
        {}

        public FileDownloadRequest(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public FileDownloadRequest(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        #region ITask Members

        public override TaskResultType Execute(DownloadRequestSession objDownloadRequestSession)
        {
            TaskResultType enuTaskResult = TaskResultType.Completed;

            objDownloadRequestSession.UpdateProgess();

            FileWebRequest objWebRequest = (FileWebRequest)WebRequest.Create(base.Source);
            objWebRequest.Proxy = ProxySettings.DefaultProxy;
            objWebRequest.Credentials = ((base.Credentials != null) ? base.Credentials.CreateNetworkCredentials() : null);
            objWebRequest.Timeout = base.TimeOut;

            using (FileWebResponse objWebResponse = (FileWebResponse)objWebRequest.GetResponse())
            {
                objDownloadRequestSession.DownloadStats.Size = objWebResponse.ContentLength;
                objDownloadRequestSession.UpdateProgess();

                bool blnFileExists = FileManager.Exists(base.Destination);
                if (blnFileExists == true)
                {
                    FileManager.Delete(base.Destination, true);
                }

                string strDirectory = Path.GetDirectoryName(base.Destination);
                bool blnDirectoryExists = DirectoryManager.Exists(strDirectory);
                if (blnDirectoryExists == false)
                {
                    DirectoryManager.Create(strDirectory);
                }

                using (Stream objResponseStream = objWebResponse.GetResponseStream())
                {
                    using (FileStream objFileStream = new FileStream(base.Destination, FileMode.Create, FileAccess.Write))
                    {
                        byte[] bytBuffer = new byte[DefaultBufferSize];
                        int intBytesRead = objResponseStream.Read(bytBuffer, 0, bytBuffer.Length);

                        while ((intBytesRead != 0) && (objDownloadRequestSession.JobTicket.Cancelled == false))
                        {
                            objFileStream.Write(bytBuffer, 0, intBytesRead);
                            objFileStream.Flush();

                            objDownloadRequestSession.DownloadStats.BytesReceived += intBytesRead;
                            objDownloadRequestSession.UpdateProgess();

                            intBytesRead = objResponseStream.Read(bytBuffer, 0, bytBuffer.Length);
                        }
                    }
                }

                objDownloadRequestSession.DownloadStats.EndTime = DateTime.Now;
                objDownloadRequestSession.UpdateProgess();

                if (objDownloadRequestSession.JobTicket.Cancelled == true)
                {
                    FileManager.Delete(base.Destination);
                }
            }

            enuTaskResult = ((objDownloadRequestSession.JobTicket.Cancelled == true) ? TaskResultType.Cancelled : TaskResultType.Completed);
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
