namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Data;
    using System.Text;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class TraceMessage : MessageBase
    {
        private string _strCategory;
        private string _strMessage;

        public TraceMessage(string strMessage)
            : this(strMessage, string.Empty)
        {}

        public TraceMessage(string strMessage, string strCategory)
            : base()
        {
            Message = strMessage;
            Category = strCategory;
        }

        public TraceMessage(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public TraceMessage(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string Category
        {
            get
            {
                return _strCategory;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Category", "A valid non-null string is required.");
                }

                _strCategory = value;
            }
        }

        public string Message
        {
            get
            {
                return _strMessage;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Message", "A valid non-null string is required.");
                }

                _strMessage = value;
            }
        }

        public override string Content
        {
            get
            {
                StringBuilder objContent = new StringBuilder();
                if (Category.Length > 0)
                {
                    objContent.AppendLine("Category: " + Category);
                }
                if (Message.Length > 0)
                {
                    objContent.AppendLine("Message: " + Message);
                }                

                return objContent.ToString();
            }
        }

        public override string ToString()
        {
            StringBuilder objString = new StringBuilder();
            objString.Append(base.ToString());
            objString.AppendLine(Content);

            return objString.ToString();
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Message", Message);
            objSerializedObject.Values.Add("Category", Category);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Message = objSerializedObject.Values.GetValue<string>("Message", string.Empty);
            Category = objSerializedObject.Values.GetValue<string>("Category", string.Empty);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Message);
            objBinaryWriter.Write(Category);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Message = objBinaryReader.ReadString();
            Category = objBinaryReader.ReadString();
        }

        #endregion

    }
}
