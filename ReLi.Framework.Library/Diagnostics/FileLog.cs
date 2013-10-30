namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Reflection;
    using System.Security.Principal;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class FileLog : LogBase
    {
        #region Constants Declarations

        public const string DefaultFileName = "[USER]_[DATE][TIME].txt";
        public const string DefaultDirectory = "[WORKING_DIRECTORY]\\Logs";

        #endregion

        private object _objSyncObject;
        private string _strFileName;
        private string _strFormattedFileName;
        private string _strDirectory;
        private string _strFormattedDirectory;
        [NonSerialized]
        private FileStream _objFileStream;
        [NonSerialized]
        private TextWriter _objTextWriter;

        public FileLog()
            : this(new MessageTypeCollection())
        {}

        public FileLog(MessageTypeCollection objMessageTypes)
            : this(objMessageTypes, DefaultDirectory, DefaultFileName)
        {}
        
        public FileLog(MessageTypeCollection objMessageTypes, string strDirectory, string strFileName)
            : base(objMessageTypes)
        {
            FileName = strFileName;
            Directory = strDirectory;
            FileStream = null;
            TextWriter = null;
        }

        public FileLog(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public FileLog(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}
        
        public string FileName
        {
            get
            {
                return _strFileName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FileName", "A valid non-null string is required.");
                }

                _strFileName = value;
                _strFormattedFileName = ReplaceVariables(_strFileName);
            }
        }

        public string FormattedFileName
        {
            get
            {
                return _strFormattedFileName;
            }
        }

        public string FilePath
        {
            get
            {
                return Path.Combine(FormattedDirectory, FormattedFileName);
            }            
        }

        public string Directory
        {
            get
            {
                return _strDirectory;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Directory", "A valid non-null string is required.");
                }

                _strDirectory = value;
                _strFormattedDirectory = ReplaceVariables(_strDirectory);
            }
        }

        public string FormattedDirectory
        {
            get
            {
                return _strFormattedDirectory;
            }
        }

        private FileStream FileStream
        {
            get
            {
                return _objFileStream;
            }
            set
            {
                _objFileStream = value;
            }
        }

        private TextWriter TextWriter
        {
            get
            {
                return _objTextWriter;
            }
            set
            {
                _objTextWriter = value;
            }
        }

        private object SyncObject
        {
            get
            {
                if (_objSyncObject == null)
                {
                    _objSyncObject = new object();
                }

                return _objSyncObject;
            }
        }

        private string ReplaceVariables(string strValue)
        {
            string strFormattedValue = strValue;

            strFormattedValue = strFormattedValue.Replace("[DATE]", DateTime.Now.ToString("MMddyyyy"));
            strFormattedValue = strFormattedValue.Replace("[TIME]", DateTime.Now.ToString("hhmmss"));
            strFormattedValue = strFormattedValue.Replace("[MACHINE]", Environment.MachineName);
            strFormattedValue = strFormattedValue.Replace("[WORKING_DIRECTORY]", ApplicationManager.ApplicationDirectory);
            strFormattedValue = strFormattedValue.Replace("[USER]", WindowsIdentity.GetCurrent().Name.Replace("\\", "_"));

            return strFormattedValue;

        }

        protected override void OnWriteMessage(MessageBase objMessage)
        {
            lock (SyncObject)
            {
                if (FileStream == null)
                {
                    if (System.IO.Directory.Exists(FormattedDirectory) == false)
                    {
                        System.IO.Directory.CreateDirectory(FormattedDirectory);
                    }

                    FileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, System.IO.FileShare.ReadWrite);
                    TextWriter = new StreamWriter(FileStream);
                }

                TextWriter.WriteLine(objMessage.ToString());
                TextWriter.Flush();
            }
        }

        #region IDisposable Members

        protected override void Dispose(bool blnDisposing)
        {
            if (blnDisposing == true)
            {
                if (TextWriter != null)
                {
                    TextWriter.Close();
                    TextWriter.Dispose();
                    TextWriter = null;
                }
                if (FileStream != null)
                {
                    FileStream.Close();
                    FileStream.Dispose();
                    FileStream = null;
                }
            }
        }

        #endregion

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            SerializedValueCollection objValues = objSerializedObject.Values;

            objValues.Add("FileName", FileName);
            objValues.Add("Directory", Directory);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            SerializedValueCollection objValues = objSerializedObject.Values;

            FileName =  (string)objValues.GetValue("FileName", DefaultFileName);
            Directory = (string)objValues.GetValue("Directory", DefaultDirectory);

            _objSyncObject = null;
            _objFileStream = null;
            _objTextWriter = null;
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(FileName);
            objBinaryWriter.Write(Directory);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            FileName = objBinaryReader.ReadString();
            Directory = objBinaryReader.ReadString();

            _objSyncObject = null;
            _objFileStream = null;
            _objTextWriter = null;
        }

        #endregion

    }
}
