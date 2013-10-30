namespace ReLi.Framework.Library.Services
{ 
    #region Using Declarations

    using System;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceProcess;

    #endregion
    
    public class ServiceManager
    {
        public static void Stop(string strServiceName)
        {
            ServiceController objService = GetService(strServiceName);
            if (objService == null)
            {
                throw new Exception("The service '" + strServiceName + "' does not exist.");
            }

            if ((objService.Status != ServiceControllerStatus.Stopped) && (objService.Status != ServiceControllerStatus.StopPending))
            {
                objService.Stop();
                objService.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 30));
            }            
        }

        public static void Start(string strServiceName)
        {
            ServiceController objService = GetService(strServiceName);
            if (objService == null)
            {
                throw new Exception("The service '" + strServiceName + "' does not exist.");
            }

            if ((objService.Status != ServiceControllerStatus.Running) && (objService.Status != ServiceControllerStatus.StartPending))
            {
                objService.Start();
                objService.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));
            }
        }

        public static bool Exists(string strServiceName)
        {
            ServiceController objService = GetService(strServiceName);
            return (objService != null);
        }

        public static ServiceController GetService(string strServiceName)
        {
            if ((strServiceName == null) || (strServiceName.Length == 0))
            {
                throw new ArgumentOutOfRangeException("strServiceName", "A valid non-null, non-empty string is required.");
            }

            ServiceController objRequestedService = null;
            string strFormattedServiceName = strServiceName.ToLower();

            ServiceController[] objServices = ServiceController.GetServices();
            foreach (ServiceController objService in objServices)
            {
                if (objService.ServiceName.ToLower() == strFormattedServiceName)
                {
                    objRequestedService = objService;
                    break;
                }
            }
            
            return objRequestedService;
        }
    }
}
