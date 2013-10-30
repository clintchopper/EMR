namespace ReLi.Framework.Library.Caching
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    public class CacheManager
    {
        private object _objLockObject;
        private Dictionary<string, CacheRecord> _objRecords;

        public CacheManager()
        {
            _objLockObject = new object();
        }

        protected Dictionary<string, CacheRecord> Records
        {
            get
            {
                if (_objRecords == null)
                {
                    _objRecords = new Dictionary<string, CacheRecord>();
                }

                return _objRecords;
            }
            set
            {
                _objRecords = value;
            }
        }

        public TValueType GetValue<TValueType>(string strKey)
        {
            return GetValue<TValueType>(strKey, default(TValueType));
        }

        public TValueType GetValue<TValueType>(string strKey, TValueType objDefault)
        {
            TValueType objValue = objDefault;

            if (Records.ContainsKey(strKey) == true)
            {
                CacheRecord objRecord = Records[strKey];
                if (objRecord != null)
                {
                    if (objRecord.HasExpired == true)
                    {
                        lock (_objLockObject)
                        {
                            if (Records.ContainsKey(strKey) == true)
                            {
                                Records.Remove(strKey);
                            }
                        }
                    }
                    else
                    {
                        objValue = (TValueType)objRecord.Value;
                    }
                }
            }

            return objValue;
        }

        public void SetValue(string strKey, object objValue)
        {
            SetValue(strKey, objValue);
        }

        public void SetValue(string strKey, object objValue, DateTime objExpirationDate)
        {
            lock (_objLockObject)
            {
                if (Records.ContainsKey(strKey) == true)
                {
                    Records.Remove(strKey);
                }

                CacheRecord objRecord = new CacheRecord(strKey, objValue, objExpirationDate);
                Records.Add(strKey, objRecord);
            }
        }

        public void Clear()
        {
            lock (_objLockObject)
            {
                Records.Clear();
            }
        }

        public bool ContainsKey(string strKey)
        {
            bool blnContainsKey = false;

            if (Records.ContainsKey(strKey) == true)
            {
                CacheRecord objRecord = Records[strKey];
                if (objRecord != null)
                {
                    if (objRecord.HasExpired == true)
                    {
                        lock (_objLockObject)
                        {
                            if (Records.ContainsKey(strKey) == true)
                            {
                                Records.Remove(strKey);
                            }
                        }
                    }
                    else
                    {
                        blnContainsKey = true;
                    }
                }
            }

            return blnContainsKey;
        }

        #region Static Members

        private static object _objSyncObject = new object();
        private static CacheManager _objGlobalCache = null;

        public static CacheManager Global
        {
            get
            {
                if (_objGlobalCache == null)
                {
                    lock (_objSyncObject)
                    {
                        if (_objGlobalCache == null)
                        {
                            _objGlobalCache = new CacheManager();
                        }
                    }
                }

                return _objGlobalCache;
            }
        }

        #endregion
    }
}
