namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Reflection;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class LogBaseCollection : ObjectListBase<LogBase>
    {
        public LogBaseCollection()
            : base()
        {}
        
        public LogBaseCollection(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}
        
        public LogBaseCollection(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);
        }

        #endregion

    }
}
