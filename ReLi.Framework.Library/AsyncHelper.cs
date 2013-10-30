namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.Threading;
    using System.Collections.Generic;

    #endregion
    
	public class AsyncHelper
    {
        private class TargetInfo
        {            
            private Delegate _objTarget;
            private object[] _objArguments;
            private ManualResetEvent _objManualResetEvent;

            internal TargetInfo(Delegate objTarget, object[] objArguments, ManualResetEvent objManualResetEvent)
            {
                Target = objTarget;
                Arguments = objArguments;
                ManualResetEvent = objManualResetEvent;
            }

            public Delegate Target
            {
                get
                {
                    return _objTarget;
                }
                private set
                {
                    _objTarget = value;
                }
            }

            public object[] Arguments
            {
                get
                {
                    return _objArguments;
                }
                private set
                {
                    _objArguments = value;
                }
            }

            public ManualResetEvent ManualResetEvent
            {
                get
                {
                    return _objManualResetEvent;
                }
                private set
                {
                    _objManualResetEvent = value;
                }
            }
        }

        #region Static Members

        private static List<ManualResetEvent> _objResetEvents = new List<ManualResetEvent>();
        private static WaitCallback _objDynamicInvokeShim = new WaitCallback(DynamicInvokeShim);

        public static void FireAndForget(Delegate objTarget, params object[] objArguments)
        {
            ManualResetEvent objResetEvent = new ManualResetEvent(false);
            objResetEvent.Reset();

            _objResetEvents.Add(objResetEvent);
            ThreadPool.QueueUserWorkItem(_objDynamicInvokeShim, new TargetInfo(objTarget, objArguments, objResetEvent));
            
        }

        private static void DynamicInvokeShim(object objObject)
        {
            TargetInfo objTargetInfo = (TargetInfo)objObject;
            try
            {
                objTargetInfo.Target.DynamicInvoke(objTargetInfo.Arguments);
            }
            finally
            {
                objTargetInfo.ManualResetEvent.Set();
                lock (_objResetEvents)
                {
                    _objResetEvents.Remove(objTargetInfo.ManualResetEvent);
                }
            }
        }

        public static void Wait()
        {
            if ((_objResetEvents != null) && (_objResetEvents.Count > 0))
            {
                WaitHandle.WaitAll(_objResetEvents.ToArray());
            }
        }

        #endregion
    }
}
