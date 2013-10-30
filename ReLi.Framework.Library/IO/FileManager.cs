namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Serialization;

    #endregion

	public class FileManager
    {
        public const string DeleteExtension = ".delete";

        public delegate void DeletedFileRenamedDelegate(string strOldFilePath, string strNewFilePath);
        public static event DeletedFileRenamedDelegate DeletedFileRenamed;

        public delegate void FileDeletedDelegate(string strFilePath);
        public static event FileDeletedDelegate FileDeleted;

        #region Static Members

        protected static void OnDeletedFileRenamed(string strOldFilePath, string strNewFilePath)
        {
            if (DeletedFileRenamed != null)
            {
                AsyncHelper.FireAndForget(DeletedFileRenamed, new object[] { strOldFilePath, strNewFilePath });   
            }
        }

        protected static void OnFileDeleted(string strFilePath)
        {
            if (FileDeleted != null)
            {
                AsyncHelper.FireAndForget(FileDeleted, new object[] { strFilePath });
            }
        }

        public static void Delete(string strFilePath)
        {
            FileManager.Delete(strFilePath, true);
        }

        public static void Delete(string strFilePath, bool blnRenameOnFail)
        {
            bool blnFileExists = FileManager.Exists(strFilePath);
            if (blnFileExists == false)
            {
                return;
            }

            Exception objActiveException = null;
            try
            {
                FileAttributes enuFileAttributes = FileManager.GetAttributes(strFilePath);
                if ((enuFileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    FileManager.SetAttributes(strFilePath, FileAttributes.Normal);
                }
            }
            catch(Exception objException)
            {
                objActiveException = objException;
            }

            if (objActiveException == null)
            {
                try
                {
                    File.Delete(strFilePath);
                    OnFileDeleted(strFilePath);
                }
                catch (Exception objException)
                {
                    objActiveException = objException;
                }
            }

            if ((objActiveException != null) && (blnRenameOnFail == true))
            {
                objActiveException = null;
                
                string strRenameFilePath = CreateDeleteFileName(strFilePath);

                try
                {
                    File.Move(strFilePath, strRenameFilePath);
                    OnDeletedFileRenamed(strFilePath, strRenameFilePath);
                }
                catch (Exception objException)
                {
                    string strErrorMessage = "Error while attempting to rename (move) file '" + strFilePath + "' for deletion:\r\n";
                    objActiveException = new Exception(strErrorMessage, objException);
                }
            }

            if (objActiveException != null)
            {
                throw objActiveException;
            }
        }

        public static bool Exists(string strFilePath)
        {
            bool blnExists = false;

            if ((strFilePath != null) && (strFilePath.Length > 0))
            {
                blnExists = File.Exists(strFilePath);
            }

            return blnExists;
        }

        public static void SetAttributes(string strFilePath, FileAttributes enuFileAttributes)
        {
            bool blnExists = FileManager.Exists(strFilePath);
            if (blnExists == true)
            {
                File.SetAttributes(strFilePath, enuFileAttributes);
            }
        }

        public static FileAttributes GetAttributes(string strFilePath)
        {
            return FileManager.GetAttributes(strFilePath, FileAttributes.Normal);
        }

        public static FileAttributes GetAttributes(string strFilePath, FileAttributes enuDefaultFileAttributes)
        {
            FileAttributes enuFileAttributes = enuDefaultFileAttributes;

            bool blnExists = FileManager.Exists(strFilePath);
            if (blnExists == true)
            {
                enuFileAttributes = File.GetAttributes(strFilePath);
            }

            return enuFileAttributes;
        }

        public static void Move(string strFilePath, string strTargetDirectory)
        {
            Move(strFilePath, strTargetDirectory, false, false);
        }

        public static void Move(string strFilePath, string strTargetDirectory, bool blnMoveOnlyIfNewer, bool blnBackupTarget)
        {
            bool blnFileExists = FileManager.Exists(strFilePath);
            if (blnFileExists == false)
            {
                throw new Exception("Source file '" + strFilePath + "' does not exist.");
            }
            FileInfo objFileToMove = new FileInfo(strFilePath);

            bool blnTargetDirectoryExists = DirectoryManager.Exists(strTargetDirectory);
            if (blnTargetDirectoryExists == false)
            {
                try
                {
                    DirectoryManager.Create(strTargetDirectory);
                }
                catch (Exception objException)
                {
                    throw new Exception("Target directory '" + strTargetDirectory + "' could not be created.", objException);
                }
            }
            DirectoryInfo objTargetDirectory = new DirectoryInfo(strTargetDirectory);

            string strTargetFilePath = Path.Combine(strTargetDirectory, objFileToMove.Name);
            bool blnTargetFileExists = FileManager.Exists(strTargetFilePath);
            if (blnTargetFileExists == true)
            {
                FileInfo objTargetFileInfo = new FileInfo(strTargetFilePath);
                if (objFileToMove.LastWriteTime.Ticks < objTargetFileInfo.LastWriteTime.Ticks)
                {
                    if (blnBackupTarget == true)
                    {
                        BackupFile(objFileToMove.FullName, strTargetDirectory);
                    }

                    if (blnMoveOnlyIfNewer == false)
                    {
                        FileManager.Delete(strTargetFilePath);
                        File.Move(objFileToMove.FullName, strTargetFilePath);
                    }
                    else
                    {
                        objFileToMove.Attributes = FileAttributes.Normal;
                        objFileToMove.Delete();
                    }
                }
                else
                {
                    if (blnBackupTarget == true)
                    {
                        BackupFile(strTargetFilePath, strTargetDirectory);
                    }

                    FileManager.Delete(strTargetFilePath);
                    File.Move(objFileToMove.FullName, strTargetFilePath);
                }
            }
            else
            {
                File.Move(objFileToMove.FullName, strTargetFilePath);
            }
        }

        public static void Copy(string strSourceFilePath, string strTargetFilePath)
        {
            bool blnSourceFileExists = FileManager.Exists(strSourceFilePath);
            if (blnSourceFileExists == false)
            {
                string strErrorMessage = "The source file '" + strSourceFilePath + "' does not exist.";
                throw new Exception(strErrorMessage);
            }

            bool blnTargetFileExists = FileManager.Exists(strTargetFilePath);
            if (blnTargetFileExists == true)
            {
                Delete(strTargetFilePath, true);
            }

            File.Copy(strSourceFilePath, strTargetFilePath);
        }

        public static void Copy(string strFilePath, string strTargetDirectory, bool blnCopyOnlyIfNewer, bool blnBackupTarget)
        {

            bool blnFileExists = FileManager.Exists(strFilePath);
            if (blnFileExists == false)
            {
                throw new Exception("Source file '" + strFilePath + "' does not exist.");
            }
            FileInfo objFileToCopy = new FileInfo(strFilePath);

            bool blnTargetDirectoryExists = DirectoryManager.Exists(strTargetDirectory);
            if (blnTargetDirectoryExists == false)
            {
                try
                {
                    DirectoryManager.Create(strTargetDirectory);
                }
                catch (Exception objException)
                {
                    throw new Exception("Target directory '" + strTargetDirectory + "' could not be created.", objException);
                }
            }
            DirectoryInfo objTargetDirectory = new DirectoryInfo(strTargetDirectory);

            string strTargetFilePath = Path.Combine(strTargetDirectory, objFileToCopy.Name);
            bool blnTargetFileExists = FileManager.Exists(strTargetFilePath);
            if (blnTargetFileExists == true) 
            {
                FileInfo objTargetFileInfo = new FileInfo(strTargetFilePath);
                if (objFileToCopy.LastWriteTime.Ticks < objTargetFileInfo.LastWriteTime.Ticks)
                {
                    if (blnBackupTarget == true)
                    {
                        BackupFile(objFileToCopy.FullName, strTargetDirectory);
                    }
                    
                    if (blnCopyOnlyIfNewer == false)
                    {
                        objTargetFileInfo.Attributes = FileAttributes.Normal;
                        objTargetFileInfo.Delete();
                        File.Copy(objFileToCopy.FullName, strTargetFilePath);
                    }
                }
                else
                {
                    if (blnBackupTarget == true)
                    {
                        BackupFile(strTargetFilePath, strTargetDirectory);
                    }

                    objTargetFileInfo.Attributes = FileAttributes.Normal;
                    objTargetFileInfo.Delete();
                    File.Copy(objFileToCopy.FullName, strTargetFilePath);
                }
            }
            else
            {
                File.Copy(objFileToCopy.FullName, strTargetFilePath);
            }
        }

        public static long Size(string strFilePath)
        {
            long lngSize = 0;

            bool blnFileExists = FileManager.Exists(strFilePath);
            if (blnFileExists == true)
            {
                FileInfo objFileInfo = new FileInfo(strFilePath);
                lngSize = objFileInfo.Length;
            }

            return lngSize;
        }

        private static string CreateDeleteFileName(string strFilePath)
        {
            int intIndex = 0;
            string strRenameFilePath = strFilePath + DeleteExtension;
            while (FileManager.Exists(strRenameFilePath) == true)
            {
                intIndex++;
                strRenameFilePath = strFilePath + "." + intIndex.ToString() + DeleteExtension; 
            }

            return strRenameFilePath;
        }

        private static void BackupFile(string strFileToBackup, string strBaseTargetDirectory)
        {
            string strBackedUpDirectory = Path.GetFileNameWithoutExtension(strFileToBackup) + "_" + Path.GetExtension(strFileToBackup);
            strBackedUpDirectory = Path.Combine(strBaseTargetDirectory, strBackedUpDirectory);

            bool blnBackedUpDirectoryExists = DirectoryManager.Exists(strBackedUpDirectory);
            if (blnBackedUpDirectoryExists == false)
            {
                try
                {
                    DirectoryManager.Create(strBackedUpDirectory);
                }
                catch (Exception objException)
                {
                    throw new Exception("Backup directory '" + strBackedUpDirectory + "' could not be created.", objException);
                }
            }

            string strUniqueValue = DateTime.Now.Ticks.ToString();
            string strBackedUpFileName = Path.GetFileNameWithoutExtension(strFileToBackup) + "_" + strUniqueValue + Path.GetExtension(strFileToBackup);
            strBackedUpFileName = Path.Combine(strBackedUpDirectory, strBackedUpFileName);

            FileManager.Copy(strFileToBackup, strBackedUpFileName);
        }

        #endregion

    }
}
