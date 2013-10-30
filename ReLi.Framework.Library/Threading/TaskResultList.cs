namespace ReLi.Framework.Library.Threading
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class TaskResultList : ObjectListBase<ITaskResult>
    {
        public TaskResultList() 
            : base()
        {}
        
        public TaskResultList(IEnumerable<ITaskResult> objTaskResults)
            : base(objTaskResults)
        {}

        public TaskResultList(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public TaskResultList(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public bool ContainsResultType(TaskResultType enuResultType)
        {
            bool blnContainsResultType = false;
            foreach (ITaskResult objTaskResult in this)
            {
                if (objTaskResult.Result == enuResultType)
                {
                    blnContainsResultType = true;
                    break;
                }
            }

            return blnContainsResultType;
        }

        public ITaskResult[] GetResultsByType(TaskResultType enuResultType)
        {
            List<ITaskResult> objTaskResults = new List<ITaskResult>();
            foreach (ITaskResult objTaskResult in this)
            {
                if (objTaskResult.Result == enuResultType)
                {
                    objTaskResults.Add(objTaskResult);
                }
            }

            return objTaskResults.ToArray();
        }

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
