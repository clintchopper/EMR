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
        
	public class ErrorMessage : MessageBase
    {
        private string _strDetails;
        private Exception _objException;
        
        public ErrorMessage(string strDetails)
            : this(null, strDetails)
        {}

        public ErrorMessage(Exception objException)
            : this(objException, string.Empty)
        {}

        public ErrorMessage(Exception objException, string strDetails)
            : base()
        {
            Details = strDetails;            
            Exception = objException;
        }

        public ErrorMessage(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}
        
        public ErrorMessage(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}
 
        public string Details
        {
            get
            {
                return _strDetails;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Details", "A valid non-null string is required.");
                }

                _strDetails = value;
            }
        }

        public Exception Exception
        {
            get
            {
                return _objException;
            }
            set
            {
                _objException = value;
            }
        }

        public override string Content
        {
            get
            {
                StringBuilder objMessage = new StringBuilder();

                if (Exception != null)
                {
                    Exception objInnerException = Exception;
                    while (objInnerException != null)
                    {
                        objMessage.AppendLine("Message: " + objInnerException.Message);
                        objMessage.AppendLine("Type: " + objInnerException.GetType().FullName);
                        objMessage.AppendLine("Source: " + objInnerException.Source);
                        objMessage.AppendLine("Stack Trace: " + objInnerException.StackTrace);
                        
                        objInnerException = objInnerException.InnerException;
                        if (objInnerException != null)
                        {
                            objMessage.AppendLine("-->Inner Exception");
                        }
                    }
                }
                if (Details.Length > 0)
                {
                    objMessage.AppendLine("Details: " + Details);
                }

                return objMessage.ToString();
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

            objSerializedObject.Values.Add("Details", Details);
            objSerializedObject.Values.Add("Content", Content);

            if (Exception != null)
            {
                SerializedWrapperObject objExceptionWrapper = new SerializedWrapperObject(Exception);
                objSerializedObject.Objects.Add("Exception", objExceptionWrapper);
            }
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Details = objSerializedObject.Values.GetValue<string>("Details", string.Empty);

            Exception = null;
            SerializedWrapperObject objExceptionWrapper = objSerializedObject.Objects.GetObject<SerializedWrapperObject>("Exception", null);
            if (objExceptionWrapper != null)
            {
                Exception = (Exception)objExceptionWrapper.Data;
            }
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Details);
            objBinaryWriter.WriteObject(Exception);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Details = objBinaryReader.ReadString();
            Exception = (Exception)objBinaryReader.ReadObject();
        }

        #endregion
    }
}
