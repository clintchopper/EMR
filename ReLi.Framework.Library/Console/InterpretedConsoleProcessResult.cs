namespace ReLi.Framework.Library.Console
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;

    #endregion

	public class InterpretedConsoleProcessResult
    {
        private bool _blnSuccess;
        private string _strMessage;

        public InterpretedConsoleProcessResult(bool blnSuccess)
            : this(blnSuccess, string.Empty)
        {}

        public InterpretedConsoleProcessResult(bool blnSuccess, string strMessage)
        {
            Success = blnSuccess;
            Message = strMessage;
        }

        public bool Success
        {
            get
            {
                return _blnSuccess;
            }
            private set
            {
                _blnSuccess = value;
            }
        }

        public string Message
        {
            get
            {
                return _strMessage;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Message", "A valid non-null, non-empty string is expected.");
                }

                _strMessage = value;
            }
        }      
    }
}
