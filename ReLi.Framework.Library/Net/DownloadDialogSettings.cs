namespace ReLi.Framework.Library.Net
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
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;

    #endregion
        
	public class DownloadDialogSettings : ObjectBase
    {
        private bool _blnAllowPause;
        private bool _blnAllowCancel;

        public DownloadDialogSettings()
            : this(true, true)
        {}

        public DownloadDialogSettings(bool blnAllowPause, bool blnAllowCancel)
            : base()
        {
            AllowPause = blnAllowPause;
            AllowCancel = blnAllowCancel;
        }

        public DownloadDialogSettings(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public DownloadDialogSettings(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}
         
        public bool AllowPause
        {
            get
            {
                return _blnAllowPause;
            }
            set
            {
                _blnAllowPause = value;
            }
        }

        public bool AllowCancel
        {
            get
            {
                return _blnAllowCancel;
            }
            set
            {
                _blnAllowCancel = value;
            }
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("AllowPause", AllowPause);
            objSerializedObject.Values.Add("AllowCancel", AllowCancel);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            AllowPause = objSerializedObject.Values.GetValue<bool>("AllowPause", true);
            AllowCancel = objSerializedObject.Values.GetValue<bool>("AllowCancel", true);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(AllowPause);
            objBinaryWriter.Write(AllowCancel);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            AllowPause = objBinaryReader.ReadBoolean();
            AllowCancel = objBinaryReader.ReadBoolean();
        }

        #endregion

    }
}
