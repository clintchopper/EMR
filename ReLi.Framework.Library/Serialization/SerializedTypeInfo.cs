namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;

    #endregion
        
    public class SerializedTypeInfo
    {
        private string _strTypeName;
        private string _strAssemblyName;

        public SerializedTypeInfo()
            : this(string.Empty, string.Empty)
        {}

        public SerializedTypeInfo(string strTypeName, string strAssemblyName)
        {
            TypeName = strTypeName;
            AssemblyName = strAssemblyName;
        }

        public SerializedTypeInfo(Type objType)
        {
            if (objType == null)
            {
                throw new ArgumentNullException("objType", "A valid non-null Type is required..");
            }

            TypeName = objType.FullName;
            AssemblyName = objType.Assembly.FullName;
        }

        public SerializedTypeInfo(object objObject)
        {
            if (objObject == null)
            {
                throw new ArgumentNullException("objObject", "A valid non-null object is required..");
            }

            Type objType = objObject.GetType();
            TypeName = objType.FullName;
            AssemblyName = objType.Assembly.FullName;
        }

        public string TypeName
        {
            get
            {
                return _strTypeName;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("TypeName", "A valid non-null string is required..");
                }

                _strTypeName = value;
            }
        }

        public string AssemblyName
        {
            get
            {
                return _strAssemblyName;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("AssemblyName", "A valid non-null string is required..");
                }

                _strAssemblyName = value;
            }
        }

        public override string ToString()
        {
            string strValue = string.Format("{0}.{1}", AssemblyName, TypeName);
            return strValue;
        }

    }
}
