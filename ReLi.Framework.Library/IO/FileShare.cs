namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Collections.Specialized;    
    using System.Text;
    using System.Management;
    using ReLi.Framework.Library.IO; 
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class FileShare : ObjectBase 
    {
        protected string _strName;
        protected string _strPath;
        protected string _strDescription;

        public FileShare(string strName, string strPath, string strDescription)
            : base()
        {
            Name = strName;
            Path = strPath;
            Description = strDescription;
        }

        public FileShare(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public FileShare(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string Name
        {
            get
            {
                return _strName;
            }
            private set
            {
                if ((value == null) || (value.Length == 0))
                {
                    throw new ArgumentException("A valid non-null, non-empty string is expected", "Name");
                }

                _strName = value;
            }
        }

        public string Path
        {
            get
            {
                return _strPath;
            }
            private set
            {
                if ((value == null) || (value.Length == 0))
                {
                    throw new ArgumentException("A valid non-null, non-empty string is expected", "Path");
                }

                _strPath = value;
            }
        }

        public string Description
        {
            get
            {
                return _strDescription;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Description", "A valid non-null string is required.");
                }

                _strDescription = value;
            }
        }

        public void Save()
        {
            FileShare.Save(this);
        }

        public void Delete()
        {
            FileShare.Delete(Name);
        }

        public bool Exists()
        {
            return FileShare.Exists(Name);
        }

        public void Refresh()
        {
            FileShare objFileShare = FileShare.Load(Name);
            if (objFileShare != null)
            {
                objFileShare.CopyTo(this);
            }
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Name", Name);
            objSerializedObject.Values.Add("Path", Path);
            objSerializedObject.Values.Add("Description", Description);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Name = objSerializedObject.Values.GetValue<string>("Name", string.Empty);
            Path = objSerializedObject.Values.GetValue<string>("Path", string.Empty);
            Description = objSerializedObject.Values.GetValue<string>("Description", string.Empty);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Name);
            objBinaryWriter.Write(Path);
            objBinaryWriter.Write(Description);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Name = objBinaryReader.ReadString();
            Path = objBinaryReader.ReadString();
            Description = objBinaryReader.ReadString();
        }

        #endregion

        #region Static Members

        public static void Save(FileShare objFileShare)
        {
            if (objFileShare == null)
            {
                throw new ArgumentNullException("objFileShare", "A valid non-null FileShare is expected");
            }

            try
            {
                string strScope = @"\\" + Environment.MachineName + @"\root\cimv2";
                ManagementScope objManagementScope = new ManagementScope(strScope);

                bool blnExists = Exists(objFileShare.Name);
                if (blnExists == false)
                {
                    ManagementClass objManagementClass = new ManagementClass("Win32_Share");
                    objManagementClass.Scope = objManagementScope;

                    string strFormattedSharePath = ((objFileShare.Path.EndsWith("\\") == true) ? objFileShare.Path.Substring(0, objFileShare.Path.Length - 1) : objFileShare.Path);

                    /// 0 indicates a file share.
                    /// 
                    object[] objArguments = new object[] { strFormattedSharePath, objFileShare.Name, "0", null, objFileShare.Description};
                    object objResult = objManagementClass.InvokeMethod("Create", objArguments);

                    Exception objException = FileShare.ProcessResult(objResult);
                    if (objException != null)
                    {
                        throw objException;
                    }
                }
                else
                {
                    string strQuery = String.Format("select * from win32_share where name = '{0}'", objFileShare.Name);
                    ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher(strQuery);
                    if (objManagementObjectSearcher != null)
                    {
                        ManagementObjectCollection objManagementObjectCollection = objManagementObjectSearcher.Get();
                        foreach (ManagementObject objManagementObject in objManagementObjectCollection)
                        {
                            object[] objArguments = new object[] { null, objFileShare.Description, null };
                            object objResult = objManagementObject.InvokeMethod("SetShareInfo", objArguments);

                            Exception objException = FileShare.ProcessResult(objResult);
                            if (objException != null)
                            {
                                throw objException;
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception objException)
            {
                string strErrorMessage = "An error was encountered while trying to create share '" + objFileShare.Name + "' for '" + objFileShare.Path + "':\r\n";
                strErrorMessage += objException.ToString();
                throw new Exception(strErrorMessage);
            }
        }
                        
        public static bool Exists(string strName)
        {
            bool blnExists = false;

            string strQuery = String.Format("select * from win32_share where name = '{0}'", strName);
            ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher(strQuery);
            if (objManagementObjectSearcher != null)
            {
                ManagementObjectCollection objManagementObjectCollection = objManagementObjectSearcher.Get();
                blnExists = (objManagementObjectCollection.Count > 0);
            }

            return blnExists;
        }

        public static void Delete(string strName)
        {
            string strQuery = String.Format("select * from win32_share where name = '{0}'", strName);
            ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher(strQuery);
            if (objManagementObjectSearcher != null)
            {
                ManagementObjectCollection objManagementObjectCollection = objManagementObjectSearcher.Get();
                foreach (ManagementObject objManagementObject in objManagementObjectCollection)
                {
                    object[] objArguments = new object[] { };
                    object objResult = objManagementObject.InvokeMethod("Delete", objArguments);

                    Exception objException = FileShare.ProcessResult(objResult);
                    if (objException != null)
                    {
                        throw objException;
                    }
                }
            }
        }

        public static FileShareList Load()
        {
            List<FileShare> objFileShares = new List<FileShare>();

            string strQuery = String.Format("select * from win32_share where type = 0");
            ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher(strQuery);
            if (objManagementObjectSearcher != null)
            {
                ManagementObjectCollection objManagementObjectCollection = objManagementObjectSearcher.Get();
                foreach (ManagementObject objManagementObject in objManagementObjectCollection)
                {
                    string strName = ((objManagementObject["Name"] != null) ? objManagementObject["Name"].ToString() : string.Empty);
                    string strPath = ((objManagementObject["Path"] != null) ? objManagementObject["Path"].ToString() : string.Empty);
                    string strDescription = ((objManagementObject["Description"] != null) ? objManagementObject["Description"].ToString() : string.Empty);

                    FileShare objFileShare = new FileShare(strName, strPath, strDescription);
                    objFileShares.Add(objFileShare);
                }
            }

            FileShareList objFileShareList = new FileShareList(objFileShares);
            return objFileShareList;
        }

        public static FileShare Load(string strName)
        {
            FileShare objFileShare = null;

            string strQuery = String.Format("select * from win32_share where name = '{0}'", strName);
            ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher(strQuery);
            if (objManagementObjectSearcher != null)
            {
                ManagementObjectCollection objManagementObjectCollection = objManagementObjectSearcher.Get();
                foreach (ManagementObject objManagementObject in objManagementObjectCollection)
                {
                    string strPath = ((objManagementObject["Path"] != null) ? objManagementObject["Path"].ToString() : string.Empty);
                    string strDescription = ((objManagementObject["Description"] != null) ? objManagementObject["Description"].ToString() : string.Empty);

                    objFileShare = new FileShare(strName, strPath, strDescription);
                    break;
                }
            }

            return objFileShare;
        }

        private static Exception ProcessResult(object objResult)
        {
            Exception objException = null;

            switch (Convert.ToInt32(objResult))
            {
                case (2):
                    objException =  new Exception("Access Denied");
                    break;

                case (8):
                    objException =  new Exception("Unknown Failure");
                    break;

                case (9):
                    objException =  new Exception("Invalid Name");
                    break;

                case (10):
                    objException =  new Exception("Invalid Level");
                    break;

                case (21):
                    objException =  new Exception("Invalid Parameter");
                    break;

                case (22):
                    objException =  new Exception("Duplicate Share");
                    break;

                case (23):
                    objException =  new Exception("Redirected Path");
                    break;

                case (24):
                    objException =  new Exception("Unknown Device or Directory");
                    break;

                case (25):
                    objException =  new Exception("Net Name Not Found");
                    break;
            }

            return objException;

        }

        #endregion
    }
}
