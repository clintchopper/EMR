namespace ReLi.Framework.Library.Security
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Principal;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;

    #endregion
        
	public class AccountToken : ObjectBase  
    {
        private Guid _objGuid;
        private string _strUserName;

        public AccountToken(Guid objGuid, string strUserName)
            : base()
        {
            Guid = objGuid;
            UserName = strUserName;
        }

        public AccountToken(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public AccountToken(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public Guid Guid
        {
            get
            {
                return _objGuid;
            }
            protected set
            {
                _objGuid = value;
            }
        }

        public string UserName
        {
            get
            {
                return _strUserName;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("UserName", "A valid non-null string is required.");
                }

                _strUserName = value;
            }
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Guid", Guid);
            objSerializedObject.Values.Add("UserName", _strUserName);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Guid = objSerializedObject.Values.GetValue<Guid>("Guid", Guid.Empty);
            UserName = objSerializedObject.Values.GetValue<string>("UserName", string.Empty);
        }

        #endregion       

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.WriteGuid(Guid);
            objBinaryWriter.Write(UserName);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Guid = objBinaryReader.ReadGuid();
            UserName = objBinaryReader.ReadString();
        }

        #endregion       

        #region Static Members

        private static object _objSyncObject = new object();
        private static AccountToken _objSupportAccount = null;
        private static AccountToken _objInvalidAccount = null;

        static AccountToken()
        {
            SyncObject = null;
            SupportAccount = null;
            InvalidAccount = null;
        }

        private static object SyncObject
        {
            get
            {
                if (_objSyncObject == null)
                {
                    _objSyncObject = new object();
                }

                return _objSyncObject;
            }
            set
            {
                _objSyncObject = value;
            }
        }

        public static AccountToken SupportAccount
        {
            get
            {
                if (_objSupportAccount == null)
                {
                    lock (SyncObject)
                    {
                        Guid objGuid = new Guid("63FE4F5E-A3CB-11DE-960B-575756D89593");
                        string strUserName = "Support Account";

                        _objSupportAccount = new AccountToken(objGuid, strUserName);
                    }
                }

                return _objSupportAccount;
            }
            private set
            {
                _objSupportAccount = null;
            }
        }

        public static AccountToken InvalidAccount
        {
            get
            {
                if (_objInvalidAccount == null)
                {
                    lock (SyncObject)
                    {
                        Guid objGuid = new Guid("00000000-0000-0000-0000-000000000000");
                        string strUserName = "Invalid Account";

                        _objInvalidAccount = new AccountToken(objGuid, strUserName);
                    }
                }

                return _objInvalidAccount;
            }
            private set
            {
                _objInvalidAccount = null;
            }
        }

        public static AccountToken EmptyAccount
        {
            get
            {
                if (_objInvalidAccount == null)
                {
                    lock (SyncObject)
                    {
                        Guid objGuid = new Guid("00000000-0000-0000-0000-000000000000");
                        string strUserName = string.Empty;

                        _objInvalidAccount = new AccountToken(objGuid, strUserName);
                    }
                }

                return _objInvalidAccount;
            }
            private set
            {
                _objInvalidAccount = null;
            }
        }

        #endregion
    }
}
