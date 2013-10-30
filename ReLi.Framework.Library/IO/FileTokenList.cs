namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Text;
    using System.Security.Principal;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Security.Hash;

    #endregion

    public class FileTokenList : ObjectListBase<FileToken>
    {
        private Dictionary<string, FileToken> _objLookupDictionary;

        public FileTokenList()
            : base()
        {
            Initialize();
        }

        public FileTokenList(IEnumerable<FileToken> objFileTokens)
            : base(objFileTokens)
        {
            Initialize();
        }

        public FileTokenList(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public FileTokenList(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        private Dictionary<string, FileToken> LookupDictionary
        {
            get
            {
                return _objLookupDictionary;
            }
            set
            {
                _objLookupDictionary = value;
            }
        }

        public void Remove(string strRelativePath)
        {
            if (LookupDictionary.ContainsKey(strRelativePath) == true)
            {
                FileToken objFileToken = LookupDictionary[strRelativePath];
                this.Remove(objFileToken);
            }
        }

        public bool Exists(string strRelativePath)
        {
            bool blnExists = LookupDictionary.ContainsKey(strRelativePath);
            return blnExists;
        }

        public FileTokenList CompareToDirectory(string strDirectory, bool blnFindDifferent, bool blnFindSame)
        {
            bool blnIncludeRecord = false;
            FileTokenList objIncludedRecords = new FileTokenList();

            foreach (FileToken objChildRecord in this)
            {
                blnIncludeRecord = true;

                string strFilePath = Path.Combine(strDirectory, objChildRecord.RelativePath);
                if (File.Exists(strFilePath) == true)
                {
                    FileToken objFileToken = FileToken.Load(strFilePath);
                    if (objFileToken != null)
                    {
                        if (objFileToken.Equals(objChildRecord) == true)
                        {
                            blnIncludeRecord = false;
                        }
                    }
                }

                if (blnIncludeRecord == true)
                {
                    objIncludedRecords.Add(objChildRecord);
                }
            }

            return objIncludedRecords;
        }

        public void Merge(FileTokenList objFileTokenList)
        {
            foreach (FileToken objFileToken in objFileTokenList)
            {
                string strKey = objFileToken.RelativePath;
                if (LookupDictionary.ContainsKey(strKey) == true)
                {
                    this.Remove(strKey);
                }

                this.Add(objFileToken);
            }
        }

        public string GenerateHash()
        {
            List<string> objHashValues = new List<string>();
            foreach (FileToken objFileToken in this)
            {
                objHashValues.Add(objFileToken.Hash);
            }
            objHashValues.Sort();

            StringBuilder objHashBuilder = new StringBuilder();
            foreach (string strHashValue in objHashValues)
            {
                objHashBuilder.Append(strHashValue);
            }

            string strHash = HashBase.Default.ComputeHash(objHashBuilder.ToString());
            return strHash;
        }

        protected override void Initialize()
        {
            base.Initialize();

            LookupDictionary = new Dictionary<string, FileToken>();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            base.ItemAddedEvent += new ItemAddedEventHandler<FileToken>(FileTokenList_ItemAddedEvent);
            base.ItemRemovedEvent += new ItemRemovedEventHandler<FileToken>(FileTokenList_ItemRemovedEvent);
        }

        private void UnregisterEvents()
        {
            base.ItemAddedEvent -= new ItemAddedEventHandler<FileToken>(FileTokenList_ItemAddedEvent);
            base.ItemRemovedEvent -= new ItemRemovedEventHandler<FileToken>(FileTokenList_ItemRemovedEvent);
        }

        private void FileTokenList_ItemAddedEvent(object objSender, ItemAddedEventArgs<FileToken> objArguments)
        {
            string strKey = objArguments.Item.RelativePath;
            if (LookupDictionary.ContainsKey(strKey) == false)
            {
                LookupDictionary.Add(objArguments.Item.RelativePath, objArguments.Item);
            }
        }

        private void FileTokenList_ItemRemovedEvent(object objSender, ItemRemovedEventArgs<FileToken> objArguments)
        {
            string strKey = objArguments.Item.RelativePath;
            if (LookupDictionary.ContainsKey(strKey) == true)
            {
                LookupDictionary.Remove(strKey);
            }
        }

        #region ICustomSerializer Members

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

        public static FileTokenList Load(string strDirectory)
        {
            return Load(strDirectory, true);
        }

        public static FileTokenList Load(string strDirectory, bool blnIncludeSubDirectories)
        {
            return Load(strDirectory, strDirectory, blnIncludeSubDirectories);
        }

        private static FileTokenList Load(string strBaseDirectory, string strCurrentDirectory, bool blnIncludeSubDirectories)
        {
            FileTokenList objFileTokenList = new FileTokenList();

            if (Directory.Exists(strCurrentDirectory) == true)
            {
                DirectoryInfo objDirectoryInfo = new DirectoryInfo(strCurrentDirectory);

                if (blnIncludeSubDirectories == true)
                {
                    foreach (DirectoryInfo objChildDirectoryInfo in objDirectoryInfo.GetDirectories())
                    {
                        FileTokenList objChildFileTokenList = FileTokenList.Load(strBaseDirectory, objChildDirectoryInfo.FullName, blnIncludeSubDirectories);
                        objFileTokenList.AddRange(objChildFileTokenList);
                    }
                }

                foreach (FileInfo objFileInfo in objDirectoryInfo.GetFiles())
                {
                    string strDirectory = Path.GetDirectoryName(objFileInfo.FullName);
                    string strRelativePath = strDirectory.Replace(strBaseDirectory, "");
                    if (strRelativePath.StartsWith("\\") == true)
                    {
                        strRelativePath = strRelativePath.Substring(1);
                    }

                    FileToken objFileToken = FileToken.Load(objFileInfo.FullName, strRelativePath);
                    if (objFileToken != null)
                    {
                        objFileTokenList.Add(objFileToken);
                    }
                }
            }

            return objFileTokenList;
        }

        #endregion
    }
}
