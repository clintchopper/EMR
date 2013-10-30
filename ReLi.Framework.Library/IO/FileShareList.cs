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
        
	public class FileShareList : ObjectListBase<FileShare>
    {
        public FileShareList(IEnumerable<FileShare> objFileShares)
            : base(objFileShares)
        {}

        public FileShareList(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public FileShareList(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public void Refresh()
        {
            base.Clear();

            FileShareList objFileShareList = FileShareList.Load();
            base.AddRange(objFileShareList);
        }

        public void Sort()
        {
            Comparison<FileShare> objComparison = new Comparison<FileShare>(FileShareComparison);
            base.Sort(objComparison);
        }

        public bool Exists(string strName)
        {
            FileShare objFileShare = GetByName(strName);
            return (objFileShare != null);
        }

        public FileShare GetByName(string strName)
        {
            FileShare objFileShare = null;

            string strFormattedName = strName.ToLower();
            foreach (FileShare objChildRecord in this)
            {
                if (objChildRecord.Name.ToLower() == strFormattedName)
                {
                    objFileShare = objChildRecord;
                    break;
                }
            }

            return objFileShare;
        }

        public FileShare GetByPath(string strPath)
        {
            FileShare objFileShare = null;

            string strFormattedPath = strPath.ToLower();
            foreach (FileShare objChildRecord in this)
            {
                if (objChildRecord.Path.ToLower() == strFormattedPath)
                {
                    objFileShare = objChildRecord;
                    break;
                }
            }

            return objFileShare;
        }

        private int FileShareComparison(FileShare objFileShare1, FileShare objFileShare2)
        {
            int intResult = 0;

            if ((objFileShare1 == null) && (objFileShare2 == null))
            {
                intResult = 0;
            }
            else if (objFileShare1 == null)
            {
                intResult = -1;
            }
            else if (objFileShare2 == null)
            {
                intResult = 1;
            }
            else
            {
                intResult = objFileShare1.Name.CompareTo(objFileShare2.Name);
            }

            return intResult;
        }

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

        public static FileShareList Load()
        {
            FileShareList objFileShareList = FileShare.Load();
            return objFileShareList;
        }

        #endregion

    }
}
