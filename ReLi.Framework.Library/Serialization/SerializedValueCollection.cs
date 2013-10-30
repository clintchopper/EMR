namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.Collections.Generic;
    
    #endregion

    
    public class SerializedValueCollection
    {
        private Dictionary<string, List<SerializedValue>> _objIndexedValues;

        public SerializedValueCollection()
        {
            Values = new Dictionary<string, List<SerializedValue>>();
        }

        private Dictionary<string, List<SerializedValue>> Values
        {
            get
            {
                return _objIndexedValues;
            }
            set
            {
                _objIndexedValues = value;
            }
        }

        public int Count
        {
            get
            {
                return Values.Count;
            }
        }

        public SerializedValue this[string strName]
        {
            get
            {
                if (strName == null)
                {
                    throw new ArgumentNullException("strName", "a valid non-null string is required..");
                }

                SerializedValue objSerializedValue = null;

                List<SerializedValue> objIndexedValues = null;
                if (Values.TryGetValue(strName, out objIndexedValues) == true)
                {
                    objSerializedValue = objIndexedValues[0];
                }

                return objSerializedValue;
            }
            set
            {
                if (strName == null)
                {
                    throw new ArgumentNullException("strName", "a valid non-null string is required..");
                }

                List<SerializedValue> objIndexedValues = null;
                if (Values.TryGetValue(strName, out objIndexedValues) == false)
                {
                    objIndexedValues = new List<SerializedValue>();
                    Values.Add(strName, objIndexedValues);
                }

                objIndexedValues.Add(value);
            }
        }

        public SerializedValue Add(string strName, bool objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, byte objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, byte[] objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, char objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, DateTime objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, Decimal objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, Double objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, Guid objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, Enum objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, Int16 objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, Int32 objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, Int64 objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, SByte objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, TimeSpan objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, Single objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, String objValue)
        {            
            return Add(strName, objValue, false);
        }

        public SerializedValue Add(string strName, String objValue, bool blnEncrypted)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue, blnEncrypted);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, UInt16 objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, UInt32 objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, UInt64 objValue)
        {
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public SerializedValue Add(string strName, object objValue)
        {
            SerializedValueType enuValueType = SerializedValueTypeHelper.GetTypeFromObject(objValue);
            return Add(strName, objValue, enuValueType);
        }

        public SerializedValue Add(string strName, object objValue, SerializedValueType enuValueType)
        {            
            SerializedValue objSerializedValue = new SerializedValue(strName, objValue, enuValueType);
            Add(objSerializedValue);

            return objSerializedValue;
        }

        public void Add(SerializedValue objSerializedValue)
        {
            if (objSerializedValue == null)
            {
                throw new ArgumentNullException("objSerializedValue", "A valid non-null SerializedValue is required..");
            }

            string strKey = objSerializedValue.Name;

            List<SerializedValue> objIndexedValues = null;
            if (Values.TryGetValue(strKey, out objIndexedValues) == false)
            {
                objIndexedValues = new List<SerializedValue>();
                Values.Add(strKey, objIndexedValues);
            }

            objIndexedValues.Add(objSerializedValue);
        }

        public bool Contains(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required..");
            }

            bool blnContains = Values.ContainsKey(strName);
            return blnContains;
        }

        public void Clear()
        {
            Values.Clear();
        }

        public void Remove(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required..");
            }

            if (Values.ContainsKey(strName) == true)
            {
                List<SerializedValue> objIndexedValues = Values[strName];
                objIndexedValues.Clear();
                Values.Remove(strName);
            }
        }

        public SerializedValue[] ToArray()
        {
            List<SerializedValue> objSerializedValues = new List<SerializedValue>();
            foreach (List<SerializedValue> objChildValues in Values.Values)
            {
                objSerializedValues.AddRange(objChildValues);
            }

            return objSerializedValues.ToArray();
        }

        public SerializedValue[] GetValues(string strName)
        {
            if (strName == null)
            {
                throw new ArgumentNullException("strName", "A valid non-null string is required.");
            }

            SerializedValue[] arrValues = null;
            if (Values.ContainsKey(strName) == true)
            {
                arrValues = Values[strName].ToArray(); 
            }
            else
            {
                arrValues = new SerializedValue[0];
            }

            return arrValues;        
        }

        public object GetValue(string strName, object objDefaultValue)
        {
            object objReturnValue = objDefaultValue;

            SerializedValue objSerializaedValue = this[strName];
            if (objSerializaedValue != null)
            {
                objReturnValue = objSerializaedValue.Value;
            }

            return objReturnValue;
        }

        public TObjectType GetValue<TObjectType>(string strName, TObjectType objDefaultValue)
        {
            TObjectType objReturnValue = objDefaultValue;

            SerializedValue objSerializaedValue = this[strName];
            if (objSerializaedValue != null)
            {
                objReturnValue = (TObjectType)objSerializaedValue.GetValue<TObjectType>();
            }

            return objReturnValue;
        }
    }
}
