namespace ReLi.Framework.Library.Console
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;

    #endregion

	public class ConsoleProcessResult
    {
        private int _intExitCode;
        private string _strFilePath;
        private string _strArguments;
        private Exception _objProcessException;
        private ConsoleProcessResultType _enuProcessResult;
        private List<string> _objOutputLines;
        private ConsoleProcessResultInterpreter _objInterpreter;

        public ConsoleProcessResult(string strFilePath, string strArguments, Exception objProcessException, ConsoleProcessResultInterpreter objInterpreter)
            : this(strFilePath, strArguments, ConsoleProcessResultType.Failed, -1, new string[] {}, objInterpreter)
        {
            ProcessException = objProcessException;
        }

        public ConsoleProcessResult(string strFilePath, string strArguments, ConsoleProcessResultType enuProcessResult, ConsoleProcessResultInterpreter objInterpreter)
            : this(strFilePath, strArguments, enuProcessResult, -1, new string[] {}, objInterpreter)
        {}

        public ConsoleProcessResult(string strFilePath, string strArguments, ConsoleProcessResultType enuProcessResult, int intExitCode, ConsoleProcessResultInterpreter objInterpreter)
            : this(strFilePath, strArguments, enuProcessResult, intExitCode, new string[] {}, objInterpreter)
        {}

        public ConsoleProcessResult(string strFilePath, string strArguments, ConsoleProcessResultType enuProcessResult, int intExitCode, IEnumerable<string> objOutputLines, ConsoleProcessResultInterpreter objInterpreter)
        { 
            FilePath = strFilePath;
            Arguments = strArguments;
            ProcessResult = enuProcessResult;
            ExitCode = intExitCode;            
            ProcessException = null;
            Interpreter = objInterpreter;
            _objOutputLines = new List<string>(objOutputLines);
        }

        public int ExitCode
        {
            get
            {
                return _intExitCode;
            }
            private set
            {
                _intExitCode = value;
            }
        }

        public ConsoleProcessResultType ProcessResult
        {
            get
            {
                return _enuProcessResult;
            }
            private set
            {
                _enuProcessResult = value;
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
                if ((value == null) || (value.Length == 0))
                {
                    throw new ArgumentOutOfRangeException("FilePath", "A valid non-null, non-empty string is expected.");
                }

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
                if (value == null)
                {
                    throw new ArgumentNullException("Arguments", "A valid non-null string is expected.");
                }

                _strArguments = value;
            }
        }

        public Exception ProcessException
        {
            get
            {
                return _objProcessException;
            }
            private set
            {
                _objProcessException = value;
            }
        }

        public string[] Output
        {
            get
            {
                return _objOutputLines.ToArray();
            }
        }

        public ConsoleProcessResultInterpreter Interpreter
        {
            get
            {
                return _objInterpreter;
            }
            set
            {
                _objInterpreter = value;
            }
        }

        public InterpretedConsoleProcessResult InterpretedResults
        {
            get
            {
                InterpretedConsoleProcessResult objResult = null;
                if (Interpreter != null)
                {
                    objResult = Interpreter(this);
                }

                return objResult;
            }
        }

        #region Static Members

        public static InterpretedConsoleProcessResult DefaultInterpreter(ConsoleProcessResult objConsoleProcessResult)
        {
            bool blnSuccess = (objConsoleProcessResult.ProcessResult == ConsoleProcessResultType.Completed);
            string strMessage = ((objConsoleProcessResult.ProcessException != null) ? objConsoleProcessResult.ProcessException.Message : string.Empty);

            InterpretedConsoleProcessResult objInterpretedResult = new InterpretedConsoleProcessResult(blnSuccess, strMessage);
            return objInterpretedResult;
        }

        #endregion

    }
}
