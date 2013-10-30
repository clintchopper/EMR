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
        
    public class WcfOptionalReliableSession : ObjectBase  
    {
        private bool _blnEnabled;
        private TimeSpan _objInactivityTimeout;
        private bool _blnOrdered;

        public WcfOptionalReliableSession()
            : this(DefaultEnabled, DefaultInactivityTimeout, DefaultOrdered)
        {}

        public WcfOptionalReliableSession(bool blnEnabled, TimeSpan objInactivityTimeout, bool blnOrdered)
            : base()
        {
            Enabled = blnEnabled;
            InactivityTimeout = objInactivityTimeout;
            Ordered = blnOrdered;
        }

        public WcfOptionalReliableSession(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public WcfOptionalReliableSession(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public bool Enabled
        {
            get
            {
                return _blnEnabled;
            }
            set
            {
                _blnEnabled = value;
            }
        }

        public TimeSpan InactivityTimeout
        {
            get
            {
                return _objInactivityTimeout;
            }
            set
            {
                _objInactivityTimeout = value;
            }
        }

        public bool Ordered
        {
            get
            {
                return _blnOrdered;
            }
            set
            {
                _blnOrdered = value;
            }
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Enabled", Enabled);
            objSerializedObject.Values.Add("InactivityTimeout", InactivityTimeout);
            objSerializedObject.Values.Add("Ordered", Ordered);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Enabled = objSerializedObject.Values.GetValue<bool>("Enabled", DefaultEnabled);
            InactivityTimeout = objSerializedObject.Values.GetValue<TimeSpan>("InactivityTimeout", DefaultInactivityTimeout);
            Ordered = objSerializedObject.Values.GetValue<bool>("Ordered", DefaultOrdered);            
        }

        #endregion       

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Enabled);
            objBinaryWriter.WriteTimeSpan(InactivityTimeout);
            objBinaryWriter.Write(Ordered);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Enabled = objBinaryReader.ReadBoolean();
            InactivityTimeout = objBinaryReader.ReadTimeSpan();
            Ordered = objBinaryReader.ReadBoolean();
        }

        #endregion

        #region Static Members

        public static readonly bool DefaultEnabled = false;
        public static readonly TimeSpan DefaultInactivityTimeout = new TimeSpan(0, 10, 0);
        public static readonly bool DefaultOrdered = true;

        #endregion
    }
}
