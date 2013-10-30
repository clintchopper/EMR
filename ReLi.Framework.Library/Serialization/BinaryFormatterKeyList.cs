namespace ReLi.Framework.Library.Serialization
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion

    public class BinaryFormatterKeyList 
    {
        private const int KeyBlocks = 50;
        private const int MinAsciiCode = 97;
        private const int MaxAsciiCode = 122;
        private const int AsciiRange = (MaxAsciiCode - MinAsciiCode) + 1;

        private Dictionary<string, string> _objListByKey;
        private Dictionary<string, string> _objListByValue;
        private List<string> _objKeys;

        public BinaryFormatterKeyList()
        {
            ListByKey = new Dictionary<string, string>();
            ListByValue = new Dictionary<string, string>();
            
            _objKeys = new List<string>();
        }

        private Dictionary<string, string> ListByKey
        {
            get
            {
                return _objListByKey;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ListByKey", "A valid non-null Dictionary<string, string> is required.");
                }

                _objListByKey = value;
            }
        }

        private Dictionary<string, string> ListByValue
        {
            get
            {
                return _objListByValue;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ListByValue", "A valid non-null Dictionary<string, string> is required.");
                }

                _objListByValue = value;
            }
        }

        public void Clear()
        {
            ListByKey.Clear();
            ListByValue.Clear();
        }

        public void Add(string strKey, string strValue)
        {
            ListByKey[strKey] = strValue;
            ListByValue[strValue] = strKey;
        }

        public void RemoveByKey(string strKey)
        {
            string strValue = FindValueByKey(strKey);
            if (strValue.Length > 0)
            {
                ListByKey.Remove(strKey);
                ListByValue.Remove(strValue);
            }
        }

        public void RemoveByValue(string strValue)
        {
            string strKey = FindKeyByValue(strValue);
            if (strKey.Length > 0)
            {
                ListByKey.Remove(strKey);
                ListByValue.Remove(strValue);
            }
        }

        public KeyValuePair<string, string>[] GetKeyValuePairs()
        {
            List<KeyValuePair<string, string>> objList = new List<KeyValuePair<string, string>>();
            foreach (string strKey in ListByKey.Keys)
            {
                objList.Add(new KeyValuePair<string, string>(strKey, ListByKey[strKey]));
            }

            return objList.ToArray();
        }

        public string GetKey(string strValue)
        {
            string strKey = FindKeyByValue(strValue);
            if (strKey.Length == 0)
            {
                int intIndex = _objListByKey.Count + 1;
                if (intIndex > _objKeys.Count)
                {
                    GenerateKeys(_objKeys);
                }
                strKey = _objKeys[intIndex - 1];

                ListByKey.Add(strKey, strValue);
                ListByValue.Add(strValue, strKey);
            }

            return strKey;
        }

        public string FindValueByKey(string strKey)
        {
            string strValue = string.Empty;
            if (ListByKey.TryGetValue(strKey, out strValue) == false)
            {
                strValue = string.Empty;
            }

            return strValue;
        }

        public string FindKeyByValue(string strValue)
        {
            string strKey = string.Empty;
            if (ListByValue.TryGetValue(strValue, out strKey) == false)
            {
                strKey = string.Empty;
            }

            return strKey;
        }
        
        private void GenerateKeys(List<string> objKeys)
        {
            int intStartIndex = objKeys.Count + 1;
            int intEndIndex = intStartIndex + KeyBlocks;

            for (int intIndex = intStartIndex; intIndex < intEndIndex; intIndex++)
            {
                int intAsciiValue = (intIndex % AsciiRange);
                if (intAsciiValue == 0)
                {
                    intAsciiValue = MaxAsciiCode;
                }
                else
                {
                    intAsciiValue += MinAsciiCode - 1;
                }

                string strKey = Convert.ToChar(intAsciiValue).ToString();

                int intPadding = ((intIndex - 1) / AsciiRange);
                string strPadding = string.Empty.PadRight(intPadding, Convert.ToChar(MaxAsciiCode));
                strKey += strPadding;

                objKeys.Add(strKey);
            }

        }
    }
}
