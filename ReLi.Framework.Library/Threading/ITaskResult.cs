namespace ReLi.Framework.Library.Threading
{
    #region Using Declarations

    using System;
    using System.Text;

    #endregion

    public interface ITaskResult : IObjectBase 
    {
        ITask Task
        {
            get;
        }

        TaskResultType Result
        {
            get;
        }

        ITaskStats Stats
        {
            get;
        }

        string Details
        {
            get;
        }
    }
}
