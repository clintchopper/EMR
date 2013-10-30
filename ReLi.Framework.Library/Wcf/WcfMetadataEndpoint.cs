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
    using System.ServiceModel;
    using System.ServiceModel.Description;

    #endregion
        
    public class WcfMetadataEndpoint : ObjectBase  
    {
        private bool _blnEnabled;
        private string _strUrl;

        public WcfMetadataEndpoint()
            : this(false, string.Empty)
        {}

        public WcfMetadataEndpoint(bool blnEnabled, string strUrl)
            : base()
        {
            Enabled = blnEnabled;
            Url = strUrl;
        }

        public WcfMetadataEndpoint(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public WcfMetadataEndpoint(BinaryReaderExtension objBinaryReader)
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

        public string Url
        {
            get
            {
                return _strUrl;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Url", "A valid non-null string is required.");
                }

                _strUrl = value;
            }
        }

        public ServiceMetadataBehavior CreateBehavior()
        {
            ServiceMetadataBehavior objServiceMetadataBehavior = new ServiceMetadataBehavior();
            objServiceMetadataBehavior.HttpGetEnabled = Enabled;
            objServiceMetadataBehavior.HttpGetUrl = new Uri(Url);

            return objServiceMetadataBehavior;
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Enabled", Enabled);
            objSerializedObject.Values.Add("Url", Url);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Enabled = objSerializedObject.Values.GetValue<bool>("Enabled", false);
            Url = objSerializedObject.Values.GetValue<string>("Url", string.Empty);
        }

        #endregion       

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Enabled);
            objBinaryWriter.Write(Url);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Enabled = objBinaryReader.ReadBoolean();
            Url = objBinaryReader.ReadString();
        }

        #endregion
    }
}
