namespace ReLi.Framework.Library.Collections
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Generic;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion

    public abstract class ObjectListBase<TObjectBase> : ListBase<TObjectBase>
        where TObjectBase : IObjectBase
    {
        public ObjectListBase()
            : base()
        { }

        public ObjectListBase(IEnumerable<TObjectBase> objItems)
            : base(objItems)
        { }

        public ObjectListBase(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public ObjectListBase(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            /// Do not execute the base class method.  We will handle the writing at 
            /// this level to better control performance.
            /// 
            if (base.Count > 0)
            {
                SerializedObject objChildren = objSerializedObject.Objects.Add("Items");

                SerializedObjectCollection objObjects = objChildren.Objects;
                foreach (TObjectBase objChildObject in this)
                {
                    objObjects.Add("Item", (ICustomSerializer)objChildObject);
                }
            }
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            /// Do not execute the base class method.  We will handle the reading at 
            /// this level to better control performance.
            /// 

            base.Initialize();

            SerializedObject objSerializedItems = objSerializedObject.Objects["Items"];
            if (objSerializedItems == null)
            {
                objSerializedItems = objSerializedObject.Objects["_Items"];
            }

            if (objSerializedItems != null)
            {
                foreach (SerializedObject objSerializedItem in objSerializedItems.Objects.ToArray())
                {
                    TObjectBase objChildObject = (TObjectBase)objSerializedItem.Deserialize();
                    Add(objChildObject);
                }
            }
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            /// Do not execute the base class method.  We will handle the writing at 
            /// this level to better control performance.
            /// 
            objBinaryWriter.Write(Count);

            if (Count > 0)
            {
                foreach (TObjectBase objChildObject in this)
                {
                    objBinaryWriter.WriteTransportableObject(objChildObject);
                }
            }
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            /// Do not execute the base class method.  We will handle the reading at 
            /// this level to better control performance.
            /// 
            Initialize();

            int intCount = objBinaryReader.ReadInt32();
            for (int intIndex = 0; intIndex < intCount; intIndex++)
            {
                TObjectBase objChildObject = objBinaryReader.ReadTransportableObject<TObjectBase>();
                Add(objChildObject);
            }
        }

        #endregion
    }
}
