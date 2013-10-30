namespace ReLi.Framework.Library.Console
{
    #region Using Declarations

    using System;
    using System.Text;

    #endregion

	public class ConsoleProcessAsynchronousHandle
    {
        private int _intTimeOut = ConsoleProcess.DefaultTimeOut;
        private bool _blnCancelled;
        private string _strFilePath;
        private string _strArguments;
        private string _strHandleUid;
        private ConsoleProcessResultInterpreter _objInterpreter;

        public ConsoleProcessAsynchronousHandle(string strFilePath, ConsoleProcessResultInterpreter objInterpreter)
            : this(strFilePath, string.Empty, ConsoleProcess.DefaultTimeOut, objInterpreter)
        {}

        public ConsoleProcessAsynchronousHandle(string strFilePath, string strArguments, ConsoleProcessResultInterpreter objInterpreter)
            : this(strFilePath, strArguments, ConsoleProcess.DefaultTimeOut, objInterpreter)
        {}

        public ConsoleProcessAsynchronousHandle(string strFilePath, int intTimeOut, ConsoleProcessResultInterpreter objInterpreter)
            : this(strFilePath, string.Empty, intTimeOut, objInterpreter)
        {}

        public ConsoleProcessAsynchronousHandle(string strFilePath, string strArguments, int intTimeOut, ConsoleProcessResultInterpreter objInterpreter)
        {
            TimeOut = intTimeOut;
            FilePath = strFilePath;
            Arguments = strArguments;
            Interpreter = objInterpreter;
            HandleUid = Guid.NewGuid().ToString();
            Cancelled = false;
        }

        public string HandleUid
        {
            get
            {
                return _strHandleUid;
            }
            private set
            {
                _strHandleUid = value;
            }
        }

        public string FilePath
        {
            get
            {
                return _strFilePath;
            }
            private set
            {
                _strFilePath = value;
            }

        }

        public string Arguments
        {
            get
            {
                return _strArguments;
            }
            private set
            {
                _strArguments = value;
            }

        }

        public int TimeOut
        {
            get
            {
                return _intTimeOut;
            }
            private set
            {
                _intTimeOut = value;
            }
        }

        public ConsoleProcessResultInterpreter Interpreter
        {
            get
            {
                return _objInterpreter;
            }
            private set
            {
                _objInterpreter = value;
            }
        }

        public bool Cancelled
        {
            get
            {
                return _blnCancelled;
            }
            private set
            {
                _blnCancelled = value;
            }

        }

        public void Cancel()
        {
            _blnCancelled = true;
        }
    }

}
