namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion
        
	public class SerializedObjectCollection
    {
        private Dictionary<string, List<SerializedObject>> _objIndexedObjects;

        public SerializedObjectCollection()
        {
            _objIndexedObjects = new Dictionary<string, List<SerializedObject>>();
        }

        private Dictionary<string, List<SerializedObject>> Objects
        {
            get
            {
                return _objIndexedObjects;
            }
            set
            {
                _objIndexedObjects = value;
            }
        }

        public int Count
        {
            get
            {
                return Objects.Count;
            }
        }

        public SerializedObject this[string strName]
        {
            get
            {
                if (strName == null)
                {
                    throw new ArgumentNullException("strName", "a valid non-null string is required..");
                }

                SerializedObject objSerializedObject = null;

                List<SerializedObject> objIndexedObjects = null;
                if (Objects.TryGetValue(strName, out objIndexedObjects) == true)
                {
                    objSerializedObject = objIndexedObjects[0];
                }

                return objSerializedObject;
            }
            set
            {
                if (strName == null)
                {
                    throw new ArgumentNullException("strName", "a valid non-null string is required..");
                }

                List<SerializedObject> objIndexedObjects = null;
                if (Objects.TryGetValue(strName, out objIndexedObjects) == false)
                {
                    objIndexedObjects = new List<SerializedObject>();
                    Objects.Add(strName, objIndexedObjects);
                }

                objIndexedObjects.Add(value);
            }
        }

        public SerializedObject Add(string strName)
        {
            return Add(strName, new SerializedTypeInfo());
        }

        public SerializedObject Add(string strName, SerializedTypeInfo objTypeInfo)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "a valid non-null string is required..");
            }
            if (objTypeInfo == null)
            {
                throw new ArgumentNullException("objTypeInfo", "a valid non-null SerializedTypeInfo is required..");
            }

            SerializedObject objObject = new SerializedObject(strName, objTypeInfo);
            Add(objObject);

            return objObject;
        }

        public SerializedObject Add(string strName, ICustomSerializer objCustomSerializer)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required..");
            }

            SerializedObject objSerializedObject = null;
            if (objCustomSerializer != null)
            {
                objSerializedObject = objCustomSerializer.Serialize(strName);
                Add(objSerializedObject);
            }

            return objSerializedObject;
        }

        public void Add(SerializedObject objSerializedObject)
        {
            if (objSerializedObject == null)
            {
                throw new ArgumentNullException("objSerializedObject", "A valid non-null SerializedObject is required..");
            }

            string strKey = objSerializedObject.Name;

            List<SerializedObject> objIndexedObjects = null;
            if (Objects.TryGetValue(strKey, out objIndexedObjects) == false)
            {
                objIndexedObjects = new List<SerializedObject>();
                Objects.Add(strKey, objIndexedObjects);
            }

            objIndexedObjects.Add(objSerializedObject);
        }
        
        public bool Contains(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required..");
            }

            bool blnContains = Objects.ContainsKey(strName);
            return blnContains;
        }

        public void Clear()
        {
            Objects.Clear();
        }

        public void Remove(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required..");
            }

            if (Objects.ContainsKey(strName) == true)
            {
                List<SerializedObject> objIndexedObjects = Objects[strName];
                objIndexedObjects.Clear();
                Objects.Remove(strName);
            }
        }

        public SerializedObject[] ToArray()
        {
            List<SerializedObject> objSerializedObjects = new List<SerializedObject>();
            foreach(List<SerializedObject> objChildObjects in Objects.Values)
            {
                objSerializedObjects.AddRange(objChildObjects);
            }

            return objSerializedObjects.ToArray();
        }

        public SerializedObject[] GetValues(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required..");
            }

            SerializedObject[] arrObjects = null;
            if (Objects.ContainsKey(strName) == true)
            {
                arrObjects = Objects[strName].ToArray();
            }
            else
            {
                arrObjects = new SerializedObject[0];
            }

            return arrObjects;
        }

        public TObjectType GetObject<TObjectType>(string strName, TObjectType objDefaultValue)
        {
            TObjectType objReturnValue = objDefaultValue;

            SerializedObject objSerializedObject = this[strName];
            if (objSerializedObject != null)
            {
                ICustomSerializer objCustomSerializer = objSerializedObject.Deserialize();

                if (objCustomSerializer is SerializedWrapperBase<TObjectType>)
                {
                    SerializedWrapperBase<TObjectType> objSerializedWrapper = (SerializedWrapperBase<TObjectType>)objCustomSerializer;
                    objReturnValue = (TObjectType)objSerializedWrapper.Data;
                }
                else
                {
                    objReturnValue = (TObjectType)((object)objCustomSerializer);
                }
            }

            return objReturnValue;
        }
     }
}
