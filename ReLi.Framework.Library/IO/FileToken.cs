namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Diagnostics;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Security.Hash;
    using ReLi.Framework.Library.Security.Encryption;


    #endregion
        
	public class FileToken : ObjectBase
    {
        private string _strName;
        private string _strFullPath;
        private string _strRelativePath;
        private string _strHash;
        private long _lngSize;
        private VersionToken _objVersion;

        public FileToken(string strFullPath, string strRelativePath, string strHash, long lngSize, VersionToken objVersion)
            : base()
        {
            FullPath = strFullPath;
            RelativePath = strRelativePath;
            Hash = strHash;
            Version = objVersion;
            Size = lngSize;
        }

        public FileToken(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public FileToken(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string Name
        {
            get
            {
                if (_strName == null)
                {
                    _strName = Path.GetFileName(FullPath);
                }

                return _strName;
            }
        }

        public string FullPath
        {
            get
            {
                return _strFullPath;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FullPath", "A valid non-null string is required.");
                }

                _strFullPath = value;
            }
        }

        public string RelativePath
        {
            get
            {
                return _strRelativePath;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("RelativePath", "A valid non-null string is required.");
                }

                _strRelativePath = value;
            }
        }

        public string Hash
        {
            get
            {
                return _strHash;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Hash", "A valid non-null string is required.");
                }

                _strHash = value;
            }
        }

        public long Size
        {
            get
            {
                return _lngSize;
            }
            set
            {
                _lngSize = value;
            }
        }

        public VersionToken Version
        {
            get
            {
                return _objVersion;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Version", "A valid non-null VersionToken is required.");
                }

                _objVersion = value;
            }
        }

        public bool Equals(FileToken objFileToken)
        {
            if (objFileToken == null)
            {
                return false;
            }

            return ((RelativePath == objFileToken.RelativePath) && (Hash == objFileToken.Hash));
        }

        public override bool Equals(object objValue)
        {
            if (objValue == null)
            {
                return false;
            }

            FileToken objFileToken = objValue as FileToken;
            return Equals(objFileToken);
        }

        public override int GetHashCode()
        {
            return RelativePath.GetHashCode() ^ Hash.GetHashCode();
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("FullPath", FullPath);
            objSerializedObject.Values.Add("RelativePath", RelativePath);
            objSerializedObject.Values.Add("Hash", Hash);
            objSerializedObject.Values.Add("Size", Size);
            objSerializedObject.Values.Add("VersionNumber", Version.Number);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            FullPath = objSerializedObject.Values.GetValue<string>("FullPath", string.Empty);
            RelativePath = objSerializedObject.Values.GetValue<string>("RelativePath", string.Empty);
            Hash = objSerializedObject.Values.GetValue<string>("Hash", string.Empty);
            Size = objSerializedObject.Values.GetValue<long>("Size", 0);

            string strVersionNumber = objSerializedObject.Values.GetValue<string>("VersionNumber", string.Empty);
            Version = new VersionToken(strVersionNumber);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(FullPath);
            objBinaryWriter.Write(RelativePath);
            objBinaryWriter.Write(Hash);
            objBinaryWriter.Write(Size);
            objBinaryWriter.WriteTransportableObject(Version);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            FullPath = objBinaryReader.ReadString();
            RelativePath = objBinaryReader.ReadString();
            Hash = objBinaryReader.ReadString();
            Size = objBinaryReader.ReadInt64();
            Version = objBinaryReader.ReadTransportableObject<VersionToken>();
        }

        #endregion

        #region Static Members

        public static FileToken Load(string strFilePath)
        {
            return FileToken.Load(strFilePath, string.Empty);
        }

        public static FileToken Load(string strFilePath, string strRelativePath)
        {
            if (strFilePath == null)
            {
                throw new ArgumentNullException("strFilePath", "A valid non-null string is required.");
            }
            if (FileManager.Exists(strFilePath) == false)
            {
                throw new FileNotFoundException("Unable to generate FileToken because the file does not exist.", strFilePath);
            }

            FileToken objFileToken = null;

            if (FileManager.Exists(strFilePath) == true)
            {
                string strHash = string.Empty;

                FileInfo objFileInfo = new FileInfo(strFilePath);
                try
                {
                    using (Stream objFileSteam = objFileInfo.OpenRead())
                    {
                        HashBase.Default.ComputeHash(objFileSteam, out strHash);
                    }

                    FileVersionInfo objFileVersionInfo = FileVersionInfo.GetVersionInfo(strFilePath);
                    VersionToken objVersionToken = new VersionToken(objFileVersionInfo.FileBuildPart, objFileVersionInfo.FileMajorPart, objFileVersionInfo.FileMinorPart, 0);
                                        
                    string strRelativeFileName = Path.Combine(strRelativePath, objFileInfo.Name);
                    objFileToken = new FileToken(strFilePath, strRelativeFileName, strHash, objFileInfo.Length, objVersionToken);
                }
                catch 
                {}
            }

            return objFileToken;
        }

        #endregion
    }
}
