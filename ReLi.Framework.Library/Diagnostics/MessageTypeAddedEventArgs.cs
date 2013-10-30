namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;

    #endregion

	public class MessageTypeAddedEventArgs : EventArgs
    {
        private MessageType _objMessageType;

        public MessageTypeAddedEventArgs(MessageType objMessageType)
        {
            MessageType = objMessageType;
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
    }
}
