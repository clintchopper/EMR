namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    #endregion

    public class RetryHelper
    {
        #region Static Members

        public static void Retry(Action objAction, int intMaximumRetries, int intDelayInMilliseconds)
        {
            int intNumberOfRetriesRemaining = intMaximumRetries;
            bool blnSucceeded = false;

            while (blnSucceeded == false)
            {
                try
                {
                    objAction();
                    blnSucceeded = true;
                }
                catch
                {
                    intNumberOfRetriesRemaining--;
                    if (intNumberOfRetriesRemaining < 0)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(intDelayInMilliseconds);
                    }
                }
            }
        }

        public static TResult Retry<TResult>(Func<TResult> objFunction, int intMaximumRetries, int intDelayInMilliseconds)
        {
            TResult objResult = default(TResult);

            int intNumberOfRetriesRemaining = intMaximumRetries;
            bool blnSucceeded = false;

            while (blnSucceeded == false)
            {
                try
                {
                    objResult = objFunction();
                    blnSucceeded = true;
                }
                catch
                {
                    intNumberOfRetriesRemaining--;
                    if (intNumberOfRetriesRemaining < 0)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(intDelayInMilliseconds);
                    }
                }
            }

            return objResult;
        }

        #endregion
    }
}
