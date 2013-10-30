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
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Threading;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class FileDownloadRequestList : DownloadRequestList 
    {
         public FileDownloadRequestList()
            : base()
        {}

         public FileDownloadRequestList(IEnumerable<DownloadRequest> objFileDownloadRequest)
             : base(objFileDownloadRequest)
        {}

         public FileDownloadRequestList(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

         public FileDownloadRequestList(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

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

        #region Static Members 

        public static FileDownloadRequestList LoadFromDirectory(string strDirectory, string strTargetDirectory)
        {
            return LoadFromDirectory(strDirectory, strTargetDirectory, null);
        }

        public static FileDownloadRequestList LoadFromDirectory(string strDirectory, string strTargetDirectory, Credentials objCredentials)
        {
            if ((strDirectory == null) || (strDirectory.Length == 0))
            {
                throw new ArgumentOutOfRangeException("strDirectory", "A valid non-null, non-empty string is required.");
            }
            if ((strTargetDirectory == null) || (strTargetDirectory.Length == 0))
            {
                throw new ArgumentOutOfRangeException("strTargetDirectory", "A valid non-null, non-empty string is required.");
            }
            if (DirectoryManager.Exists(strDirectory) == false)
            {
                throw new DirectoryNotFoundException(string.Format("The directory '{0}' does not exist.", strDirectory));
            }

            FileDownloadRequestList objFileDownloadRequests = new FileDownloadRequestList();

            DirectoryInfo objDirectoryInfo = new DirectoryInfo(strDirectory);
            FileInfo[] objFiles = objDirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo objFile in objFiles)
            {
                string strTargetFilePath = objFile.FullName.Replace(strDirectory, strTargetDirectory);
                FileDownloadRequest objFileDownloadRequest = new FileDownloadRequest(objFile.FullName, strTargetFilePath, objCredentials);
                objFileDownloadRequests.Add(objFileDownloadRequest);
            }

            return objFileDownloadRequests;
        }

        #endregion

    }
}
