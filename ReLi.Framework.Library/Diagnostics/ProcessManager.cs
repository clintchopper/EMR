namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations 

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Management;

    #endregion

    public class ProcessManager
    {
        #region Win32 Declarations

        private const uint MAXIMUM_ALLOWED = 0x02000000;
        private const uint NORMAL_PRIORITY_CLASS = 0x0020;

        private enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        private enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }
          
        private enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation
        }

        private enum TOKEN_ACCESS_TYPE : uint
        {
            STANDARD_RIGHTS_REQUIRED = 0x000F0000,
            STANDARD_RIGHTS_READ = 0x00020000,
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            TOKEN_DUPLICATE = 0x0002,
            TOKEN_IMPERSONATE = 0x0004,
            TOKEN_QUERY = 0x0008,
            TOKEN_QUERY_SOURCE = 0x0010,
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            TOKEN_ADJUST_GROUPS = 0x0040,
            TOKEN_ADJUST_DEFAULT = 0x0080,
            TOKEN_ADJUST_SESSIONID = 0x0100,
            TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY),
            TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID)
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }
                
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ProcessIdToSessionId(uint intProcessId, ref uint intSessionId);

        [DllImport("Advapi32.dll", SetLastError = true)]
        private static extern bool RevertToSelf();

        [DllImport("Advapi32.dll", SetLastError = true)]
        private static extern bool ImpersonateSelf(SECURITY_IMPERSONATION_LEVEL ImpersonationLevel);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenThreadToken(IntPtr ThreadHandle, TOKEN_ACCESS_TYPE DesiredAccess, bool OpenAsSelf, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", EntryPoint = "SetTokenInformation", SetLastError = true)]
        private static extern bool SetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, ref uint TokenInformation, uint TokenInformationLength);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess, IntPtr lpTokenAttributes, SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, TOKEN_TYPE TokenType, out IntPtr phNewToken);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CreateProcess(string lpApplicationName, [In] StringBuilder lpCommandLine, [In] IntPtr lpProcessAttributes, [In] IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
                
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CreateProcessAsUser(IntPtr hToken, string lpApplicationName, [In] StringBuilder lpCommandLine, [In] IntPtr lpProcessAttributes, [In] IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CreateProcessWithLogonW(String userName, String domain, String password, uint logonFlags, String applicationName, String commandLine, uint creationFlags, uint environment, String currentDirectory, ref  STARTUPINFO startupInfo, out PROCESS_INFORMATION processInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();

        [DllImport("UScan.dll")]
        private static extern bool SetPrivilege(string privilegeName, bool enablePrivilege);

        [DllImport("Kernel32.dll")]
        private static extern bool GetExitCodeProcess(IntPtr ptrProcessHandle, out uint uintExitCode);

        [DllImport("Kernel32.dll")]
        private static extern uint WaitForSingleObject(IntPtr ptrProcessHandle, uint uintMilliseconds);

        [DllImport("Kernel32.dll")]
        private static extern bool TerminateProcess(IntPtr ptrProcessHandle, uint uintExecitCode);

        private const UInt32 Infinite = 268435455;
        private const Int32 Startf_UseStdHandles = 256;
        private const Int32 StdOutputHandle = -11;
        private const Int32 StdErrorHandle = -12;

        #endregion

        #region Static Members 

        public static Process RunAs(string strApplicationPath, string strArguments, string strUserName, string strPassword)
        {
            return RunAs(strApplicationPath, strArguments, strUserName, strPassword, false);
        }

        public static Process RunAs(string strApplicationPath, string strArguments, string strUserName, string strPassword, bool blnWaitForProcess)
        {
            Process objProcess = null;
            IntPtr ptrOutputHandle = IntPtr.Zero;
            IntPtr ptrErrorHandle = IntPtr.Zero;

            try
            {
                ptrOutputHandle = Marshal.AllocHGlobal(4);
                Marshal.WriteInt32(ptrOutputHandle, StdOutputHandle);

                ptrErrorHandle = Marshal.AllocHGlobal(4);
                Marshal.WriteInt32(ptrErrorHandle, StdErrorHandle);

                STARTUPINFO objStartupInfo = new STARTUPINFO();
                objStartupInfo.lpReserved = null;
                objStartupInfo.dwFlags = objStartupInfo.dwFlags & Startf_UseStdHandles;
                objStartupInfo.hStdOutput = ptrOutputHandle;
                objStartupInfo.hStdError = ptrErrorHandle;

                PROCESS_INFORMATION objProcessInformation = new PROCESS_INFORMATION();

                string strCurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string strCommandLine = "\"" + strApplicationPath + "\" " + strArguments;

                bool blnResult = CreateProcessWithLogonW(strUserName, string.Empty, strPassword, Convert.ToUInt32(1), strApplicationPath, strCommandLine, Convert.ToUInt32(0), Convert.ToUInt32(0), strCurrentDirectory, ref objStartupInfo, out objProcessInformation);
                if (blnResult == false)
                {
                    int intWin32Error = Marshal.GetLastWin32Error();
                    throw new Exception("The call to 'CreateProcessWithLogonW' failed with the following error code : " + intWin32Error.ToString());
                }

                try
                {
                    objProcess = Process.GetProcessById(objProcessInformation.dwProcessId);
                }
                catch
                {
                    objProcess = null;
                }

                if ((blnWaitForProcess == true) && (objProcess != null))
                {
                    uint uintResult = WaitForSingleObject(ptrOutputHandle, Infinite);
                    if (uintResult != 0)
                    {
                        /// This is needed as a workaround for users that belong only to the User group.
                        /// These members do not have the necessary privs to wait for a process to complete.
                        /// 
                        while (true)
                        {
                            try
                            {
                                objProcess = Process.GetProcessById(objProcessInformation.dwProcessId);
                                System.Threading.Thread.Sleep(100);
                            }
                            catch
                            {
                                /// The process is no longer running
                                break;
                            }
                        }
                    }                    
                }

                return objProcess;
            }
            finally
            {
                if (ptrOutputHandle != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrOutputHandle);
                }
                if (ptrErrorHandle != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrErrorHandle);
                }
            }
        }

        public static string GetProcessOwner(int intProcessId)
        {
            string strOwner = string.Empty;

            string strQueryString = string.Format("Select * From Win32_Process Where ProcessID = {0}", intProcessId);
            ManagementObjectSearcher objManagementObjectSearcher = new ManagementObjectSearcher(strQueryString);
            ManagementObjectCollection objProcessList = objManagementObjectSearcher.Get();

            foreach (ManagementObject objProcess in objProcessList)
            {
                string[] strArguments = new string[] { string.Empty, string.Empty };
                int intReturnValue = Convert.ToInt32(objProcess.InvokeMethod("GetOwner", strArguments));
                if (intReturnValue == 0)
                {
                    strOwner = string.Format("{0}\\{1}", strArguments[1], strArguments[0]);
                    break;
                }
            }

            return strOwner;
        }

        #endregion
    }
}
