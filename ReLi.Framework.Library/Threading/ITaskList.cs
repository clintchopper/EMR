namespace ReLi.Framework.Library.Threading
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class ITaskList : ObjectListBase<ITask>
    {
        public ITaskList() 
            : base()
        {}

        public ITaskList(IEnumerable<ITask> objItems)
            : base(objItems)
        {}

        public ITaskList(SerializedObject objSerializedObject)
            : base(objSerializedObject)
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
    }
}
