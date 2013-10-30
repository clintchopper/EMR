namespace ReLi.Framework.Library.Net.Mail
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Security.Principal;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;
    using NetMail = System.Net.Mail;

    #endregion
        
	public class MailMessage : ObjectBase
    {
        #region Constant Declarations

        public static NetMail.DeliveryNotificationOptions DefaultDeliveryNotificationOptions = NetMail.DeliveryNotificationOptions.OnFailure;
        public static bool DefaultIsBodyHtml = false;

        #endregion

        private ListBase<string> _objTo;
        private string _strFrom;
        private bool _blnIsBodyHtml;
        private string _strSubject;
        private string _strBody;
        private NetMail.DeliveryNotificationOptions _enuDeliveryNotificationOptions;

        public MailMessage()
            : this(string.Empty, string.Empty, string.Empty, string.Empty)
        {}

        public MailMessage(string strFrom, string strTo)
            : this(strFrom, strTo, string.Empty, string.Empty)
        {}

        public MailMessage(string strFrom, string strTo, string strSubject, string strBody)
            : base()
        {
            From = strFrom;
            To = new ListBase<string>();
            if ((strTo != null) && (strTo.Trim().Length > 0))
            {
                To.AddRange(strTo.Split(new char[]{';', ','}));
            }
            Subject = strSubject;
            Body = strBody;
            IsBodyHtml = DefaultIsBodyHtml;
            DeliveryNotificationOptions = DefaultDeliveryNotificationOptions;
        }

        public MailMessage(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public MailMessage(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public ListBase<string> To
        {
            get
            {
                return _objTo;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("To", "A valid non-null ListBase<string> is required.");
                }

                _objTo = value;
            }
        }

        public string From
        {
            get
            {
                return _strFrom;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("From", "A valid non-null string is required.");
                }

                _strFrom = value;
            }
        }
          
        public bool IsBodyHtml
        {
            get
            {
                return _blnIsBodyHtml;
            }
            set
            {
                _blnIsBodyHtml = value;
            }
        }

        public string Subject
        {
            get
            {
                return _strSubject;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Subject", "A valid non-null string is required.");
                }

                _strSubject = value;
            }
        }

        public string Body
        {
            get
            {
                return _strBody;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Body", "A valid non-null string is required.");
                }

                _strBody = value;
            }
        }

        public NetMail.DeliveryNotificationOptions DeliveryNotificationOptions
        {
            get
            {
                return _enuDeliveryNotificationOptions;
            }
            set
            {
                _enuDeliveryNotificationOptions = value;
            }
        }
            
        internal NetMail.MailMessage CreateMailMessage()
        {
            NetMail.MailMessage objMailMessage = new NetMail.MailMessage();

            foreach (string strAddress in To)
            {
                objMailMessage.To.Add(strAddress);
            }

            objMailMessage.From = new NetMail.MailAddress(From);
            objMailMessage.IsBodyHtml = IsBodyHtml;
            objMailMessage.Subject = Subject;
            objMailMessage.Body = Body;
            objMailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions;

            return objMailMessage;
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("From", From);
            objSerializedObject.Values.Add("Subject", Subject);
            objSerializedObject.Values.Add("IsBodyHtml", IsBodyHtml);
            objSerializedObject.Values.Add("Body", Body);
            objSerializedObject.Values.Add("DeliveryNotificationOptions", DeliveryNotificationOptions);
            objSerializedObject.Objects.Add("To", To);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            From = objSerializedObject.Values.GetValue<string>("From", string.Empty);
            Subject = objSerializedObject.Values.GetValue<string>("Subject", string.Empty);
            IsBodyHtml = objSerializedObject.Values.GetValue<bool>("IsBodyHtml", DefaultIsBodyHtml);
            Body = objSerializedObject.Values.GetValue<string>("Body", string.Empty);
            DeliveryNotificationOptions = objSerializedObject.Values.GetValue<NetMail.DeliveryNotificationOptions>("DeliveryNotificationOptions", DefaultDeliveryNotificationOptions);
            To = objSerializedObject.Objects.GetObject<ListBase<string>>("To", new ListBase<string>());
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(From);
            objBinaryWriter.Write(Subject);
            objBinaryWriter.Write(IsBodyHtml);
            objBinaryWriter.Write(Body);
            objBinaryWriter.Write((byte)DeliveryNotificationOptions);
            objBinaryWriter.WriteTransportableObject(To);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            From = objBinaryReader.ReadString();
            Subject = objBinaryReader.ReadString();
            IsBodyHtml = objBinaryReader.ReadBoolean();
            Body = objBinaryReader.ReadString();
            DeliveryNotificationOptions = (NetMail.DeliveryNotificationOptions)objBinaryReader.ReadByte();
            To = objBinaryReader.ReadTransportableObject<ListBase<string>>();
        }

        #endregion

     }
}
