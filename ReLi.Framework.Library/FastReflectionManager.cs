namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Reflection;
    using ReLi.Framework.Library.Serialization;

    #endregion

    public class FastReflectionManager
    {
        public delegate TObjectType ActivatorDelegate<TObjectType>(params object[] objArguments);
        public delegate object FunctionDelegate(object objInstance, params object[] objArguments);
        public delegate void VoidDelegate(object objInstance, params object[] objArguments);

        #region CacheKey Definition

        private class CacheKey
        {
            private int _intHashCode = -1;
            private Type _objType;
            private string _strMethodName;
            private Type[] _objArgumentTypes;

            public CacheKey(Type objType, Type[] objArgumentTypes)
                : this(objType, string.Empty, objArgumentTypes)
            { }

            public CacheKey(Type objType, string strMethodName, Type[] objArgumentTypes)
            {
                Type = objType;
                MethodName = strMethodName;
                ArgumentTypes = objArgumentTypes;
            }

            public Type Type
            {
                get
                {
                    return _objType;
                }
                protected set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("Type", "A valid non-null Type is required.");
                    }

                    _objType = value;
                }
            }

            public string MethodName
            {
                get
                {
                    return _strMethodName;
                }
                protected set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("MethodName", "A valid non-null string is required.");
                    }

                    _strMethodName = value;
                }
            }

            public Type[] ArgumentTypes
            {
                get
                {
                    return _objArgumentTypes;
                }
                protected set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("ArgumentTypes", "A valid non-null Type[] is required.");
                    }

                    _objArgumentTypes = value;
                }
            }

            public override int GetHashCode()
            {
                if (_intHashCode == -1)
                {
                    _intHashCode = Type.GetHashCode() ^ MethodName.GetHashCode();
                    foreach (Type objType in ArgumentTypes)
                    {
                        _intHashCode ^= objType.FullName.GetHashCode();
                    }
                }

                return _intHashCode;
            }

            public override bool Equals(object obj)
            {
                bool blnEqual = false;

                if (obj is CacheKey)
                {
                    CacheKey objCacheKey = obj as CacheKey;
                    blnEqual = ((objCacheKey.Type.FullName == this.Type.FullName) && (objCacheKey.MethodName == this.MethodName));
                    if (blnEqual == true)
                    {
                        if (objCacheKey.ArgumentTypes.Length != this.ArgumentTypes.Length)
                        {
                            blnEqual = false;
                        }
                        else
                        {
                            int intTotalArgumentTypes = this.ArgumentTypes.Length;
                            for (int intIndex = 0; intIndex < intTotalArgumentTypes - 1; intIndex++)
                            {
                                if (objCacheKey.ArgumentTypes[intIndex].FullName != this.ArgumentTypes[intIndex].FullName)
                                {
                                    blnEqual = false;
                                    break;
                                }
                            }
                        }
                    }

                }

                return blnEqual;
            }
        }

        #endregion

        private object _objSyncObject = new object();
        private Dictionary<CacheKey, Delegate> _objDelegateCache;
        private Dictionary<string, Type> _objTypeCache;

        private FastReflectionManager()
        {
            _objDelegateCache = new Dictionary<CacheKey, Delegate>();
            _objTypeCache = new Dictionary<string,Type>();
        }

        public Delegate CreateMethodDelegate(object objInstance, string strMethodName, Type[] objArgumentTypes)
        {
            Delegate objDelegate = null;
            Type objType = objInstance.GetType();

            CacheKey objCacheKey = new CacheKey(objType, strMethodName, objArgumentTypes);
            if (_objDelegateCache.ContainsKey(objCacheKey) == true)
            {
                objDelegate = _objDelegateCache[objCacheKey];
            }
            else
            {
                MethodInfo objMethodInfo = objType.GetMethod(strMethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, null, objArgumentTypes, null);
                ParameterExpression objInstanceParameter = Expression.Parameter(typeof(object), "target");
                ParameterExpression objArgumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

                ParameterInfo[] objParameterInfo = objMethodInfo.GetParameters();
                Expression[] objArgumentsExpressions = new Expression[objParameterInfo.Length];

                int intParameterCount = objParameterInfo.Length;
                for (int intIndex = 0; intIndex < intParameterCount; intIndex++)
                {
                    Expression objExpressionIndex = Expression.Constant(intIndex);
                    Type objParameterType = objParameterInfo[intIndex].ParameterType;

                    Expression objParameterAccessorExpression = Expression.ArrayIndex(objArgumentsParameter, objExpressionIndex);
                    Expression objParameterCastExpression = Expression.Convert(objParameterAccessorExpression, objParameterType);

                    objArgumentsExpressions[intIndex] = objParameterCastExpression;
                }

                MethodCallExpression objMethodCall = Expression.Call(Expression.Convert(objInstanceParameter, objMethodInfo.DeclaringType), objMethodInfo, objArgumentsExpressions);
                if (objMethodInfo.ReturnType == typeof(void))
                {
                    objDelegate = Expression.Lambda<VoidDelegate>(objMethodCall, objInstanceParameter, objArgumentsParameter).Compile();
                }
                else
                {
                    objDelegate = Expression.Lambda<FunctionDelegate>(Expression.Convert(objMethodCall, typeof(object)), objInstanceParameter, objArgumentsParameter).Compile();
                }

                lock (_objSyncObject)
                {
                    if (_objDelegateCache.ContainsKey(objCacheKey) == false)
                    {
                        _objDelegateCache.Add(objCacheKey, objDelegate);
                    }
                }
            }

            return objDelegate;
        }

        public ActivatorDelegate<TObjectType> CreateActivatorDelegate<TObjectType>(Type objType, Type[] objArgumentTypes)
        {
            ActivatorDelegate<TObjectType> objActivator = null;
            CacheKey objCacheKey = new CacheKey(objType, objArgumentTypes);
            if (_objDelegateCache.ContainsKey(objCacheKey) == true)
            {
                try
                {
                    objActivator = (ActivatorDelegate<TObjectType>)_objDelegateCache[objCacheKey];
                }
                catch
                {
                    objActivator = null;
                }
            }
            if (objActivator == null)
            {
                ConstructorInfo objConstructorInfo = objType.GetConstructor(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, null, objArgumentTypes, null );
                ParameterInfo[] objParameterInfo = objConstructorInfo.GetParameters();

                ParameterExpression objParameterExpression = Expression.Parameter(typeof(object[]), "objArguments");
                Expression[] objArgumentsExpressions = new Expression[objParameterInfo.Length];

                int intParameterCount = objParameterInfo.Length;
                for (int intIndex = 0; intIndex < intParameterCount; intIndex++)
                {
                    Expression objExpressionIndex = Expression.Constant(intIndex);
                    Type objParameterType = objParameterInfo[intIndex].ParameterType;

                    Expression objParameterAccessorExpression = Expression.ArrayIndex(objParameterExpression, objExpressionIndex);
                    Expression objParameterCastExpression = Expression.Convert(objParameterAccessorExpression, objParameterType);

                    objArgumentsExpressions[intIndex] = objParameterCastExpression;
                }

                NewExpression objNewExpression = Expression.New(objConstructorInfo, objArgumentsExpressions);
                LambdaExpression objLambdaExpression = Expression.Lambda(typeof(ActivatorDelegate<TObjectType>), objNewExpression, objParameterExpression);

                objActivator = (ActivatorDelegate<TObjectType>)objLambdaExpression.Compile();

                lock (_objSyncObject)
                {
                    if (_objDelegateCache.ContainsKey(objCacheKey) == false)
                    {
                        _objDelegateCache.Add(objCacheKey, objActivator);
                    }
                }
            }

            return objActivator;
        }

        public ActivatorDelegate<TObjectType> CreateActivatorDelegate<TObjectType>(SerializedTypeInfo objTypeInfo, Type[] objArgumentTypes)
        {
            Type objType = null;

            string strTypeKey = string.Format("{0}.{1}", objTypeInfo.AssemblyName, objTypeInfo.TypeName);
            if (_objTypeCache.ContainsKey(strTypeKey) == false)
            {
                Assembly objAssembly = null;
                try
                {
                    objAssembly = Assembly.Load(objTypeInfo.AssemblyName);
                }
                catch (Exception objException)
                {
                    string strErrorMessage = "An error was encountered while loading the assembly - Assembly.Load('" + objTypeInfo.AssemblyName + "'):\n";
                    throw new Exception(strErrorMessage, objException);
                }

                try
                {
                    objType = objAssembly.GetType(objTypeInfo.TypeName, true, true);
                }
                catch (Exception objException)
                {
                    string strErrorMessage = "An error was encountered while loading the type - objAssemblyName.GetType('" + objTypeInfo.TypeName + "', True, True):\n";
                    throw new Exception(strErrorMessage, objException);
                }

                _objTypeCache.Add(strTypeKey, objType);
            }
            else
            {
                objType = _objTypeCache[strTypeKey];
            }

            return CreateActivatorDelegate<TObjectType>(objType, objArgumentTypes);
        }

        #region Static Members

        private static FastReflectionManager _objInstance;
        private static object _objLockObject = new object();

        private static FastReflectionManager Instance
        {
            get
            {
                if (_objInstance == null)
                {
                    lock (_objLockObject)
                    {
                        if (_objInstance == null)
                        {
                            _objInstance = new FastReflectionManager();
                        }
                    }
                }

                return _objInstance;
            }
        }
        
        public static object InvokeFunction(object objInstance, string strMethodName, params object[] objArguments)
        {
            Type[] objArgumentTypes = new Type[objArguments.Length];
            for (int intIndex = 0; intIndex < objArgumentTypes.Length; intIndex++)
            {
                object objArgument = objArguments[intIndex];
                if (objArgument != null)
                {
                    objArgumentTypes[intIndex] = objArgument.GetType();
                }
            }

            return InvokeFunction(objInstance, strMethodName, objArgumentTypes, objArguments);
        }

        public static object InvokeFunction(object objInstance, string strMethodName, Type[] objArgumentTypes, params object[] objArguments)
        {
            object objReturnValue = null;
            Delegate objMethodDelegate = Instance.CreateMethodDelegate(objInstance, strMethodName, objArgumentTypes);

            if (objMethodDelegate is FunctionDelegate)
            {
                FunctionDelegate objFunctionDelegate = objMethodDelegate as FunctionDelegate;
                objReturnValue = objFunctionDelegate(objInstance, objArguments);
            }
            else
            {
                VoidDelegate objVoidDelegate = objMethodDelegate as VoidDelegate;
                objVoidDelegate(objInstance, objArguments);
            }

            return objReturnValue;
        }

        public static TObjectType CreateInstance<TObjectType>(SerializedTypeInfo objTypeInfo, Type[] objArgumentTypes, params object[] objArguments)
        {
            ActivatorDelegate<TObjectType> objActivator = Instance.CreateActivatorDelegate<TObjectType>(objTypeInfo, objArgumentTypes);
            return objActivator(objArguments);
        }

        public static TObjectType CreateInstance<TObjectType>(Type objType, Type[] objArgumentTypes, params object[] objArguments)
        {
            ActivatorDelegate<TObjectType> objActivator = Instance.CreateActivatorDelegate<TObjectType>(objType, objArgumentTypes);
            return objActivator(objArguments);
        }

        #endregion
    }
}
