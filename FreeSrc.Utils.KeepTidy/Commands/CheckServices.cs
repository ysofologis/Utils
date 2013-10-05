using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Threading;

namespace FreeSrc.Utils.KeepTidy.Commands
{
   /// <summary>
   /// 
   /// </summary>
   public class CheckServices : UiCommand
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <param name="commandParameter"></param>
      /// <returns></returns>
      protected override bool doExecute(AppModel appModel, object commandParameter)
      {
         foreach (var si in appModel.serviceInstances)
         {
            if (si.isSelected)
            {
               var s = ServiceController.GetServices().Where(x => x.ServiceName == si.serviceName).FirstOrDefault();

               if (s != null)
               {
                  try
                  {
                     si.serviceStatus = s.Status.ToString();
                  }
                  catch (Exception x)
                  {
                     this.writeLog(Log.Severity.Warn, x);

                     appModel.errorMessage = x.Message;
                     appModel.errorTrace = x.StackTrace;
                  }
               }
            }
         }

         return true;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="service"></param>
      /// <param name="serviceInstance"></param>
      public static void updateStatus(ServiceController service, ServiceInstance serviceInstance)
      {
         Thread.Sleep(2000);

         serviceInstance.serviceStatus = service.Status.ToString();
      }
   }
}
