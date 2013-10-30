namespace ReLi.Framework.Library.Wcf
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Principal;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Diagnostics;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Net.WebServices;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Serialization;

    #endregion

    public class WcfServiceEndpoint : ObjectBase
    {
        private Type _objType;
        private IWcfBinding _objBinding;
        private string _strAddress;

        public WcfServiceEndpoint(Type objType, IWcfBinding objBinding)
            : this(objType, objBinding, string.Empty)
        {}

        public WcfServiceEndpoint(Type objType, IWcfBinding objBinding, string strAddress)
            : base()
        {
            Type = objType;
            Binding = objBinding;
            Address = strAddress;
        }

        public WcfServiceEndpoint(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public WcfServiceEndpoint(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        public Type Type
        {
            get
            {
                return _objType;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Type", "A valid non-null Type is required.");
                }

                _objType = value;
            }
        }

        public IWcfBinding Binding
        {
            get
            {
                return _objBinding;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Binding", "A valid non-null IWcfBinding is required.");
                }

                _objBinding = value;
            }
        }

        public string Address
        {
            get
            {
                return _strAddress;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Address", "A valid non-null string is required.");
                }

                _strAddress = value;
            }
        }
       
        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("AssemblyQualifiedName", Type.AssemblyQualifiedName);
            objSerializedObject.Objects.Add("Binding", Binding);            
            objSerializedObject.Values.Add("Address", Address);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            string strType = objSerializedObject.Values.GetValue<string>("AssemblyQualifiedName", null);
            if (strType != null)
            {
                Type = System.Type.GetType(strType);
            }
            else
            {
                Type = null;
            }

            Binding = objSerializedObject.Objects.GetObject<IWcfBinding>("Binding", null);
            Address = objSerializedObject.Values.GetValue<string>("Address", null);            
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Type.FullName);
            objBinaryWriter.WriteTransportableObject(Binding);
            objBinaryWriter.Write(Address);            
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            string strType = objBinaryReader.ReadString();
            Type = System.Type.GetType(strType);
  
            Binding = objBinaryReader.ReadTransportableObject<IWcfBinding>();
            Address = objBinaryReader.ReadString();           
        }

        #endregion
    }
}
