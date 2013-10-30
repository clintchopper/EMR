namespace ReLi.Framework.Library.Console
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion

    public enum ConsoleProcessResultType
    {
        Completed = 1,
        TimedOut = 2,
        Cancelled = 3,
        Failed = 4
    }
}
