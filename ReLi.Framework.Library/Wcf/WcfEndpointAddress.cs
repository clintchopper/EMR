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
        
    public class WcfEndpointAddress : ObjectBase  
    {
        private string _strAddress;

        public WcfEndpointAddress(string strAddress)
            : base()
        {
            Address = strAddress;
        }

        public WcfEndpointAddress(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public WcfEndpointAddress(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string Address
        {
            get
            {
                return _strAddress;
            }
            set
            {
                if (string.IsNullOrEmpty(value) == true)
                {
                    throw new ArgumentOutOfRangeException("Address", "A valid non-null, non-empty string is required.");
                }

                _strAddress = value;
            }
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Address", Address);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Address = objSerializedObject.Values.GetValue<string>("Address", null);
        }

        #endregion       

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Address);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Address = objBinaryReader.ReadString();
        }

        #endregion
    }
}
