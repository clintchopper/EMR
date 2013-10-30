namespace ReLi.Framework.Library.Security
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Principal;
    using System.Collections.Generic;
    using System.Management;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Security.Hash;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Diagnostics;

    #endregion

    public sealed class ComputerToken : ObjectBase
    {
        private string _strHash;
        private string _strName;
        private string _strDescription;
        private string _strProductId;
        private string _strProcessorId;
        private string _strSerialNumber;
        private string _strVersionString;
        private string _strServicePack;
        private VersionToken _objVersion;
        private ComputerProcessorArchitectureType _enuArchitectureType;

        private ComputerToken()
            : base()
        {
            LoadInformation();
        }

        public ComputerToken(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public ComputerToken(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        public ComputerProcessorArchitectureType ArchitectureType
        {
            get
            {
                return _enuArchitectureType;
            }
            private set
            {
                _enuArchitectureType = value;
            }
        }

        public string Hash
        {
            get
            {
                return _strHash;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Hash", "A valid non-null string is required.");
                }

                _strHash = value;
            }
        }

        public string Name
        {
            get
            {
                return _strName;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Name", "A valid non-null string is required.");
                }

                _strName = value;
            }
        }

        public string Description
        {
            get
            {
                return _strDescription;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Description", "A valid non-null string is required.");
                }

                _strDescription = value;
            }
        }

        public string ProductId
        {
            get
            {
                return _strProductId;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ProductId", "A valid non-null string is required.");
                }

                _strProductId = value;
            }
        }

        public string ProcessorId
        {
            get
            {
                return _strProcessorId;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ProcessorId", "A valid non-null string is required.");
                }

                _strProcessorId = value;
            }
        }

        public string SerialNumber
        {
            get
            {
                return _strSerialNumber;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("SerialNumber", "A valid non-null string is required.");
                }

                _strSerialNumber = value;
            }
        }

        public string VersionString
        {
            get
            {
                return _strVersionString;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("VersionString", "A valid non-null string is required.");
                }

                _strVersionString = value;
            }
        }

        public string ServicePack
        {
            get
            {
                return _strServicePack;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ServicePack", "A valid non-null string is required.");
                }

                _strServicePack = value;
            }
        }

        public VersionToken Version
        {
            get
            {
                return _objVersion;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Version", "A valid non-null VersionToken is required.");
                }

                _objVersion = value;
            }
        }

        private void LoadInformation()
        {
            Version = new VersionToken(Environment.OSVersion.Version);
            VersionString = Environment.OSVersion.VersionString;
            ServicePack = Environment.OSVersion.ServicePack;
            Name = Environment.MachineName;
            Description = GetDescription();
            SerialNumber = GetSerialNumber();
            ProductId = GetProductId();
            ProcessorId = GetProcessorId();
            ArchitectureType = GetProcessArchitectureType();
            Hash = GetHash(ProductId, ProcessorId, SerialNumber);
        }

        private string GetProductId()
        {
            string strProductId = string.Empty;

            try
            {
                ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher("select SerialNumber from Win32_OperatingSystem");
                if (objManagementObjectSearcher != null)
                {
                    ManagementObjectCollection objManagementObjectCollection = objManagementObjectSearcher.Get();
                    foreach (ManagementObject objManagementObject in objManagementObjectCollection)
                    {
                        strProductId = Convert.ToString(objManagementObject["SerialNumber"]);
                        break;
                    }
                }
            }
            catch (Exception objException)
            {
                ApplicationManager.Logs.WriteMessage(new ErrorMessage(objException));
            }

            return strProductId;
        }

        private string GetProcessorId()
        {
            StringBuilder objProcessorId = new StringBuilder();

            try
            {
                ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher("select ProcessorId from Win32_Processor");
                if (objManagementObjectSearcher != null)
                {
                    ManagementObjectCollection objManagementObjectCollection = objManagementObjectSearcher.Get();
                    foreach (ManagementObject objManagementObject in objManagementObjectCollection)
                    {
                        objProcessorId.Append(Convert.ToString(objManagementObject["ProcessorId"]));
                        break;
                    }
                }
            }
            catch (Exception objException)
            {
                ApplicationManager.Logs.WriteMessage(new ErrorMessage(objException));
            }

            string strProcessorId = objProcessorId.ToString();
            return strProcessorId;
        }

        private string GetSerialNumber()
        {
            string strSerialNumber = string.Empty;

            try
            {
                string strSystemDriveLetter = Environment.SystemDirectory.Substring(0, 1);
                ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher("select VolumeSerialNumber from Win32_LogicalDisk where DeviceID = '" + strSystemDriveLetter + ":'");
                if (objManagementObjectSearcher != null)
                {
                    ManagementObjectCollection objManagementObjectCollection = objManagementObjectSearcher.Get();
                    foreach (ManagementObject objManagementObject in objManagementObjectCollection)
                    {
                        strSerialNumber = Convert.ToString(objManagementObject["VolumeSerialNumber"]);
                        break;
                    }
                }
            }
            catch (Exception objException)
            {
                ApplicationManager.Logs.WriteMessage(new ErrorMessage(objException));
            }

            return strSerialNumber;
        }

        private string GetDescription()
        {
            string strDescription = string.Empty;

            try
            {
                ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher("select Description from Win32_OperatingSystem");
                if (objManagementObjectSearcher != null)
                {
                    ManagementObjectCollection objManagementObjectCollection = objManagementObjectSearcher.Get();
                    foreach (ManagementObject objManagementObject in objManagementObjectCollection)
                    {
                        strDescription = Convert.ToString(objManagementObject["Description"]);
                        break;
                    }
                }
            }
            catch (Exception objException)
            {
                ApplicationManager.Logs.WriteMessage(new ErrorMessage(objException));
            }

            return strDescription;
        }

        private ComputerProcessorArchitectureType GetProcessArchitectureType()
        {
            string strArchitecture = string.Empty;

            try
            {
                ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher("select OSArchitecture from Win32_OperatingSystem");
                if (objManagementObjectSearcher != null)
                {
                    ManagementObjectCollection objManagementObjectCollection = objManagementObjectSearcher.Get();
                    foreach (ManagementObject objManagementObject in objManagementObjectCollection)
                    {
                        strArchitecture = Convert.ToString(objManagementObject["OSArchitecture"]);
                        break;
                    }
                }
            }
            catch
            {
                try
                {
                    strArchitecture = "32-bit";

                    /// Hack to check for the existence of syswow64
                    /// 
                    string strSysWow64Path = Path.Combine(Environment.GetEnvironmentVariable("windir"), "syswow64");
                    if (Directory.Exists(strSysWow64Path) == true)
                    {
                        strArchitecture = "64-bit";
                    }
                }
                catch (Exception objException)
                {
                    ApplicationManager.Logs.WriteMessage(new ErrorMessage(objException));
                }
            }

            ComputerProcessorArchitectureType enuComputerProcessorArchitectureType;
            switch (strArchitecture)
            {
                case ("32-bit"):
                    enuComputerProcessorArchitectureType = ComputerProcessorArchitectureType._32Bit;
                    break;
                case ("64-bit"):
                    enuComputerProcessorArchitectureType = ComputerProcessorArchitectureType._64Bit;
                    break;
                default:
                    enuComputerProcessorArchitectureType = ComputerProcessorArchitectureType.Unknown;
                    break;
            }

            return enuComputerProcessorArchitectureType;
        }

        private string GetHash(params string[] strValues)
        {
            StringBuilder objValue = new StringBuilder();
            foreach (string strValue in strValues)
            {
                objValue.Append(strValue);
            }

            string strHash = HashBase.Default.ComputeHash(objValue.ToString());
            return strHash;
        }

        public override string ToString()
        {
            StringBuilder objString = new StringBuilder();

            objString.AppendLine("Hash: " + Hash);
            objString.AppendLine("Name: " + Name);
            objString.AppendLine("Description: " + Name);
            objString.AppendLine("Product Id: " + ProductId);
            objString.AppendLine("Processor Id: " + ProcessorId);
            objString.AppendLine("Serial Number: " + SerialNumber);
            objString.AppendLine("Architecture Type: " + ArchitectureType.ToString());
            objString.AppendLine("Version: " + VersionString + " - " + Version.Number);
            objString.AppendLine("ServicePack: " + ServicePack);

            return objString.ToString();
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Hash", Hash);
            objSerializedObject.Values.Add("Name", Name);
            objSerializedObject.Values.Add("Description", Description);
            objSerializedObject.Values.Add("ProductId", ProductId);
            objSerializedObject.Values.Add("ProcessorId", ProcessorId);
            objSerializedObject.Values.Add("SerialNumber", SerialNumber);
            objSerializedObject.Values.Add("ArchitectureType", ArchitectureType);
            objSerializedObject.Values.Add("VersionString", VersionString);
            objSerializedObject.Values.Add("ServicePack", ServicePack);
            objSerializedObject.Objects.Add("Version", Version);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Hash = objSerializedObject.Values.GetValue<string>("Hash", string.Empty);
            Name = objSerializedObject.Values.GetValue<string>("Name", string.Empty);
            Description = objSerializedObject.Values.GetValue<string>("Description", string.Empty);
            ProductId = objSerializedObject.Values.GetValue<string>("ProductId", string.Empty);
            ProcessorId = objSerializedObject.Values.GetValue<string>("ProcessorId", string.Empty);
            SerialNumber = objSerializedObject.Values.GetValue<string>("SerialNumber", string.Empty);
            ArchitectureType = objSerializedObject.Values.GetValue<ComputerProcessorArchitectureType>("ArchitectureType", ComputerProcessorArchitectureType.Unknown);
            VersionString = objSerializedObject.Values.GetValue<string>("VersionString", string.Empty);
            ServicePack = objSerializedObject.Values.GetValue<string>("ServicePack", string.Empty);
            Version = objSerializedObject.Objects.GetObject<VersionToken>("Version", null);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Hash);
            objBinaryWriter.Write(Name);
            objBinaryWriter.Write(Description);
            objBinaryWriter.Write(ProductId);
            objBinaryWriter.Write(ProcessorId);
            objBinaryWriter.Write(SerialNumber);
            objBinaryWriter.Write((byte)ArchitectureType);
            objBinaryWriter.Write(VersionString);
            objBinaryWriter.Write(ServicePack);
            objBinaryWriter.WriteTransportableObject(Version);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Hash = objBinaryReader.ReadString();
            Name = objBinaryReader.ReadString();
            Description = objBinaryReader.ReadString();
            ProductId = objBinaryReader.ReadString();
            ProcessorId = objBinaryReader.ReadString();
            SerialNumber = objBinaryReader.ReadString();
            ArchitectureType = (ComputerProcessorArchitectureType)objBinaryReader.ReadByte();
            VersionString = objBinaryReader.ReadString();
            ServicePack = objBinaryReader.ReadString();
            Version = objBinaryReader.ReadTransportableObject<VersionToken>();
        }

        #endregion

        #region Static Members

        private static object _objSyncObject = new object();
        private static ComputerToken _objInstance;

        private static object SyncObject
        {
            get
            {
                return _objSyncObject;
            }
        }

        public static ComputerToken Instance
        {
            get
            {
                if (_objInstance == null)
                {
                    lock (SyncObject)
                    {
                        if (_objInstance == null)
                        {
                            _objInstance = new ComputerToken();
                        }
                    }
                }

                return _objInstance;
            }
        }

        #endregion
    }
}
