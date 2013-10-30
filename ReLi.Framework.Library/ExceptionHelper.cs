namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;    
    using System.Text;

    #endregion
        
	public class ExceptionHelper
    {
        private Exception _objException;

        public ExceptionHelper(Exception objException)
        {
            _objException = objException;
        }

        public string GetDetailedErrorMessage()
        {
            return GetDetailedErrorMessage(string.Empty);
        }

        public string GetDetailedErrorMessage(string strMessage)
        {
            StringBuilder objErrorMessage = new StringBuilder(strMessage);

            if (_objException != null)
            {
                Exception objException = _objException;
                while (objException != null)
                {
                    objErrorMessage.AppendLine("Source:");
                    objErrorMessage.AppendLine(objException.Source);

                    objErrorMessage.AppendLine("Message:");
                    objErrorMessage.AppendLine(objException.Message);

                    objErrorMessage.AppendLine("Stack:");
                    objErrorMessage.AppendLine(objException.StackTrace);
                    objErrorMessage.AppendLine();

                    objException = objException.InnerException;
                }
            }

            return objErrorMessage.ToString();
        }
    }
}
