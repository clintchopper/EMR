namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;
    using System.Management;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Serialization;

    #endregion
    
	public class MappedDriveList : ObjectListBase<MappedDrive>
    {
        public MappedDriveList()
            : base()
        {}

        public MappedDriveList(IEnumerable<MappedDrive> objMappedDrives)
            : base(objMappedDrives)
        {}

        public MappedDriveList(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public MappedDriveList(BinaryReaderExtension objBinaryReader)
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
