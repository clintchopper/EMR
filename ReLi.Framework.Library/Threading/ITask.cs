namespace ReLi.Framework.Library.Threading
{
    #region Using Declarations

    using System;
    using System.Text;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.Serialization;

    #endregion

    public interface ITask : IObjectBase 
    {
        ITaskResult Execute(JobTicket objJobTicket);
    }
}
