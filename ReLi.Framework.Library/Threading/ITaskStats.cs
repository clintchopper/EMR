namespace ReLi.Framework.Library.Threading
{
    #region Using Declarations

    using System;
    using System.Text;

    #endregion

    public interface ITaskStats : IObjectBase
    {
        ITask Task
        {
            get;
        }

        DateTime StartTime
        {
            get;
        }

        DateTime EndTime
        {
            get;
        }

        TimeSpan Duration
        {
            get;
        }

        string FormattedDuration
        {
            get;
        }
    }
}
