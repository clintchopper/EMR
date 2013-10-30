namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Reflection;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Configuration;
    using ReLi.Framework.Library.XmlLite;
    using ReLi.Framework.Library.Diagnostics;
    using ReLi.Framework.Library.Serialization;

    #endregion
    
    public sealed class ApplicationManager : ObjectBase
    {
        public const string FileName = "application.settings.xml";

        private LogManager _objLogs;
        private SettingsManager _objSettings;
        private string _strFilePath;

        private ApplicationManager()
        {
            _objLogs = new LogManager();
            _objSettings = new SettingsManager();        
        }

        public ApplicationManager(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public ApplicationManager(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}
        
        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            SerializedObjectCollection objObjects = objSerializedObject.Objects;

            objObjects.Add("Logs", _objLogs);
            objObjects.Add("Settings", _objSettings);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            SerializedObjectCollection objObjects = objSerializedObject.Objects;

            _objLogs = objObjects.GetObject<LogManager>("Logs", null);
            if (_objLogs == null)
            {
                _objLogs = new LogManager();
            }

            _objSettings = objObjects.GetObject<SettingsManager>("Settings", null);
            if (_objSettings == null)
            {
                _objSettings = new SettingsManager();
            }
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTransportableObject(ApplicationManager.Logs);
            objBinaryWriter.WriteTransportableObject(ApplicationManager.Settings);
            objBinaryWriter.Write(ApplicationManager.FilePath);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            ApplicationManager.Logs = objBinaryReader.ReadTransportableObject<LogManager>();
            ApplicationManager.Settings = objBinaryReader.ReadTransportableObject<SettingsManager>();
            ApplicationManager.FilePath = objBinaryReader.ReadString();
        }

        #endregion

        #region Static Members 

        private static object _syncObject = new object();
        private static ApplicationManager _objInstance;
        private static string _strApplicationDirectory;

        private static object SyncObject
        {
            get
            {
                return _syncObject;
            }
        }

        private static ApplicationManager Instance
        {
            get
            {
                if (_objInstance == null)
                {
                    lock (SyncObject)
                    {
                        if (_objInstance == null)
                        {
                            _objInstance = ApplicationManager.Load();
                        }
                    }
                }

                return _objInstance;
            }
            set
            {
                _objInstance = value;
            }
        }

        private static string GetApplicationDirectory()
        {
            string strApplicationDirectory = string.Empty;
            if (System.Web.HttpContext.Current != null)
            {
                strApplicationDirectory = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(""), "bin");
                if (Directory.Exists(strApplicationDirectory) == false)
                {
                    strApplicationDirectory = System.Web.HttpContext.Current.Server.MapPath("");
                }
            }
            else
            {
                strApplicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }

            return strApplicationDirectory;
        }

        private static string FindSettingsFile()
        {
            string strSettingsFilePath = string.Empty;
            string[] strSearchDirectories = new string[] { "", "bin", "config" };

            string strRootDirectory = string.Empty;
            if (System.Web.HttpContext.Current != null)
            {
                strRootDirectory = System.Web.HttpContext.Current.Server.MapPath("");
            }
            else
            {
                strRootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            
            bool blnFileExists = false;
            string strConfigFilePath = string.Empty;
            string strConfigDirectory = string.Empty;

            foreach (string strSearchDirectory in strSearchDirectories)
            {
                strConfigDirectory = Path.Combine(strRootDirectory, strSearchDirectory);
                strConfigFilePath = Path.Combine(strConfigDirectory, ApplicationManager.FileName);

                blnFileExists = File.Exists(strConfigFilePath);
                if (blnFileExists == true)
                {
                    strSettingsFilePath = strConfigFilePath;
                    break;
                }
            }

            return strSettingsFilePath;
        }

        private static ApplicationManager Load()
        {
            string strSettingsFilePath = FindSettingsFile();

            ApplicationManager objApplicationManager = null;
            if (strSettingsFilePath.Length == 0)
            {
                objApplicationManager = new ApplicationManager();
            }
            else
            {
                objApplicationManager = ObjectBase.DeserializeFromFile<ApplicationManager>(FormatterManager.XmlFormatter, strSettingsFilePath);
            }

            return objApplicationManager;
        }

        public static string CompanyName
        {
            get
            {
                return Application.CompanyName;
            }
        }

        public static string ProductName
        {
            get
            {
                return Application.ProductName;
            }
        }

        public static string ProductVersion
        {
            get
            {
                return Application.ProductVersion;
            }
        }

        public static LogManager Logs
        {
            get
            {
                return Instance._objLogs;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Logs", "A valid non-null LogManager is required.");
                }

                Instance._objLogs = value;
            }
        }

        public static SettingsManager Settings
        {
            get
            {
                return Instance._objSettings;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Settings", "A valid non-null SettingsManager is required.");
                }

                Instance._objSettings = value;
            }
        }

        public static string ApplicationDirectory
        {
            get
            {
                if (_strApplicationDirectory == null)
                {
                    lock (SyncObject)
                    {
                        _strApplicationDirectory = GetApplicationDirectory();
                    }
                }

                return _strApplicationDirectory;
            }
            private set
            {
                _strApplicationDirectory = value;
            }
        }

        public static string FilePath
        {
            get
            {
                if (Instance._strFilePath == null)
                {
                    lock (SyncObject)
                    {
                        Instance._strFilePath = Path.Combine(ApplicationDirectory, FileName);
                    }
                }

                return Instance._strFilePath;
            }
            private set
            {
                Instance._strFilePath = value;
            }
        }

        public static void Save()
        {
            string strFilePath = Path.Combine(ApplicationDirectory, FileName);
            Instance.SerializeToFile(FormatterManager.XmlFormatter, strFilePath);
        }

        public static void Cleanup()
        {
            AsyncHelper.Wait();
        }

        #endregion
    }
}
