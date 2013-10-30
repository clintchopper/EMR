namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Serialization;

    #endregion

	public class DirectoryManager
    {
        #region Static Members

        public static void Create(string strDirectoryPath)
        {
            bool blnDirectoryExists = DirectoryManager.Exists(strDirectoryPath);
            if (blnDirectoryExists == false)
            {
                Directory.CreateDirectory(strDirectoryPath);
            }
        }

        public static void Delete(string strDirectoryPath)
        {
            DirectoryManager.Delete(strDirectoryPath, true);
        }

        public static void Delete(string strDirectoryPath, bool blnRenameOnFail)
        {
            bool blnDirectoryExists = DirectoryManager.Exists(strDirectoryPath);
            if (blnDirectoryExists == false)
            {
                return;
            }

            Exception objActiveException = null;
            try
            {
                FileAttributes enuFileAttributes = DirectoryManager.GetAttributes(strDirectoryPath);
                if ((enuFileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    DirectoryManager.SetAttributes(strDirectoryPath, FileAttributes.Normal);
                }
            }
            catch (Exception objException)
            {
                objActiveException = objException;
            }

            if (objActiveException == null)
            {
                try
                {
                    Directory.Delete(strDirectoryPath, true);
                }
                catch (Exception objException)
                {
                    objActiveException = objException;
                }
            }

            if ((objActiveException != null) && (blnRenameOnFail == true))
            {
                objActiveException = null;
                string strRenameDirectoryPath = DirectoryManager.CreateDeleteDirectoryName(strDirectoryPath);

                try
                {
                    DirectoryManager.Move(strDirectoryPath, strRenameDirectoryPath, "*.*");
                }
                catch (Exception objException)
                {
                    string strErrorMessage = "Error while attempting to rename (move) directory '" + strDirectoryPath + "' for deletion:\r\n";
                    objActiveException = new Exception(strErrorMessage, objException);
                }
            }

            if (objActiveException != null)
            {
                throw objActiveException;
            }
        }

        public static bool Exists(string strDirectoryPath)
        {
            bool blnExists = Directory.Exists(strDirectoryPath);
            return blnExists;
        }

        public static void SetAttributes(string strDirectoryPath, FileAttributes enuFileAttributes)
        {
            bool blnExists = DirectoryManager.Exists(strDirectoryPath);
            if (blnExists == true)
            {
                DirectoryInfo objDirectoryInfo = new DirectoryInfo(strDirectoryPath);
                objDirectoryInfo.Attributes = enuFileAttributes;
            }
        }

        public static FileAttributes GetAttributes(string strDirectoryPath)
        {
            return FileManager.GetAttributes(strDirectoryPath, FileAttributes.Normal);
        }

        public static FileAttributes GetAttributes(string strDirectoryPath, FileAttributes enuDefaultFileAttributes)
        {
            FileAttributes enuFileAttributes = enuDefaultFileAttributes;

            bool blnExists = FileManager.Exists(strDirectoryPath);
            if (blnExists == true)
            {
                DirectoryInfo objDirectoryInfo = new DirectoryInfo(strDirectoryPath);
                enuFileAttributes = objDirectoryInfo.Attributes;
            }

            return enuFileAttributes;
        }

        public static void Move(string strSourceDirectory, string strTargetDirectory, string strFilter)
        {
            Move(strSourceDirectory, strTargetDirectory, strFilter, false, false);
        }

        public static void Move(string strSourceDirectory, string strTargetDirectory, string strFilter, bool blnMoveOnlyIfNewer, bool blnBackupTarget)
        {
            bool blnSourceDirectoryExists = DirectoryManager.Exists(strSourceDirectory);
            if (blnSourceDirectoryExists == false)
            {
                throw new Exception("Source directory '" + strSourceDirectory + "' does not exist.");
            }
            DirectoryInfo objSourceDirectory = new DirectoryInfo(strSourceDirectory);

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

            FileInfo[] objFilesToMove = objSourceDirectory.GetFiles(strFilter);
            foreach (FileInfo objFileToMove in objFilesToMove)
            {
                FileManager.Move(objFileToMove.FullName, strTargetDirectory, blnMoveOnlyIfNewer, blnBackupTarget);
            }
        }

        public static void Copy(string strSourceDirectory, string strTargetDirectory, string strFilter)
        {
            Copy(strSourceDirectory, strTargetDirectory, strFilter, false, false);
        }

        public static void Copy(string strSourceDirectory, string strTargetDirectory, string strFilter, bool blnCopyOnlyIfNewer, bool blnBackupTarget)
        {
            bool blnSourceDirectoryExists = DirectoryManager.Exists(strSourceDirectory);
            if (blnSourceDirectoryExists == false)
            {
                throw new Exception("Source directory '" + strSourceDirectory + "' does not exist.");
            }
            DirectoryInfo objSourceDirectory = new DirectoryInfo(strSourceDirectory);

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

            FileInfo[] objFilesToCopy = objSourceDirectory.GetFiles(strFilter);
            foreach (FileInfo objFileToCopy in objFilesToCopy)
            {
                FileManager.Copy(objFileToCopy.FullName, strTargetDirectory, blnCopyOnlyIfNewer, blnBackupTarget);
            }
        }

        private static string CreateDeleteDirectoryName(string strDirectoryPath)
        {
            int intIndex = 0;
            string strRenameDirectoryPath = strDirectoryPath + FileManager.DeleteExtension;
            while (DirectoryManager.Exists(strRenameDirectoryPath) == true)
            {
                intIndex++;
                strRenameDirectoryPath = strDirectoryPath + "." + intIndex.ToString() + FileManager.DeleteExtension;
            }

            return strRenameDirectoryPath;
        }

        public static void RemoveDeletedItems(string strDirectoryPath, SearchOption enuDeleteScope)
        {
            bool blnDirectoryExists = DirectoryManager.Exists(strDirectoryPath);
            if (blnDirectoryExists == true)
            {
                string strSearchPattern = "*" + FileManager.DeleteExtension;

                try
                {
                    string[] strDeletedDirectories = Directory.GetDirectories(strDirectoryPath, strSearchPattern, enuDeleteScope);
                    foreach (string strDeletedDirectory in strDeletedDirectories)
                    {
                        try
                        {
                            DirectoryManager.Delete(strDeletedDirectory, false);
                        }
                        catch
                        {
                            /// Ignore any errors since we are trying to remove files that 
                            /// are marked for deletion.
                            /// 
                        }
                    }
                }
                catch
                {
                    /// Ignore any errors since we are trying to remove files that 
                    /// are marked for deletion.
                    /// 
                }

                try
                {
                    string[] strDeletedFiles = Directory.GetFiles(strDirectoryPath, strSearchPattern, SearchOption.TopDirectoryOnly);
                    foreach (string strDeletedFile in strDeletedFiles)
                    {
                        try
                        {
                            FileManager.Delete(strDeletedFile, false);
                        }
                        catch
                        {
                            /// Ignore any errors since we are trying to remove files that 
                            /// are marked for deletion.
                            /// 
                        }
                    }
                }
                catch
                {
                    /// Ignore any errors since we are trying to remove files that 
                    /// are marked for deletion.
                    /// 
                }
            }
        }

        #endregion
    }
}
