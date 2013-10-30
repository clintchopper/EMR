namespace ReLi.Framework.Library.Caching
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    public class CacheRecord
    {
        private string _strKey;
        private DateTime _objExpirationDate;
        private object _objValue;

        public CacheRecord(string strKey, object objValue)
            : this(strKey, objValue, DateTime.MaxValue)
        {}

        public CacheRecord(string strKey, object objValue, DateTime objExpirationDate)
        {
            Key = strKey;
            Value = objValue;
            ExpiratationDate = objExpirationDate;
        }

        public string Key
        {
            get
            {
                return _strKey;
            }
            private set
            {
                if (string.IsNullOrEmpty(value) == true)
                {
                    throw new ArgumentOutOfRangeException("Key", "A valKey non-null, non-empty string is required.");
                }

                _strKey = value;
            }
        }

        public object Value
        {
            get
            {
                return _objValue;
            }
            set
            {
                _objValue = value;
            }
        }

        public DateTime ExpiratationDate
        {
            get
            {
                return _objExpirationDate;
            }
            set
            {
                _objExpirationDate = value;
            }
        }

        public bool HasExpired
        {
            get
            {
                return ExpiratationDate <= DateTime.Now;
            }
        }
    }
}
