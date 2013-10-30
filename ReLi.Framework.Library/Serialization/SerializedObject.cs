namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Reflection;
    
    #endregion
        
	public class SerializedObject 
    {
        private string _strName;
        private SerializedTypeInfo _objTypeInfo;
        private SerializedValueCollection _objValues;
        private SerializedObjectCollection _objObjects;

        public SerializedObject(string strName)
            : this(strName, new SerializedTypeInfo())
        {}

        public SerializedObject(string strName, SerializedTypeInfo objTypeInfo)
        {
            Name = strName;
            TypeInfo = objTypeInfo;
            Objects = new SerializedObjectCollection();
            Values = new SerializedValueCollection();
        }
         
        public string Name
        {
            get
            {
                return _strName;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Name", "A valid non-null string is required..");
                }
                string strFormattedName = value.Trim();
                if (strFormattedName.Length == 0)
                {
                    throw new ArgumentOutOfRangeException("Name", "A valid non-empty string is required..");
                }


                _strName = value;
            }
        }

        public SerializedTypeInfo TypeInfo
        {
            get
            {
                return _objTypeInfo;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("TypeInfo", "A valid non-null SerializedTypeInfo is required..");
                }

                _objTypeInfo = value;
            }
        }

        public SerializedValueCollection Values
        {
            get
            {
                return _objValues;
            }
            private set
            {
                _objValues = value;
            }
        }

        public SerializedObjectCollection Objects
        {
            get
            {
                return _objObjects;
            }
            private set
            {
                _objObjects = value;
            }

        }

        public void Clear()
        {
            Objects.Clear();
            Values.Clear();
        }

        public ICustomSerializer Deserialize()
        {
            string strAssemblyName = TypeInfo.AssemblyName;
            string strTypeName = TypeInfo.TypeName;

            Assembly objAssembly = null;
            try
            {
                objAssembly = Assembly.Load(strAssemblyName);
            }
            catch (Exception objException)
            {
                string strErrorMessage = "An error was encountered while loading the assembly - Assembly.Load('" + strAssemblyName + "'):\n";
                throw new Exception(strErrorMessage, objException);
            }

            Type objType = null;
            try
            {
                objType = objAssembly.GetType(strTypeName, true, true);
            }
            catch (Exception objException)
            {
                string strErrorMessage = "An error was encountered while loading the type - objAssemblyName.GetType('" + strTypeName + "', True, True):\n";
                throw new Exception(strErrorMessage, objException);
            }

            ICustomSerializer objCustomSerializer = null;
            try
            {
                ConstructorInfo objConstructorInfo = objType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(SerializedObject) }, null);
                objCustomSerializer = (ICustomSerializer)objConstructorInfo.Invoke(new object[] {this});
            }
            catch (Exception objException)
            {
                string strErrorMessage = "An error was encountered while creating the object - Activator.CreateInstance('" + objType.FullName + "'):\n";
                throw new Exception(strErrorMessage, objException);
            }

            return objCustomSerializer;
        }

        #region Static Members

        public static SerializedObject Serialize(string strName, ICustomSerializer objCustomSerializer)
        {
            if ((strName == null) || (strName.Length == 0))
            {
                throw new ArgumentOutOfRangeException("strName", "A valid non-null, non-empty string is required..");
            }
            if (objCustomSerializer == null)
            {
                throw new ArgumentException("objCustomSerializer", "A valid non-null ICustomSerializer is required..");
            }

            SerializedTypeInfo objTypeInfo = new SerializedTypeInfo(objCustomSerializer);
            SerializedObject objSerializedObject = new SerializedObject(strName, objTypeInfo);

            objCustomSerializer.WriteData(objSerializedObject);

            return objSerializedObject;
        }

        #endregion
    }
}
