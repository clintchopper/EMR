namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Principal;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public abstract class MessageBase : ObjectBase 
    {       
        private DateTime _dtCreatedDate;
        private string _strWindowsId;
        private string _strMachineName;

        protected MessageBase()
            : base()
        {
            CreatedDate = DateTime.Now;
            WindowsId = WindowsIdentity.GetCurrent().Name;
            MachineName = Environment.MachineName;
        }

        protected MessageBase(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        protected MessageBase(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}
 
        public string WindowsId
        {
            get
            {
                return _strWindowsId;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("WindowsId", "A valid non-null string is required.");
                }

                _strWindowsId = value;
            }
        }

        public string MachineName
        {
            get
            {
                return _strMachineName;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("MachineName", "A valid non-null string is required.");
                }

                _strMachineName = value;
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return _dtCreatedDate;
            }
            protected set
            {
                _dtCreatedDate = value;
            }
        }
           
        public abstract string Content
        {
            get;
        }

        public override string ToString()
        {
            StringBuilder objString = new StringBuilder();
            objString.AppendLine("Created Date: " + _dtCreatedDate.ToString());
            objString.AppendLine("Machine: " + _strMachineName);
            objString.AppendLine("Windows ID: " + _strWindowsId);

            return objString.ToString();
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("CreatedDate", _dtCreatedDate);
            objSerializedObject.Values.Add("MachineName", _strMachineName);
            objSerializedObject.Values.Add("WindowsId", _strWindowsId);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            CreatedDate = objSerializedObject.Values.GetValue<DateTime>("CreatedDate", DateTime.MinValue);
            MachineName = objSerializedObject.Values.GetValue<string>("MachineName", string.Empty);
            WindowsId = objSerializedObject.Values.GetValue<string>("WindowsId", string.Empty);
        }

        #endregion             

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteDateTime(CreatedDate);
            objBinaryWriter.Write(MachineName);
            objBinaryWriter.Write(WindowsId);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            CreatedDate = objBinaryReader.ReadDateTime();
            MachineName = objBinaryReader.ReadString();
            WindowsId = objBinaryReader.ReadString();
        }

        #endregion
    }
}
