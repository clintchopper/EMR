namespace ReLi.Framework.Library.Console
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Diagnostics;
    using System.Threading;
    using System.Collections.Generic;

    #endregion

	public class ConsoleProcess
    {
        #region Event Declarations

        public delegate void OutputReceivedDelegate(ConsoleProcessAsynchronousHandle objHandle, string strOutputLine);
        public event OutputReceivedDelegate OutputReceived;
        protected void OnOutputReceived(ConsoleProcessAsynchronousHandle objHandle, string strOutputLine)
        {
            if (OutputReceived != null)
            {
                OutputReceived(objHandle, strOutputLine);
            }
        }

        public delegate void ExecuteCompletedDelegate(ConsoleProcessAsynchronousHandle objHandle, ConsoleProcessResult objResult);
        public event ExecuteCompletedDelegate ExecuteCompleted;
        protected void OnExecuteCompleted(ConsoleProcessAsynchronousHandle objHandle, ConsoleProcessResult objResult)
        {
            if (OutputReceived != null)
            {
                ExecuteCompleted(objHandle, objResult);
            }
        }

        #endregion

        public const int DefaultTimeOut = 30;

        public ConsoleProcess()
        {}

        private ProcessStartInfo CreateProcessStartInfo(string strFileName, string strArguments)
        {
            ProcessStartInfo objStartInfo = new ProcessStartInfo();

            objStartInfo.UseShellExecute = false;
            objStartInfo.RedirectStandardOutput = true;
            objStartInfo.RedirectStandardError = true;
            objStartInfo.CreateNoWindow = true;
            objStartInfo.FileName = strFileName;
            objStartInfo.Arguments = strArguments;

            return objStartInfo;

        }

        public ConsoleProcessResult ExecuteSynchronous(string strFilePath)
        {
            return ExecuteSynchronous(strFilePath, string.Empty, DefaultTimeOut, ConsoleProcessResult.DefaultInterpreter);
        }

        public ConsoleProcessResult ExecuteSynchronous(string strFilePath, ConsoleProcessResultInterpreter objInterpreter)
        {
            return ExecuteSynchronous(strFilePath, string.Empty, DefaultTimeOut, objInterpreter);
        }

        public ConsoleProcessResult ExecuteSynchronous(string strFilePath, int intTimeOut)
        {
            return ExecuteSynchronous(strFilePath, string.Empty, intTimeOut, ConsoleProcessResult.DefaultInterpreter);
        }

        public ConsoleProcessResult ExecuteSynchronous(string strFilePath, int intTimeOut, ConsoleProcessResultInterpreter objInterpreter)
        {
            return ExecuteSynchronous(strFilePath, string.Empty, intTimeOut, objInterpreter);
        }

        public ConsoleProcessResult ExecuteSynchronous(string strFilePath, string strArguments)
        {
            return ExecuteSynchronous(strFilePath, strArguments, DefaultTimeOut, ConsoleProcessResult.DefaultInterpreter);
        }

        public ConsoleProcessResult ExecuteSynchronous(string strFilePath, string strArguments, ConsoleProcessResultInterpreter objInterpreter)
        {
            return ExecuteSynchronous(strFilePath, strArguments, DefaultTimeOut, objInterpreter);
        }

        public ConsoleProcessResult ExecuteSynchronous(string strFilePath, string strArguments, int intTimeOut)
        {
            return ExecuteSynchronous(strFilePath, strArguments, intTimeOut, ConsoleProcessResult.DefaultInterpreter);
        }

        public ConsoleProcessResult ExecuteSynchronous(string strFilePath, string strArguments, int intTimeOut, ConsoleProcessResultInterpreter objInterpreter)
        {
            ConsoleProcessResult objResult = null;

            ProcessStartInfo objStartInfo = CreateProcessStartInfo(strFilePath, strArguments);

            try
            {
                using (Process objProcess = Process.Start(objStartInfo))
                {
                    bool blnResult = objProcess.WaitForExit(intTimeOut * 1000);
                    if (blnResult == true)
                    {
                        string strOutputLine;
                        List<string> objOutputLines = new List<string>();
                        while ((strOutputLine = objProcess.StandardOutput.ReadLine()) != null)
                        {
                            objOutputLines.Add(strOutputLine);
                        }

                        objResult = new ConsoleProcessResult(strFilePath, strArguments, ConsoleProcessResultType.Completed, objProcess.ExitCode, objOutputLines, objInterpreter);
                    }
                    else
                    {
                        objResult = new ConsoleProcessResult(strFilePath, strArguments, ConsoleProcessResultType.TimedOut, -1, objInterpreter);
                    }
                }
            }
            catch (Exception objException)
            {
                objResult = new ConsoleProcessResult(strFilePath, strArguments, objException, objInterpreter);
            }

            return objResult;
        }

        public ConsoleProcessAsynchronousHandle ExecuteAsynchronous(string strFilePath)
        {
            return ExecuteAsynchronous(strFilePath, string.Empty, DefaultTimeOut, ConsoleProcessResult.DefaultInterpreter);
        }

        public ConsoleProcessAsynchronousHandle ExecuteAsynchronous(string strFilePath, ConsoleProcessResultInterpreter objInterpreter)
        {
            return ExecuteAsynchronous(strFilePath, string.Empty, DefaultTimeOut, objInterpreter);
        }

        public ConsoleProcessAsynchronousHandle ExecuteAsynchronous(string strFilePath, int intTimeOut)
        {
            return ExecuteAsynchronous(strFilePath, string.Empty, intTimeOut, ConsoleProcessResult.DefaultInterpreter);
        }

        public ConsoleProcessAsynchronousHandle ExecuteAsynchronous(string strFilePath, int intTimeOut, ConsoleProcessResultInterpreter objInterpreter)
        {
            return ExecuteAsynchronous(strFilePath, string.Empty, intTimeOut, objInterpreter);
        }

        public ConsoleProcessAsynchronousHandle ExecuteAsynchronous(string strFilePath, string strArguments)
        {
            return ExecuteAsynchronous(strFilePath, strArguments, DefaultTimeOut, ConsoleProcessResult.DefaultInterpreter);
        }

        public ConsoleProcessAsynchronousHandle ExecuteAsynchronous(string strFilePath, string strArguments, ConsoleProcessResultInterpreter objInterpreter)
        {
            return ExecuteAsynchronous(strFilePath, strArguments, DefaultTimeOut, objInterpreter);
        }

        public ConsoleProcessAsynchronousHandle ExecuteAsynchronous(string strFilePath, string strArguments, int intTimeOut)
        {
            return ExecuteAsynchronous(strFilePath, strArguments, intTimeOut, ConsoleProcessResult.DefaultInterpreter);
        }

        public ConsoleProcessAsynchronousHandle ExecuteAsynchronous(string strFilePath, string strArguments, int intTimeOut, ConsoleProcessResultInterpreter objInterpreter)
        {
            ParameterizedThreadStart objParameterizedThreadStart = new ParameterizedThreadStart(ExecuteAsynchronousProc);
            Thread objExecuteAsynchronousThread = new Thread(objParameterizedThreadStart);
            objExecuteAsynchronousThread.IsBackground = true;

            ConsoleProcessAsynchronousHandle objHandle = new ConsoleProcessAsynchronousHandle(strFilePath, strArguments, intTimeOut, objInterpreter);
            objExecuteAsynchronousThread.Start(objHandle);

            return objHandle;
        }

        private void ExecuteAsynchronousProc(object objArguments)
        {
            ConsoleProcessResult objResult = null;
            ConsoleProcessAsynchronousHandle objHandle = (ConsoleProcessAsynchronousHandle)objArguments;

            List<string> objOutputLines = new List<string>();

            ProcessStartInfo objStartInfo = CreateProcessStartInfo(objHandle.FilePath, objHandle.Arguments);
            using (Process objProcess = new Process())
            {
                /// We define the delegate like this since there is no way for us
                /// to group multiple running asynchronous processes using the traditional 
                /// method.  Defining the delegate this way will allow us to utilize the 
                /// objects tha have already been defined within this thread.
                /// 
                objProcess.OutputDataReceived += delegate(object objSender, DataReceivedEventArgs objData)
                {
                    if (String.IsNullOrEmpty(objData.Data) == false)
                    {
                        objOutputLines.Add(objData.Data);
                        OnOutputReceived(objHandle, objData.Data);
                    }
                };

                objProcess.StartInfo = objStartInfo;
                objProcess.EnableRaisingEvents = true;
                objProcess.Start();
                objProcess.BeginOutputReadLine();

                objResult = null;
                while (objProcess.HasExited == false)
                {
                    if (objHandle.Cancelled == true)
                    {
                        objProcess.Kill();
                        objResult = new ConsoleProcessResult(objHandle.FilePath, objHandle.Arguments, ConsoleProcessResultType.Cancelled, objHandle.Interpreter);
                        break;
                    }

                    TimeSpan objDuration = DateTime.Now - objProcess.StartTime;
                    if (objDuration.TotalSeconds > objHandle.TimeOut)
                    {
                        objProcess.Kill();
                        objResult = new ConsoleProcessResult(objHandle.FilePath, objHandle.Arguments, ConsoleProcessResultType.TimedOut, objHandle.Interpreter);
                        break;
                    }

                    objProcess.WaitForExit(10);
                }

                if (objResult == null)
                {
                    objResult = new ConsoleProcessResult(objHandle.FilePath, objHandle.Arguments, ConsoleProcessResultType.Completed, objProcess.ExitCode, objOutputLines, objHandle.Interpreter);
                }
            }

            OnExecuteCompleted(objHandle, objResult);
        }
    }
}
