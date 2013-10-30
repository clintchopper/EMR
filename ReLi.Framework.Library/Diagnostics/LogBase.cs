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

    #endregion
        
	public abstract class LogBase : ObjectBase, IDisposable
    {
        private bool _blnIsDisposed;
        private bool _blnEnabled;
        private MessageTypeCollection _objMessageTypes;

        public LogBase()
            : this(new MessageTypeCollection())
        {}

        public LogBase(MessageTypeCollection objMessageTypes)
            : base()
        {
            if (objMessageTypes == null)
            {
                throw new ArgumentNullException("objMessageTypes", "A valid non-null MessageTypeCollection is required..");
            }

            _blnIsDisposed = false;
            Enabled = true;

            MessageTypes = objMessageTypes;
        }

        public LogBase(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}
        
        public LogBase(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public bool Enabled
        {
            get
            {
                return _blnEnabled;
            }
            set
            {
                _blnEnabled = value;
            }
        }

        public MessageTypeCollection MessageTypes
        {
            get
            {
                return _objMessageTypes;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("MessageTypes", "A valid non-null MessageTypeCollection is required.");
                }
                
                if (_objMessageTypes != null)
                {
                    _objMessageTypes.ItemAddedEvent -= new ReLi.Framework.Library.Collections.ItemAddedEventHandler<MessageType>(MessageTypes_ItemAddedEvent);
                    _objMessageTypes.ItemRemovedEvent -= new ReLi.Framework.Library.Collections.ItemRemovedEventHandler<MessageType>(MessageTypes_ItemRemovedEvent);
                    _objMessageTypes.MessageTypePropertyChanged -= new MessageTypePropertyChangedEventHandler(MessageTypes_MessageTypePropertyChanged);
                }

                _objMessageTypes = value;

                _objMessageTypes.ItemAddedEvent += new ReLi.Framework.Library.Collections.ItemAddedEventHandler<MessageType>(MessageTypes_ItemAddedEvent);
                _objMessageTypes.ItemRemovedEvent += new ReLi.Framework.Library.Collections.ItemRemovedEventHandler<MessageType>(MessageTypes_ItemRemovedEvent);
                _objMessageTypes.MessageTypePropertyChanged += new MessageTypePropertyChangedEventHandler(MessageTypes_MessageTypePropertyChanged);
            }
        }

        public void WriteMessage(MessageBase objMessage)
        {
            if (objMessage != null)
            {
                bool blnIsMessageSupported = MessageTypes.IsMessageSupported(objMessage);
                if (blnIsMessageSupported == true)
                {
                    try
                    {
                        OnWriteMessage(objMessage);
                    }
                    catch (Exception objException)
                    {
                        WriteMessageFailedEventArgs objArguments = new WriteMessageFailedEventArgs(objMessage, objException);
                        OnWriteMessageFailed(objArguments);
                    }
                }
            }
        }

        protected abstract void OnWriteMessage(MessageBase objMessage);

        private void MessageTypes_ItemRemovedEvent(object objSender, ReLi.Framework.Library.Collections.ItemRemovedEventArgs<MessageType> objArguments)
        {
            OnMessageTypeRemoved(new MessageTypeRemovedEventArgs(objArguments.Item));
        }

        private void MessageTypes_ItemAddedEvent(object objSender, ReLi.Framework.Library.Collections.ItemAddedEventArgs<MessageType> objArguments)
        {
            OnMessageTypeAdded(new MessageTypeAddedEventArgs(objArguments.Item));
        }

        private void MessageTypes_MessageTypePropertyChanged(object objSender, MessageTypePropertyChangedEventArgs objArguments)
        {
            OnMessageTypeChanged(objArguments);
        }

        #region Event Declarations 

        public event WriteMessageFailedHandler WriteMessageFailed;
        protected void OnWriteMessageFailed(WriteMessageFailedEventArgs objArguments)
        {
            if (base.Initializing == false)
            {
                WriteMessageFailedHandler objHandler = WriteMessageFailed;
                if (objHandler != null)
                {
                    objHandler(this, objArguments);
                }
            }
        }

        public event MessageTypeAddedEventHandler MessageTypeAdded;
        protected void OnMessageTypeAdded(MessageTypeAddedEventArgs objArguments)
        {
            if (base.Initializing == false)
            {
                MessageTypeAddedEventHandler objHandler = MessageTypeAdded;
                if (objHandler != null)
                {
                    objHandler(this, objArguments);
                }
            }
        }

        public event MessageTypeRemovedEventHandler MessageTypeRemoved;
        protected void OnMessageTypeRemoved(MessageTypeRemovedEventArgs objArguments)
        {
            if (base.Initializing == false)
            {
                MessageTypeRemovedEventHandler objHandler = MessageTypeRemoved;
                if (objHandler != null)
                {
                    objHandler(this, objArguments);
                }
            }
        }

        public event MessageTypePropertyChangedEventHandler MessageTypeChanged;
        protected void OnMessageTypeChanged(MessageTypePropertyChangedEventArgs objArguments)
        {
            if (base.Initializing == false)
            {
                MessageTypePropertyChangedEventHandler objHandler = MessageTypeChanged;
                if (objHandler != null)
                {
                    objHandler(this, objArguments);
                }
            }
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            if (_blnIsDisposed == false)
            {
                Dispose(true);
                GC.SuppressFinalize(this);

                _blnIsDisposed = true;
            }
        }

        protected virtual void Dispose(bool blnDisposing)
        {}

        #endregion

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Enabled", Enabled);
            objSerializedObject.Objects.Add("MessageTypes", MessageTypes);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Enabled = objSerializedObject.Values.GetValue<bool>("Enabled", true);

            MessageTypeCollection objMessageTypes = objSerializedObject.Objects.GetObject<MessageTypeCollection>("MessageTypes", null);
            if (objMessageTypes == null)
            {
                objMessageTypes = new MessageTypeCollection();
            }
            MessageTypes = objMessageTypes;

            _blnIsDisposed = false;
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Enabled);
            objBinaryWriter.WriteTransportableObject(MessageTypes);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Enabled = objBinaryReader.ReadBoolean();
            MessageTypes = objBinaryReader.ReadTransportableObject<MessageTypeCollection>();
        }

        #endregion
    }
}
