namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    
    #endregion
        
	public abstract class SerializedWrapperBase<TValueType> : ObjectBase 
    {
        private TValueType _objData;

        public SerializedWrapperBase(TValueType objData)
            : base()
        {
            Data = objData;
        }

        protected SerializedWrapperBase(SerializedObject objSerializedObject)
        {
            ReadData(objSerializedObject);
        }

        public TValueType Data
        {
            get
            {
                return _objData;
            }
            set
            {
                _objData = value;
            }
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            /// We do not need to include the base data.
            /// 
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            /// We did not serialize the base data so ignore reading it.
            /// 
        }

        #endregion             
    }
}
