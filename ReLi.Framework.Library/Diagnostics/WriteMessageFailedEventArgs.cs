namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;

    #endregion

	public class WriteMessageFailedEventArgs : EventArgs
    {
        private Exception _objException;
        private MessageBase _objMessage;

        public WriteMessageFailedEventArgs(MessageBase objMessage, Exception objException)
        {
            Message = objMessage;
            Exception = objException;
        }

        public Exception Exception
        {
            get
            {
                return _objException;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Exception", "A valid non-null Exception is required.");
                }

                _objException = value;
            }
        }

        public MessageBase Message
        {
            get
            {
                return _objMessage;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Message", "A valid non-null MessageBase is required.");
                }

                _objMessage = value;
            }
        }
    }
}
