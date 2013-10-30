namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;

    #endregion

	public class MessageTypePropertyChangedEventArgs : EventArgs
    {
        private MessageType _objMessageType;
        private string _strName;
        private object _objOldValue;
        private object _objNewValue;

        public MessageTypePropertyChangedEventArgs(MessageType objMessageType, string strName, object objOldValue, object objNewValue)
        {
            MessageType = objMessageType;
            Name = strName;
            OldValue = objOldValue;
            NewValue = objNewValue;
        }

        public MessageType MessageType
        {
            get
            {
                return _objMessageType;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("MessageType", "A valid non-null Exception is required.");
                }

                _objMessageType = value;
            }
        }

        public string Name
        {
            get
            {
                return _strName;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Name", "A valid non-null string is required.");
                }

                _strName = value;
            }
        }

        public object OldValue
        {
            get
            {
                return _objOldValue;
            }
            protected set
            {
                _objOldValue = value;
            }
        }

        public object NewValue
        {
            get
            {
                return _objNewValue;
            }
            protected set
            {
                _objNewValue = value;
            }
        }
    }
}
