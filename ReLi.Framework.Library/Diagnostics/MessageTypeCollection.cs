namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;

    #endregion
        
	public class MessageTypeCollection : ObjectListBase<MessageType>
    {
        private object _objSyncObject;

        public MessageTypeCollection()
            : base()
        {}

        public MessageTypeCollection(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public MessageTypeCollection(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}
        
        private object SyncObject
        {
            get
            {
                if (_objSyncObject == null)
                {
                    _objSyncObject = new object();
                }

                return _objSyncObject;
            }          
        }

        public bool IsMessageTypeSupported<TMessageType>()
            where TMessageType : MessageBase 
        {
            bool blnSupported = false;
            string strFullName = typeof(TMessageType).FullName;

            foreach (MessageType objMessageType in this)
            {
                if ((objMessageType.FullName == strFullName) && (objMessageType.Enabled == true))
                {
                    blnSupported = true;
                    break;
                }
            }

            return blnSupported;
        }             

        public bool IsMessageSupported(MessageBase objMessageBase)
        {
            bool blnSupported = false;

            if (objMessageBase != null)
            {
                string strFullName = objMessageBase.GetType().FullName;
                foreach (MessageType objMessageType in this)
                {
                    if ((objMessageType.FullName == strFullName) && (objMessageType.Enabled == true))
                    {
                        blnSupported = true;
                        break;
                    }
                }
            }

            return blnSupported;
        }

        public override void Add(MessageType objItem)
        {
            objItem.PropertyChanged += new MessageTypePropertyChangedEventHandler(objItem_PropertyChanged);
            base.Add(objItem);
        }

        public override void Insert(int intIndex, MessageType objItem)
        {
            if (objItem != null)
            {
                objItem.PropertyChanged += new MessageTypePropertyChangedEventHandler(objItem_PropertyChanged);
            }

            base.Insert(intIndex, objItem);
        }

        public override void Clear()
        {
            foreach (MessageType objItem in this)
            {
                if (objItem != null)
                {
                    objItem.PropertyChanged -= new MessageTypePropertyChangedEventHandler(objItem_PropertyChanged);
                }
            }

            base.Clear();
        }

        public override bool Remove(MessageType objItem)
        {
            if (objItem != null)
            {
                objItem.PropertyChanged -= new MessageTypePropertyChangedEventHandler(objItem_PropertyChanged);
            }

            return base.Remove(objItem);
        }

        public override void RemoveAt(int intIndex)
        {
            MessageType objItem = this[intIndex];
            if (objItem != null)
            {
                objItem.PropertyChanged -= new MessageTypePropertyChangedEventHandler(objItem_PropertyChanged);
            }

            base.RemoveAt(intIndex);
        }

        void objItem_PropertyChanged(object objSender, MessageTypePropertyChangedEventArgs objArguments)
        {
            OnMessageTypePropertyChanged(objArguments);
        }

        #region Event Declarations

        public event MessageTypePropertyChangedEventHandler MessageTypePropertyChanged;
        protected void OnMessageTypePropertyChanged(MessageTypePropertyChangedEventArgs objPropetyChangedArguments)
        {
            if (this.MessageTypePropertyChanged != null)
            {
                this.MessageTypePropertyChanged(this, objPropetyChangedArguments);
            }
        }

        #endregion

        #region SerializableObject Members

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
