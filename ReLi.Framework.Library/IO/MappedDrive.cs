namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Security;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Console;

    #endregion
        
	public class MappedDrive : ObjectBase 
    {
        private string _strDriveLetter;
        private string _strPath;
        private Credentials _objCredentials;

        public MappedDrive(string strDriveLetter, string strPath, Credentials objCredentials)
        {
            DriveLetter = strDriveLetter;
            Path = strPath;
            Credentials = objCredentials;
        }

        public MappedDrive(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public MappedDrive(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string DriveLetter
        {
            get
            {
                return _strDriveLetter;
            }
            private set
            {
                if ((value == null) || (value.Length == 0))
                {
                    throw new ArgumentException("A valid non-null, non-empty string is expected", "DriveLetter");
                }

                _strDriveLetter = value;
            }
        }

        public string Path
        {
            get
            {
                return _strPath;
            }
            private set
            {
                if ((value == null) || (value.Length == 0))
                {
                    throw new ArgumentException("A valid non-null, non-empty string is expected", "Path");
                }

                _strPath = value;
            }
        }

        public Credentials Credentials
        {
            get
            {
                return _objCredentials;
            }
            private set
            {
                if (value == null) 
                {
                    throw new ArgumentNullException("Credentials", "A valid non-null Credentials is expected");
                }

                _objCredentials = value;
            }
        }

        public bool IsConnected
        {
            get                
            {
                bool blnIsConnected = false;

                try
                {
                    DriveInfo objDriveInfo = new DriveInfo(DriveLetter);
                    blnIsConnected = objDriveInfo.IsReady;
                }
                catch
                {
                    blnIsConnected = false;
                }

                return blnIsConnected;
            }
        }

        public string Status
        {
            get
            {
                string strStatus = string.Empty;
                if (IsConnected == true)
                {
                    strStatus = "Connected";
                }
                else
                {
                    strStatus = "Disconnected";
                }

                return strStatus;
            }
        }

        public ConsoleProcessResult Connect()
        {
            return Connect(DriveLetter, Path, Credentials);
        }

        public ConsoleProcessResult Disconnect()
        {
            return Disconnect(DriveLetter);
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("DriveLetter", DriveLetter);
            objSerializedObject.Values.Add("Path", Path);
            objSerializedObject.Objects.Add("Credentials", Credentials);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            DriveLetter = objSerializedObject.Values.GetValue<string>("DriveLetter", string.Empty);
            Path = objSerializedObject.Values.GetValue<string>("Path", string.Empty);
            Credentials = objSerializedObject.Objects.GetObject<Credentials>("Credentials", null);
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(DriveLetter);
            objBinaryWriter.Write(Path);
            objBinaryWriter.WriteTransportableObject(Credentials);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            DriveLetter = objBinaryReader.ReadString();
            Path = objBinaryReader.ReadString();
            Credentials = objBinaryReader.ReadTransportableObject<Credentials>();
        }

        #endregion

        #region Static Members

        public static InterpretedConsoleProcessResult ProcessResultInterpreter(ConsoleProcessResult objConsoleProcessResult)
        {
            bool blnSuccess = false;
            string strMessage = string.Empty;

            if (objConsoleProcessResult.Output.Length > 0)
            {
                string strOutput = objConsoleProcessResult.Output[objConsoleProcessResult.Output.Length - 1].ToLower();
                if (strOutput.EndsWith("successfully.") == true)
                {
                    blnSuccess = true;
                }
                else
                {
                    strMessage = String.Join("\r\n", objConsoleProcessResult.Output);
                }
            }

            InterpretedConsoleProcessResult objInterpretedResult = new InterpretedConsoleProcessResult(blnSuccess, strMessage);
            return objInterpretedResult;
        }

        public static ConsoleProcessResult Connect(string strDriveLetter, string strPath, Credentials objCredentials)
        {
            if ((strDriveLetter == null) || (strDriveLetter.Length == 0))
            {
                throw new ArgumentOutOfRangeException("strDriveLetter", "A valid non-null, non-empty string is required.");
            }
            if ((strPath == null) || (strPath.Length == 0))
            {
                throw new ArgumentOutOfRangeException("strPath", "A valid non-null, non-empty string is required.");
            }
            if (objCredentials == null) 
            {
                throw new ArgumentNullException("objCredentials", "A valid non-null Credentials is required.");
            }

            string strArguments = "use " + strDriveLetter + ": " + strPath + " /user:" + objCredentials.FullUserName + " " + objCredentials.Password + " /persistent:no";

            ConsoleProcess objConsoleProcess = new ConsoleProcess();
            ConsoleProcessResult objResult = objConsoleProcess.ExecuteSynchronous("net", strArguments, 10, new ConsoleProcessResultInterpreter(ProcessResultInterpreter));

            return objResult;
        }

        public static ConsoleProcessResult Disconnect(string strDriveLetter)
        {
            string strArguments = "use " + strDriveLetter + ": /delete";

            ConsoleProcess objConsoleProcess = new ConsoleProcess();
            ConsoleProcessResult objResult = objConsoleProcess.ExecuteSynchronous("net", strArguments, 10, new ConsoleProcessResultInterpreter(ProcessResultInterpreter));

            return objResult;
        }

        public static string[] GetAvailableDriveLetters()
        {
            List<string> objDriveLetters = new List<string>();
            for (int intIndex = 90; intIndex >= 65; intIndex--)
            {
                objDriveLetters.Add(Convert.ToChar(intIndex).ToString().ToUpper());
            }

            DriveInfo[] objDriveInfoObjects = DriveInfo.GetDrives();
            foreach (DriveInfo objDriveInfo in objDriveInfoObjects)
            {
                string strDriveLetter = objDriveInfo.Name.Substring(0, 1).ToUpper();
                objDriveLetters.Remove(strDriveLetter);
            }

            return objDriveLetters.ToArray();
        }

        public static string GetAvailableDriveLetter()
        {
            string strAvailableDriveLetter = string.Empty;

            string[] strDriveLetters = GetAvailableDriveLetters();
            if (strDriveLetters.Length > 0)
            {
                strAvailableDriveLetter = strDriveLetters[strDriveLetters.Length - 1];
            }

            return strAvailableDriveLetter;
        }

        public static bool IsDriveLetterAvailable(string strDriveLetter)
        {
            string[] strAvailableDriveLetters = GetAvailableDriveLetters();
            List<string> objAvailableDriveLetters = new List<string>(strAvailableDriveLetters);

            bool blnIsDriveLetterAvailable = (objAvailableDriveLetters.Contains(strDriveLetter) == true);
            return blnIsDriveLetterAvailable;
        }

        #endregion
    }
}
