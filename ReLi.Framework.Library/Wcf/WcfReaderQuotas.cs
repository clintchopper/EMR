namespace ReLi.Framework.Library.Wcf
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Principal;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Net.WebServices;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
    public class WcfReaderQuotas : ObjectBase  
    {
        private int _intMaxArrayLength;
        private int _intMaxBytesPerRead;
        private int _intMaxDepth;
        private int _intMaxNameTableCharCount;
        private int _intMaxStringContentLength;

        public WcfReaderQuotas()
            : this(DefaultMaxArrayLength, DefaultMaxBytesPerRead, DefaultMaxDepth, DefaultMaxNameTableCharCount, DefaultMaxStringContentLength)
        {}

        public WcfReaderQuotas(int intMaxArrayLength, int intMaxBytesPerRead, int intMaxDepth, int intMaxNameTableCharCount, int intMaxStringContentLength)
            : base()
        {
            MaxArrayLength = intMaxArrayLength;
            MaxBytesPerRead = intMaxBytesPerRead;
            MaxDepth = intMaxDepth;
            MaxNameTableCharCount = intMaxNameTableCharCount;
            MaxStringContentLength = intMaxStringContentLength;
        }

        public WcfReaderQuotas(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public WcfReaderQuotas(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public int MaxArrayLength
        {
            get
            {
                return _intMaxArrayLength;
            }
            set
            {
                _intMaxArrayLength = value;
            }
        }

        public int MaxBytesPerRead
        {
            get
            {
                return _intMaxBytesPerRead;
            }
            set
            {
                _intMaxBytesPerRead = value;
            }
        }

        public int MaxDepth
        {
            get
            {
                return _intMaxDepth;
            }
            set
            {
                _intMaxDepth = value;
            }
        }

        public int MaxNameTableCharCount
        {
            get
            {
                return _intMaxNameTableCharCount;
            }
            set
            {
                _intMaxNameTableCharCount = value;
            }
        }

        public int MaxStringContentLength
        {
            get
            {
                return _intMaxStringContentLength;
            }
            set
            {
                _intMaxStringContentLength = value;
            }
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("MaxArrayLength", MaxArrayLength);
            objSerializedObject.Values.Add("MaxBytesPerRead", MaxBytesPerRead);
            objSerializedObject.Values.Add("MaxDepth", MaxDepth);
            objSerializedObject.Values.Add("MaxNameTableCharCount", MaxNameTableCharCount);
            objSerializedObject.Values.Add("MaxStringContentLength", MaxStringContentLength);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            MaxArrayLength = objSerializedObject.Values.GetValue<int>("MaxArrayLength", DefaultMaxArrayLength);
            MaxBytesPerRead = objSerializedObject.Values.GetValue<int>("MaxBytesPerRead", DefaultMaxBytesPerRead);
            MaxDepth = objSerializedObject.Values.GetValue<int>("MaxDepth", DefaultMaxDepth);
            MaxNameTableCharCount = objSerializedObject.Values.GetValue<int>("MaxNameTableCharCount", DefaultMaxNameTableCharCount);
            MaxStringContentLength = objSerializedObject.Values.GetValue<int>("MaxStringContentLength", DefaultMaxStringContentLength);            
        }

        #endregion       

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(MaxArrayLength);
            objBinaryWriter.Write(MaxBytesPerRead);
            objBinaryWriter.Write(MaxDepth);
            objBinaryWriter.Write(MaxNameTableCharCount);
            objBinaryWriter.Write(MaxStringContentLength);            
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            MaxArrayLength = objBinaryReader.ReadInt32();
            MaxBytesPerRead = objBinaryReader.ReadInt32();
            MaxDepth = objBinaryReader.ReadInt32();
            MaxNameTableCharCount = objBinaryReader.ReadInt32();
            MaxStringContentLength = objBinaryReader.ReadInt32();
        }

        #endregion

        #region Static Members

        public static readonly int DefaultMaxArrayLength = 16384;
        public static readonly int DefaultMaxBytesPerRead = 4096;
        public static readonly int DefaultMaxDepth = 32;
        public static readonly int DefaultMaxNameTableCharCount = 16384;
        public static readonly int DefaultMaxStringContentLength = 8192;

        #endregion
    }
}
