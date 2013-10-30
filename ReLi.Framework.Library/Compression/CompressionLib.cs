namespace ReLi.Framework.Library.Compression
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Text;

    #endregion

    public enum CompressedFileRecordState
    {
        Deleted = 1,
        Inserted = 2,
        Updated = 4
    }

    public class CompressedFileRecord
    {
        private static string PropertyDelimeter = BitConverter.ToString(new byte[] { 198, 234, 146, 221, 204 });
        private static string PropertyValueDelimeter = BitConverter.ToString(new byte[] { 199, 14, 23, 218, 204 });
        private static string KeyValueDelimeter = BitConverter.ToString(new byte[] { 174, 232, 246, 216, 204 });

        private int _intSize;
        private int _intCompressedSize;
        private DateTime _dtCreatedDate;
        private string _strFileName;
        private string _strRelativePath;
        private string _strOriginalFilePath;
        private string _strVersion;
        private List<string> _objKeys;

        public CompressedFileRecord(string strStringValue)
        {
            Dictionary<string, string> objProperties = new Dictionary<string, string>();

            string[] strProperties = strStringValue.Split(new string[] { CompressedFileRecord.PropertyDelimeter }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string strProperty in strProperties)
            {
                string[] strPropertyParts = strProperty.Split(new string[] { CompressedFileRecord.PropertyValueDelimeter }, StringSplitOptions.None);
                if (strPropertyParts.Length != 2)
                {
                    throw new Exception("Unable to retrieve property information from '" + strProperty + "'.");
                }

                objProperties.Add(strPropertyParts[0], strPropertyParts[1]);
            }

            int intSize = ((objProperties.ContainsKey("Size") == true) ? Convert.ToInt32(objProperties["Size"]) : -1);
            if (intSize == -1)
            {
                throw new Exception("The 'Size' property value is missing from '" + strStringValue + "'.");
            }
            Size = intSize;

            int intCompressedSize = ((objProperties.ContainsKey("CompressedSize") == true) ? Convert.ToInt32(objProperties["CompressedSize"]) : -1);
            if (intCompressedSize == -1)
            {
                throw new Exception("The 'CompressedSize' property value is missing from '" + strStringValue + "'.");
            }
            CompressedSize = intCompressedSize;

            DateTime dtCreatedDate = ((objProperties.ContainsKey("CreatedDate") == true) ? Convert.ToDateTime(objProperties["CreatedDate"]) : DateTime.MinValue);
            if (dtCreatedDate == DateTime.MinValue)
            {
                throw new Exception("The 'CreatedDate' property value is missing from '" + strStringValue + "'.");
            }
            CreatedDate = dtCreatedDate;

            string strFileName = ((objProperties.ContainsKey("FileName") == true) ? objProperties["FileName"] : null);
            if (strFileName == null)
            {
                throw new Exception("The 'Path' property value is missing from '" + strStringValue + "'.");
            }
            FileName = strFileName;

            string strRelativePath = ((objProperties.ContainsKey("RelativePath") == true) ? objProperties["RelativePath"] : null);
            if (strRelativePath == null)
            {
                throw new Exception("The 'RelativePath' property value is missing from '" + strStringValue + "'.");
            }
            RelativePath = strRelativePath;

            string strOriginalFilePath = ((objProperties.ContainsKey("OriginalFilePath") == true) ? objProperties["OriginalFilePath"] : null);
            if (strOriginalFilePath == null)
            {
                throw new Exception("The 'OriginalFilePath' property value is missing from '" + strStringValue + "'.");
            }
            OriginalFilePath = strOriginalFilePath;

            string strVersion = ((objProperties.ContainsKey("Version") == true) ? objProperties["Version"] : null);
            if (strVersion == null)
            {
                throw new Exception("The 'Version' property value is missing from '" + strStringValue + "'.");
            }
            Version = strVersion;

            string strKeys = ((objProperties.ContainsKey("Keys") == true) ? objProperties["Keys"] : null);
            if (strKeys == null)
            {
                throw new Exception("The 'Keys' property value is missing from '" + strStringValue + "'.");
            }

            string[] strKeyValues = strKeys.Split(new string[] { KeyValueDelimeter }, StringSplitOptions.RemoveEmptyEntries);
            Keys = new List<string>(strKeyValues);
        }

        public CompressedFileRecord(string strFilePath, int intCompressedSize)
            : this(strFilePath, intCompressedSize, string.Empty)
        { }

        public CompressedFileRecord(string strFilePath, int intCompressedSize, string strRelativePath)
        {
            if ((strFilePath == null) || (strFilePath.Length == 0))
            {
                throw new ArgumentOutOfRangeException("A valid non-null, non-empty string is required.");
            }
            if (File.Exists(strFilePath) == false)
            {
                throw new FileNotFoundException("The file could not be found.", strFilePath);
            }

            FileInfo objFileInfo = new FileInfo(strFilePath);
            CompressedSize = intCompressedSize;
            CreatedDate = objFileInfo.CreationTime;
            FileName = Path.GetFileName(strFilePath);
            Keys = new List<string>();
            OriginalFilePath = strFilePath;
            RelativePath = strRelativePath;
            Size = (int)objFileInfo.Length;
            Version = FileVersionInfo.GetVersionInfo(strFilePath).FileVersion;
        }

        public int CompressedSize
        {
            get
            {
                return _intCompressedSize;
            }
            private set
            {
                _intCompressedSize = value;
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return _dtCreatedDate;
            }
            private set
            {
                if (value == DateTime.MinValue)
                {
                    throw new Exception("The date must be greater the DateTime.MinValue");
                }

                _dtCreatedDate = value;
            }
        }

        public string FileName
        {
            get
            {
                return _strFileName;
            }
            set
            {
                if ((value == null) || (value.Length == 0))
                {
                    throw new ArgumentOutOfRangeException("FileName", "A valid non-null, non-empty string is required.");
                }
                if ((value.Length > 260) || (value.IndexOfAny(Path.GetInvalidFileNameChars()) != -1))
                {
                    throw new Exception("'" + value + "' does not represent a valid filename");
                }

                _strFileName = value;
            }
        }

        public List<string> Keys
        {
            get
            {
                return _objKeys;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Keys", "A valid non-null List<string> is required.");
                }

                _objKeys = value;
            }
        }

        public string OriginalFilePath
        {
            get
            {
                return _strOriginalFilePath;
            }
            private set
            {
                if ((value == null) || (value.Length == 0))
                {
                    throw new ArgumentOutOfRangeException("OriginalFilePath", "A valid non-null, non-empty string is required.");
                }

                _strOriginalFilePath = value;
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
                if ((value.Length > 248) || (value.IndexOfAny(Path.GetInvalidPathChars()) != -1))
                {
                    throw new Exception("'" + value + "' does not represent a valid path");
                }

                _strRelativePath = value;
            }
        }

        public string RelativeFilePath
        {
            get
            {
                string strRelativeFilePath = FileName;
                if (RelativePath.Length > 0)
                {
                    strRelativeFilePath = RelativePath;
                }

                return strRelativeFilePath;
            }
        }

        public int Size
        {
            get
            {
                return _intSize;
            }
            private set
            {
                _intSize = value;
            }
        }

        public string Version
        {
            get
            {
                return _strVersion;
            }
            private set
            {
                if (value == null)
                {
                    _strVersion = string.Empty;
                }
                else
                {
                    _strVersion = value;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder objValue = new StringBuilder();

            objValue.Append("CompressedSize" + PropertyValueDelimeter + CompressedSize.ToString());
            objValue.Append(PropertyDelimeter);
            objValue.Append("CreatedDate" + PropertyValueDelimeter + CreatedDate.ToString());
            objValue.Append(PropertyDelimeter);
            objValue.Append("FileName" + PropertyValueDelimeter + FileName);
            objValue.Append(PropertyDelimeter);
            objValue.Append("OriginalFilePath" + PropertyValueDelimeter + OriginalFilePath);
            objValue.Append(PropertyDelimeter);
            objValue.Append("RelativePath" + PropertyValueDelimeter + RelativePath);
            objValue.Append(PropertyDelimeter);
            objValue.Append("Size" + PropertyValueDelimeter + Size.ToString());
            objValue.Append(PropertyDelimeter);
            objValue.Append("Version" + PropertyValueDelimeter + Version);
            objValue.Append(PropertyDelimeter);

            StringBuilder objKeyValues = new StringBuilder();
            foreach (string strKey in Keys)
            {
                objKeyValues.Append(strKey);
                objKeyValues.Append(KeyValueDelimeter);
            }
            objValue.Append("Keys" + PropertyValueDelimeter + objKeyValues.ToString());

            return objValue.ToString();
        }
    }

    public class CompressedFileRecordManager
    {
        private static string RecordDelimeter = BitConverter.ToString(new byte[] { 32, 12, 3, 234, 204 });

        private int _intFileDataStartingIndex;
        private Dictionary<CompressedFileRecord, CompressedFileRecordState> _objSortedRecords;

        public CompressedFileRecordManager(int intFileDataStartingIndex)
            : this(intFileDataStartingIndex, new List<CompressedFileRecord>(), CompressedFileRecordState.Updated)
        { }

        public CompressedFileRecordManager(int intFileDataStartingIndex, IEnumerable<CompressedFileRecord> objRecords, CompressedFileRecordState enuInitialState)
        {
            FileDataStartingIndex = intFileDataStartingIndex;

            SortedRecords = new Dictionary<CompressedFileRecord, CompressedFileRecordState>();
            foreach (CompressedFileRecord objRecord in objRecords)
            {
                SortedRecords.Add(objRecord, enuInitialState);
            }
        }

        private Dictionary<CompressedFileRecord, CompressedFileRecordState> SortedRecords
        {
            get
            {
                return _objSortedRecords;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Records", "A valid non-null Dictionary<CompressedFileRecord, CompressedFileRecordState> is required.");
                }

                _objSortedRecords = value;
            }
        }

        public int FileDataStartingIndex
        {
            get
            {
                return _intFileDataStartingIndex;
            }
            private set
            {
                _intFileDataStartingIndex = value;
            }
        }

        public bool Exists(string strRelativeFilePath)
        {
            bool blnExists = false;

            if (strRelativeFilePath != null)
            {
                foreach (CompressedFileRecord objRecord in ActiveRecords)
                {
                    if (objRecord.RelativeFilePath == strRelativeFilePath)
                    {
                        blnExists = true;
                        break;
                    }
                }
            }

            return blnExists;
        }

        public bool Exists(CompressedFileRecord objRecord)
        {
            bool blnExists = false;

            if (objRecord != null)
            {
                blnExists = SortedRecords.ContainsKey(objRecord);
            }

            return blnExists;
        }

        public void Insert(CompressedFileRecord objRecord)
        {
            if (objRecord != null)
            {
                if (Exists(objRecord.RelativeFilePath) == true)
                {
                    throw new Exception("A record already exists for the RelatveFilePath '" + objRecord.RelativeFilePath + "'.");
                }

                SortedRecords[objRecord] = CompressedFileRecordState.Inserted;
            }
        }

        public void Insert(IEnumerable<CompressedFileRecord> objRecords)
        {
            if (objRecords != null)
            {
                foreach (CompressedFileRecord objRecord in objRecords)
                {
                    Insert(objRecord);
                }
            }
        }

        public void Delete(CompressedFileRecord objRecord)
        {
            if (objRecord != null)
            {
                if (SortedRecords.ContainsKey(objRecord) == true)
                {
                    SortedRecords[objRecord] = CompressedFileRecordState.Deleted;
                }
            }
        }

        public void Clear()
        {
            foreach (CompressedFileRecord objRecord in SortedRecords.Keys)
            {
                SortedRecords[objRecord] = CompressedFileRecordState.Deleted;
            }
        }

        public CompressedFileRecord this[string strRelativeFilePath]
        {
            get
            {
                CompressedFileRecord objCompressedFileRecord = null;

                if (strRelativeFilePath != null)
                {
                    foreach (CompressedFileRecord objRecord in SortedRecords.Keys)
                    {
                        if (objRecord.RelativeFilePath == strRelativeFilePath)
                        {
                            objCompressedFileRecord = objRecord;
                            break;
                        }
                    }
                }

                return objCompressedFileRecord;
            }
        }

        public CompressedFileRecord[] AllRecords
        {
            get
            {
                return GetRecordsByState(CompressedFileRecordState.Inserted | CompressedFileRecordState.Updated | CompressedFileRecordState.Deleted);
            }
        }

        public CompressedFileRecord[] ActiveRecords
        {
            get
            {
                return GetRecordsByState(CompressedFileRecordState.Inserted | CompressedFileRecordState.Updated);
            }
        }

        public CompressedFileRecord[] InsertedRecords
        {
            get
            {
                return GetRecordsByState(CompressedFileRecordState.Inserted);
            }
        }

        public CompressedFileRecord[] UpdatedRecords
        {
            get
            {
                return GetRecordsByState(CompressedFileRecordState.Updated);
            }
        }

        public CompressedFileRecord[] DeletedRecords
        {
            get
            {
                return GetRecordsByState(CompressedFileRecordState.Deleted);
            }
        }

        public CompressedFileRecord[] GetRecordsByState(CompressedFileRecordState enuState)
        {
            List<CompressedFileRecord> objRecords = new List<CompressedFileRecord>();
            foreach (KeyValuePair<CompressedFileRecord, CompressedFileRecordState> objKeyValuePair in SortedRecords)
            {
                if ((enuState & objKeyValuePair.Value) == objKeyValuePair.Value)
                {
                    objRecords.Add(objKeyValuePair.Key);
                }
            }

            return objRecords.ToArray();
        }

        public CompressedFileRecordState GetRecordState(CompressedFileRecord objRecord)
        {
            if (objRecord == null)
            {
                throw new ArgumentNullException("objRecord", "A valid non-null CompressedFileRecord is required.");
            }
            if (Exists(objRecord) == false)
            {
                throw new Exception("A record for '" + objRecord.RelativePath + "' could not be found.");
            }

            return SortedRecords[objRecord];
        }

        public CompressedFileRecord GetRecordByKey(string strKey)
        {
            CompressedFileRecord objRecord = null;

            if (strKey != null)
            {
                foreach (CompressedFileRecord objActiveRecord in ActiveRecords)
                {
                    if (objActiveRecord.Keys.Contains(strKey) == true)
                    {
                        objRecord = objActiveRecord;
                        break;
                    }
                }
            }

            return objRecord;
        }

        public CompressedFileRecord[] GetRecordsByKey(string strKey)
        {
            List<CompressedFileRecord> objRecords = new List<CompressedFileRecord>();

            if (strKey != null)
            {
                foreach (CompressedFileRecord objActiveRecord in ActiveRecords)
                {
                    if (objActiveRecord.Keys.Contains(strKey) == true)
                    {
                        objRecords.Add(objActiveRecord);
                    }
                }
            }

            return objRecords.ToArray();
        }

        public int FindFileStartIndex(CompressedFileRecord objRecord)
        {
            int intStartIndex = FileDataStartingIndex;
            bool blnFoundRecord = false;

            foreach (CompressedFileRecord objKey in SortedRecords.Keys)
            {
                if (objRecord == objKey)
                {
                    blnFoundRecord = true;
                    break;
                }

                intStartIndex += objKey.CompressedSize;
            }

            if (blnFoundRecord == false)
            {
                intStartIndex = -1;
            }

            return intStartIndex;
        }

        #region Static Members

        public static string ToString(CompressedFileRecord[] objRecords)
        {
            StringBuilder objValue = new StringBuilder();

            foreach (CompressedFileRecord objRecord in objRecords)
            {
                objValue.Append(objRecord.ToString());
                objValue.Append(CompressedFileRecordManager.RecordDelimeter);
            }

            return objValue.ToString();
        }

        public static CompressedFileRecord[] FromString(string strStringValue)
        {
            List<CompressedFileRecord> objRecords = new List<CompressedFileRecord>();

            string[] strRecords = strStringValue.Split(new string[] { CompressedFileRecordManager.RecordDelimeter }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string strRecord in strRecords)
            {
                CompressedFileRecord objRecord = new CompressedFileRecord(strRecord);
                objRecords.Add(objRecord);
            }

            return objRecords.ToArray();
        }

        #endregion
    }

    public class CompressedPackage
    {
        private readonly byte[] CompressedStreamSignature = new byte[] { 128, 162, 178, 241, 143, 235, 250, 196 };

        private Stream _objCompressedStream;
        private CompressedFileRecordManager _objCompressedFileRecordManager;

        public CompressedPackage(string strFilePath)
        {
            string strDirectory = Path.GetDirectoryName(strFilePath);
            if (Directory.Exists(strDirectory) == false)
            {
                Directory.CreateDirectory(strDirectory);
            }

            CompressedStream = new FileStream(strFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Refresh();
        }

        public CompressedPackage(Stream objStream)
        {
            CompressedStream = objStream;
            Refresh();
        }

        private Stream CompressedStream
        {
            get
            {
                return _objCompressedStream;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("CompressedStream", "A valid non-null Stream is required.");
                }

                _objCompressedStream = value;
            }
        }

        private CompressedFileRecordManager CompressedFileRecordManager
        {
            get
            {
                return _objCompressedFileRecordManager;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("CompressedFileRecordManager", "A valid non-null CompressedFileRecordManager is required.");
                }

                _objCompressedFileRecordManager = value;
            }
        }

        public CompressedFileRecord[] Files
        {
            get
            {
                return CompressedFileRecordManager.ActiveRecords;
            }
        }

        public CompressedFileRecord GetFileByKey(string strKey)
        {
            return CompressedFileRecordManager.GetRecordByKey(strKey);
        }

        public CompressedFileRecord[] GetFilesByKey(string strKey)
        {
            return CompressedFileRecordManager.GetRecordsByKey(strKey);
        }

        public void Clear()
        {
            CompressedFileRecordManager.Clear();
        }

        public void Close()
        {
            CompressedStream.Close();
        }

        private void Refresh()
        {
            int intFileDataStartingIndex = 0;
            CompressedFileRecord[] objRecords = new CompressedFileRecord[] { };

            CompressedStream.Position = 0;
            if (CompressedStream.Length > 0)
            {
                if (CompressedStream.Length < CompressedStreamSignature.Length)
                {
                    throw new Exception("The stream does not represent a valid format.");
                }

                byte[] bytBuffer = new byte[CompressedStreamSignature.Length];
                CompressedStream.Read(bytBuffer, 0, bytBuffer.Length);

                for (int intIndex = 0; intIndex < bytBuffer.Length; intIndex++)
                {
                    if (bytBuffer[intIndex] != CompressedStreamSignature[intIndex])
                    {
                        throw new Exception("The stream does not represent a valid format.");
                    }
                }

                bytBuffer = new byte[4];
                CompressedStream.Read(bytBuffer, 0, 4);

                int intRecordLength = BitConverter.ToInt32(bytBuffer, 0);
                bytBuffer = new byte[intRecordLength];

                CompressedStream.Read(bytBuffer, 0, intRecordLength);
                intFileDataStartingIndex = (int)CompressedStream.Position;

                byte[] bytRecordBytes = CompressionManager.Decompress(bytBuffer);
                string strRecordData = ASCIIEncoding.ASCII.GetString(bytRecordBytes);

                objRecords = CompressedFileRecordManager.FromString(strRecordData);
            }

            CompressedFileRecordManager = new CompressedFileRecordManager(intFileDataStartingIndex, objRecords, CompressedFileRecordState.Updated);
        }

        public byte[] ExtractFile(CompressedFileRecord objRecord)
        {
            if (objRecord == null)
            {
                throw new ArgumentNullException("objRecord", "A valid non-null CompressedFileRecord is required.");
            }

            if (CompressedFileRecordManager.Exists(objRecord) == false)
            {
                throw new Exception("A record for '" + objRecord.RelativePath + "' could not be found.");
            }

            CompressedFileRecordState enuRecordState = CompressedFileRecordManager.GetRecordState(objRecord);
            if (enuRecordState == CompressedFileRecordState.Deleted)
            {
                throw new Exception("A record for '" + objRecord.RelativePath + "' could not be found.");
            }

            using (MemoryStream objMemoryStream = new MemoryStream())
            {
                ExtractFile(objRecord, enuRecordState, objMemoryStream);
                objMemoryStream.Position = 0;

                return objMemoryStream.ToArray();
            }
        }

        public void ExtractFile(CompressedFileRecord objRecord, Stream objOutputStream)
        {
            if (objRecord == null)
            {
                throw new ArgumentNullException("objRecord", "A valid non-null CompressedFileRecord is required.");
            }
            if (objOutputStream == null)
            {
                throw new ArgumentNullException("objOutputStream", "A valid non-null Stream is required.");
            }

            if (CompressedFileRecordManager.Exists(objRecord) == false)
            {
                throw new Exception("A record for '" + objRecord.RelativePath + "' could not be found.");
            }

            CompressedFileRecordState enuRecordState = CompressedFileRecordManager.GetRecordState(objRecord);
            if (enuRecordState == CompressedFileRecordState.Deleted)
            {
                throw new Exception("A record for '" + objRecord.RelativePath + "' could not be found.");
            }

            ExtractFile(objRecord, enuRecordState, objOutputStream);
        }

        public FileInfo ExtractFile(CompressedFileRecord objRecord, string strPath)
        {
            if (objRecord == null)
            {
                throw new ArgumentNullException("objRecord", "A valid non-null CompressedFileRecord is required.");
            }
            if (strPath == null)
            {
                throw new ArgumentNullException("strPath", "A valid non-null string is required.");
            }

            if (CompressedFileRecordManager.Exists(objRecord) == false)
            {
                throw new Exception("A record for '" + objRecord.RelativePath + "' could not be found.");
            }

            CompressedFileRecordState enuRecordState = CompressedFileRecordManager.GetRecordState(objRecord);
            if (enuRecordState == CompressedFileRecordState.Deleted)
            {
                throw new Exception("A record for '" + objRecord.RelativePath + "' could not be found.");
            }

            string strFilePath = Path.Combine(strPath, objRecord.RelativeFilePath);
            string strDirectoryPath = Path.GetDirectoryName(strFilePath);

            if (Directory.Exists(strDirectoryPath) == false)
            {
                Directory.CreateDirectory(strDirectoryPath);
            }

            using (FileStream objFileStream = new FileStream(strFilePath, FileMode.Create, FileAccess.Write))
            {
                ExtractFile(objRecord, enuRecordState, objFileStream);
            }

            FileInfo objFileInfo = new FileInfo(strFilePath);
            return objFileInfo;
        }

        public FileInfo[] ExtractFiles(IEnumerable<CompressedFileRecord> objRecords, string strPath)
        {
            if (objRecords == null)
            {
                throw new ArgumentNullException("objRecords", "A valid non-null IEnumerable<CompressedFileRecord> is required.");
            }
            if (strPath == null)
            {
                throw new ArgumentNullException("strPath", "A valid non-null string is required.");
            }

            List<FileInfo> objFileInfoRecords = new List<FileInfo>();
            foreach (CompressedFileRecord objRecord in objRecords)
            {
                FileInfo objFileInfo = ExtractFile(objRecord, strPath);
                objFileInfoRecords.Add(objFileInfo);
            }

            return objFileInfoRecords.ToArray();
        }

        public FileInfo[] ExtractAll(string strPath)
        {
            if (strPath == null)
            {
                throw new ArgumentNullException("strPath", "A valid non-null string is required.");
            }

            CompressedFileRecord[] objRecords = CompressedFileRecordManager.ActiveRecords;
            return ExtractFiles(objRecords, strPath);
        }

        public CompressedFileRecord[] AddDirectory(string strDirectory)
        {
            return AddDirectory(strDirectory, string.Empty);
        }

        public CompressedFileRecord[] AddDirectory(string strDirectory, string strRelativePath)
        {
            if ((strDirectory == null) || (strDirectory.Length == 0))
            {
                throw new ArgumentOutOfRangeException("strDirectory", "A valid non-null, non-empty string is required.");
            }
            if (Directory.Exists(strDirectory) == false)
            {
                throw new DirectoryNotFoundException("The source directory could not be found: '" + strDirectory + "'.");
            }

            List<CompressedFileRecord> objRecords = new List<CompressedFileRecord>();
            AddDirectory(strDirectory, strRelativePath, objRecords);

            return objRecords.ToArray();
        }

        private void AddDirectory(string strDirectory, string strRelativePath, List<CompressedFileRecord> objRecords)
        {
            if ((strDirectory == null) || (strDirectory.Length == 0))
            {
                throw new ArgumentOutOfRangeException("strDirectory", "A valid non-null, non-empty string is required.");
            }
            if (Directory.Exists(strDirectory) == false)
            {
                throw new DirectoryNotFoundException("The source directory could not be found: '" + strDirectory + "'.");
            }

            DirectoryInfo objDirectoryInfo = new DirectoryInfo(strDirectory);
            string strFileRelativePath = Path.Combine(strRelativePath, objDirectoryInfo.Name);

            foreach (FileInfo objFileInfo in objDirectoryInfo.GetFiles())
            {
                CompressedFileRecord objRecord = AddFile(objFileInfo.FullName, strFileRelativePath);
                objRecords.Add(objRecord);
            }
            foreach (DirectoryInfo objChildDirectoryInfo in objDirectoryInfo.GetDirectories())
            {
                AddDirectory(objChildDirectoryInfo.FullName, strFileRelativePath, objRecords);
            }
        }

        public CompressedFileRecord AddFile(string strFilePath)
        {
            return AddFile(strFilePath, string.Empty);
        }

        public CompressedFileRecord AddFile(string strFilePath, string strRelativePath)
        {
            if ((strFilePath == null) || (strFilePath.Length == 0))
            {
                throw new ArgumentOutOfRangeException("strFilePath", "A valid non-null, non-empty string is required.");
            }
            if (File.Exists(strFilePath) == false)
            {
                throw new FileNotFoundException("The source file could not be found.", strFilePath);
            }

            string strFormattedRelativePath = strRelativePath;
            if (Path.GetFileName(strFilePath).ToLower() == strRelativePath.ToLower())
            {
                strFormattedRelativePath = Path.GetDirectoryName(strRelativePath);
            }

            CompressedFileRecord objRecord = new CompressedFileRecord(strFilePath, 0, strFormattedRelativePath);
            CompressedFileRecordManager.Insert(objRecord);

            return objRecord;
        }

        public void Save()
        {
            List<CompressedFileRecord> objSavedRecords = new List<CompressedFileRecord>();

            string strTempFilePath = Path.GetTempFileName();
            using (FileStream objTempFileStream = new FileStream(strTempFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                CompressedFileRecord[] objInsertedRecords = CompressedFileRecordManager.InsertedRecords;
                foreach (CompressedFileRecord objInsertedRecord in objInsertedRecords)
                {
                    using (FileStream objInputFile = new FileStream(objInsertedRecord.OriginalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        int intStartingPosition = (int)objTempFileStream.Position;
                        CompressionManager.Compress(objInputFile, objTempFileStream);

                        int intCompressedSize = (int)objTempFileStream.Position - intStartingPosition;
                        CompressedFileRecord objSavedRecord = new CompressedFileRecord(objInsertedRecord.OriginalFilePath, intCompressedSize, objInsertedRecord.RelativePath);
                        objSavedRecord.Keys.AddRange(objInsertedRecord.Keys);
                        objSavedRecords.Add(objSavedRecord);
                    }

                    objTempFileStream.Flush();
                }

                CompressedFileRecord[] objUpdatedRecords = CompressedFileRecordManager.UpdatedRecords;
                foreach (CompressedFileRecord objUpdatedRecord in objUpdatedRecords)
                {
                    int intFileStartIndex = CompressedFileRecordManager.FindFileStartIndex(objUpdatedRecord);
                    if (intFileStartIndex != -1)
                    {
                        CompressedStream.Position = intFileStartIndex;
                        CompressionManager.CopyToStream(CompressedStream, objTempFileStream, objUpdatedRecord.CompressedSize);
                        objSavedRecords.Add(objUpdatedRecord);
                    }

                    objTempFileStream.Flush();
                }

                objTempFileStream.Position = 0;
                CompressedStream.SetLength(0);
                CompressedStream.Position = 0;

                CompressedStream.Write(CompressedStreamSignature, 0, CompressedStreamSignature.Length);

                string strSavedRecords = CompressedFileRecordManager.ToString(objSavedRecords.ToArray());
                byte[] bytSavedRecords = ASCIIEncoding.ASCII.GetBytes(strSavedRecords);
                byte[] bytCompressedRecords = CompressionManager.Compress(bytSavedRecords);
                int intRecordLength = bytCompressedRecords.Length;
                byte[] bytHeaderRecordLength = BitConverter.GetBytes(intRecordLength);
                CompressedStream.Write(bytHeaderRecordLength, 0, bytHeaderRecordLength.Length);
                CompressedStream.Write(bytCompressedRecords, 0, bytCompressedRecords.Length);

                CompressionManager.CopyToStream(objTempFileStream, CompressedStream);
                CompressedStream.Flush();
            }

            File.Delete(strTempFilePath);
            Refresh();
        }

        private void ExtractFile(CompressedFileRecord objRecord, CompressedFileRecordState enuRecordState, Stream objTargetStream)
        {
            if (enuRecordState == CompressedFileRecordState.Inserted)
            {
                using (FileStream objDiskFileStream = new FileStream(objRecord.OriginalFilePath, FileMode.Open, FileAccess.Read))
                {
                    CompressionManager.CopyToStream(objDiskFileStream, objTargetStream);
                }
            }
            else
            {
                int intFileStartIndex = CompressedFileRecordManager.FindFileStartIndex(objRecord);
                if (intFileStartIndex == -1)
                {
                    throw new FileNotFoundException("Unable to locate '" + objRecord.RelativeFilePath + "' within package.");
                }

                CompressedStream.Position = intFileStartIndex;
                CompressionManager.Decompress(CompressedStream, objTargetStream, objRecord.Size);
            }
        }
    }

    public class CompressionManager
    {
        #region Static Members

        public static int Decompress(Stream objInputStream, Stream objOutputStream)
        {
            int intStartingPosition = (int)objOutputStream.Position;
            using (GZipStream objZipStream = new GZipStream(objInputStream, CompressionMode.Decompress, true))
            {
                CopyToStream(objZipStream, objOutputStream);
            }
            int intEndingPosition = (int)objOutputStream.Position;

            int intLength = intEndingPosition - intStartingPosition + 1;
            return intLength;
        }

        public static void Decompress(Stream objInputStream, Stream objOutputStream, int intLength)
        {
            using (GZipStream objZipStream = new GZipStream(objInputStream, CompressionMode.Decompress, true))
            {
                CopyToStream(objZipStream, objOutputStream, intLength);
            }
        }

        public static byte[] CompressFromString(string strInputString)
        {
            byte[] bytData = ASCIIEncoding.ASCII.GetBytes(strInputString);
            byte[] bytCompressedData = CompressionManager.Compress(bytData);
            return bytCompressedData;
        }

        public static string DecompressToString(byte[] bytData)
        {
            byte[] bytDecompressedData = CompressionManager.Decompress(bytData);

            string strData = ASCIIEncoding.ASCII.GetString(bytDecompressedData);
            return strData;
        }

        public static int Compress(Stream objInputStream, Stream objOutputStream)
        {
            int intLength = 0;

            using (GZipStream objZipStream = new GZipStream(objOutputStream, CompressionMode.Compress, true))
            {
                intLength = CopyToStream(objInputStream, objZipStream);
            }

            return intLength;
        }

        public static void Compress(Stream objInputStream, Stream objOutputStream, int intLength)
        {
            using (GZipStream objZipStream = new GZipStream(objOutputStream, CompressionMode.Compress, true))
            {
                CopyToStream(objInputStream, objZipStream, intLength);
            }
        }

        public static byte[] Compress(byte[] bytInputBytes)
        {
            byte[] bytOutputBytes = null;

            using (MemoryStream objInputStream = new MemoryStream(bytInputBytes))
            {
                using (MemoryStream objOutputStream = new MemoryStream())
                {
                    int intTotalBytes = Compress(objInputStream, objOutputStream);
                    bytOutputBytes = objOutputStream.ToArray();
                }
            }

            return bytOutputBytes;
        }

        public static byte[] Decompress(byte[] bytInputBytes)
        {
            byte[] bytOutputBytes = null;

            using (MemoryStream objInputStream = new MemoryStream(bytInputBytes))
            {
                using (MemoryStream objOutputStream = new MemoryStream())
                {
                    int intTotalBytes = Decompress(objInputStream, objOutputStream);
                    bytOutputBytes = objOutputStream.ToArray();
                }
            }

            return bytOutputBytes;
        }

        public static int CopyToStream(Stream objInputStream, Stream objOutputStream)
        {
            int intBytesRead = 0;
            int intTotalBytesRead = 0;
            byte[] bytBuffer = new byte[4096];

            while ((intBytesRead = objInputStream.Read(bytBuffer, 0, bytBuffer.Length)) != 0)
            {
                objOutputStream.Write(bytBuffer, 0, intBytesRead);
                intTotalBytesRead += intBytesRead;
            }

            return intTotalBytesRead;
        }

        public static int CopyToStream(Stream objInputStream, Stream objOutputStream, int intLength)
        {
            int intTotalBytesRead = 0;
            byte[] bytBuffer = new byte[4096];

            int intBytesRemaining = intLength;
            while (intBytesRemaining > 0)
            {
                int intBytesToRead = ((intBytesRemaining < bytBuffer.Length) ? intBytesRemaining : bytBuffer.Length);
                int intBytesRead = objInputStream.Read(bytBuffer, 0, intBytesToRead);
                objOutputStream.Write(bytBuffer, 0, intBytesRead);

                intBytesRemaining -= intBytesRead;
                intTotalBytesRead += intBytesRead;
            }

            return intTotalBytesRead;
        }

        #endregion
    }
}
