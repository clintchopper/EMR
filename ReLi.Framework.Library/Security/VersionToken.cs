namespace ReLi.Framework.Library.Security
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Principal;
    using System.Collections.Generic;
    using System.Management;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Security.Hash;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;
    using ReLi.Framework.Library.Diagnostics;

    #endregion

    public class VersionToken : ObjectBase
    {
        private int _intMajor;
        private int _intMinor;
        private int _intBuild;
        private int _intRevision;
        private string _strNumber;

        public VersionToken()
            : this(0, 0, 0, 0)
        { }

        public VersionToken(string strNumber)
            : base()
        {
            if (strNumber == null)
            {
                throw new ArgumentNullException("strNumber", "A valid non-null string is required.");
            }
            if (strNumber.Length != 24)
            {
                throw new ArgumentOutOfRangeException("strNumber", strNumber, "The value must have length 24");
            }

            Number = strNumber;
            Major = System.Convert.ToInt32(strNumber.Substring(0, 6));
            Minor = System.Convert.ToInt32(strNumber.Substring(6, 6));
            Build = System.Convert.ToInt32(strNumber.Substring(12, 6));
            Revision = System.Convert.ToInt32(strNumber.Substring(18, 6));
        }

        public VersionToken(Version objVersion)
            : base()
        {
            if (objVersion == null)
            {
                throw new ArgumentNullException("objVersion", "A valid non-null Version is required.");
            }

            Build = objVersion.Build;
            Major = objVersion.Major;
            Minor = objVersion.Minor;
            Revision = objVersion.Revision;
        }

        public VersionToken(int intBuild, int intMajor, int intMinor, int intRevision)
            : base()
        {
            Build = intBuild;
            Major = intMajor;
            Minor = intMinor;
            Revision = intRevision;
        }

        public VersionToken(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        { }

        public VersionToken(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        { }

        public int Major
        {
            get
            {
                return _intMajor;
            }
            protected set
            {
                _intMajor = value;
            }
        }

        public int Minor
        {
            get
            {
                return _intMinor;
            }
            protected set
            {
                _intMinor = value;
            }
        }

        public int Build
        {
            get
            {
                return _intBuild;
            }
            protected set
            {
                _intBuild = value;
            }
        }

        public int Revision
        {
            get
            {
                return _intRevision;
            }
            protected set
            {
                _intRevision = value;
            }
        }

        public string Number
        {
            get
            {
                if (_strNumber == null)
                {
                    _strNumber = string.Format("{0:000000}", Major) + string.Format("{0:000000}", Minor) + string.Format("{0:000000}", Build) + string.Format("{0:000000}", Revision);
                }
                return _strNumber;
            }
            protected set
            {
                _strNumber = value;
            }
        }

        public string FormattedNumber
        {
            get
            {
                string strFormattedNumber = string.Format("{0:000000}", Major) + "." + string.Format("{0:000000}", Minor) + "." + string.Format("{0:000000}", Build) + "." + string.Format("{0:000000}", Revision);
                return strFormattedNumber;
            }
        }

        public override bool Equals(object objObject)
        {
            VersionToken objVersionToken1 = this;
            VersionToken objVersionToken2 = objObject as VersionToken;

            if ((objVersionToken1 == null) && (objVersionToken2 == null))
            {
                return true;
            }
            else if (objVersionToken1 == null)
            {
                return false;
            }
            else if (objVersionToken2 == null)
            {
                return false;
            }

            Version objVersion1 = new Version(objVersionToken1.Major, objVersionToken1.Minor, objVersionToken1.Build, objVersionToken1.Revision);
            Version objVersion2 = new Version(objVersionToken2.Major, objVersionToken2.Minor, objVersionToken2.Build, objVersionToken2.Revision);

            return (objVersion1.Equals(objVersion2));
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public static bool operator >(VersionToken objVersionToken1, VersionToken objVersionToken2)
        {
            if (objVersionToken1 == null)
            {
                return false;
            }
            if (objVersionToken2 == null)
            {
                return true;
            }

            Version objVersion1 = new Version(objVersionToken1.Major, objVersionToken1.Minor, objVersionToken1.Build, objVersionToken1.Revision);
            Version objVersion2 = new Version(objVersionToken2.Major, objVersionToken2.Minor, objVersionToken2.Build, objVersionToken2.Revision);

            return (objVersion1 > objVersion2);
        }

        public static bool operator >=(VersionToken objVersionToken1, VersionToken objVersionToken2)
        {
            if ((objVersionToken1 == null) && (objVersionToken2 == null))
            {
                return true;
            }
            else if (objVersionToken1 == null)
            {
                return false;
            }
            else if (objVersionToken2 == null)
            {
                return true;
            }

            Version objVersion1 = new Version(objVersionToken1.Major, objVersionToken1.Minor, objVersionToken1.Build, objVersionToken1.Revision);
            Version objVersion2 = new Version(objVersionToken2.Major, objVersionToken2.Minor, objVersionToken2.Build, objVersionToken2.Revision);

            return (objVersion1 >= objVersion2);
        }

        public static bool operator <(VersionToken objVersionToken1, VersionToken objVersionToken2)
        {
            if (objVersionToken1 == null)
            {
                return true;
            }
            if (objVersionToken2 == null)
            {
                return false;
            }

            Version objVersion1 = new Version(objVersionToken1.Major, objVersionToken1.Minor, objVersionToken1.Build, objVersionToken1.Revision);
            Version objVersion2 = new Version(objVersionToken2.Major, objVersionToken2.Minor, objVersionToken2.Build, objVersionToken2.Revision);

            return (objVersion1 < objVersion2);
        }

        public static bool operator <=(VersionToken objVersionToken1, VersionToken objVersionToken2)
        {
            if ((objVersionToken1 == null) && (objVersionToken2 == null))
            {
                return true;
            }
            else if (objVersionToken1 == null)
            {
                return true;
            }
            else if (objVersionToken2 == null)
            {
                return false;
            }

            Version objVersion1 = new Version(objVersionToken1.Major, objVersionToken1.Minor, objVersionToken1.Build, objVersionToken1.Revision);
            Version objVersion2 = new Version(objVersionToken2.Major, objVersionToken2.Minor, objVersionToken2.Build, objVersionToken2.Revision);

            return (objVersion1 <= objVersion2);
        }

        public override string ToString()
        {
            StringBuilder objString = new StringBuilder();

            objString.AppendLine("Major: " + Major.ToString());
            objString.AppendLine("Minor: " + Minor.ToString());
            objString.AppendLine("Build: " + Build.ToString());
            objString.AppendLine("Revision: " + Revision.ToString());
            objString.AppendLine("Number: " + Number);

            return objString.ToString();
        }

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Major", Major);
            objSerializedObject.Values.Add("Minor", Minor);
            objSerializedObject.Values.Add("Build", Build);
            objSerializedObject.Values.Add("Revision", Revision);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Major = objSerializedObject.Values.GetValue<int>("Major", 0);
            Minor = objSerializedObject.Values.GetValue<int>("Minor", 0);
            Build = objSerializedObject.Values.GetValue<int>("Build", 0);
            Revision = objSerializedObject.Values.GetValue<int>("Revision", 0);
            Number = null;
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Major);
            objBinaryWriter.Write(Minor);
            objBinaryWriter.Write(Build);
            objBinaryWriter.Write(Revision);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Major = objBinaryReader.ReadInt32();
            Minor = objBinaryReader.ReadInt32();
            Build = objBinaryReader.ReadInt32();
            Revision = objBinaryReader.ReadInt32();
            Number = null;
        }

        #endregion
    }
}
