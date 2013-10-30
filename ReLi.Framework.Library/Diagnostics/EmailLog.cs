namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Net.Mail;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Serialization;

    #endregion
    
	public class EmailLog : LogBase
    {
        private SmtpClient _objSmtpClient;
        private MailMessage _objDefaultMailMessage;
        
        public EmailLog(SmtpClient objSmtpClient, MailMessage objDefaultMailMessage)
            : this(objSmtpClient, objDefaultMailMessage, new MessageTypeCollection())
        {}

        public EmailLog(SmtpClient objSmtpClient, MailMessage objDefaultMailMessage, MessageTypeCollection objMessageTypes)
            : base(objMessageTypes)
        {
            SmtpClient = objSmtpClient;
            DefaulMailMessage = objDefaultMailMessage;
        }

        public EmailLog(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}
        
        public EmailLog(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public SmtpClient SmtpClient
        {
            get
            {
                return _objSmtpClient;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("SmtpClient", "A valid non-null SmtpClient is required.");
                }

                _objSmtpClient = value;
            }
        }

        public MailMessage DefaulMailMessage
        {
            get
            {
                return _objDefaultMailMessage;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("DefaulMailMessage", "A valid non-null MailMessage is required.");
                }

                _objDefaultMailMessage = value;
            }
        }
               
        protected override void OnWriteMessage(MessageBase objMessage)
        {
            MailMessage objMailMessage = (MailMessage)DefaulMailMessage.Clone();

            objMailMessage.Subject = Environment.MachineName + ": " + objMessage.GetType().Name;
            objMailMessage.Body = objMessage.ToString();

            SmtpClient.SendMessage(objMailMessage, true);
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);
            
            objSerializedObject.Objects.Add("SmtpClient", SmtpClient);
            objSerializedObject.Objects.Add("DefaulMailMessage", DefaulMailMessage);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            SmtpClient = objSerializedObject.Objects.GetObject<SmtpClient>("SmtpClient", null);
            DefaulMailMessage = objSerializedObject.Objects.GetObject<MailMessage>("DefaulMailMessage", null);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteTransportableObject(SmtpClient);
            objBinaryWriter.WriteTransportableObject(DefaulMailMessage);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            SmtpClient = objBinaryReader.ReadTransportableObject<SmtpClient>();
            DefaulMailMessage = objBinaryReader.ReadTransportableObject<MailMessage>();
        }

        #endregion

    }
}
