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
        
	public class ComputerTokenList : ObjectListBase<ComputerToken>
    {
        public ComputerTokenList()
            : base()
        {}

        public ComputerTokenList(IEnumerable<ComputerToken> objComputerTokens)
            : base(objComputerTokens)
        {}

        public ComputerTokenList(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public ComputerTokenList(BinaryReaderExtension objBinaryReader)
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
