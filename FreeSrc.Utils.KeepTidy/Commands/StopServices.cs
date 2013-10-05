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
   public class StopServices : UiCommand 
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
            if (appModel.cancelProcessing)
            {
               break;
            }

            if (si.isSelected)
            {
               var s = ServiceController.GetServices().Where(x => x.ServiceName == si.serviceName).FirstOrDefault();

               if (s != null)
               {
                  if (s.Status == ServiceControllerStatus.Running)
                  {
                     try
                     {
                        appModel.processingMessage = string.Format("Stopping service '{0}'...", s.DisplayName);

                        s.Stop();

                        ThreadPool.QueueUserWorkItem((x) =>
                        {
                           StartServices.updateStatus(s, si);
                        });
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
         }

         return true;
      }
   }
}
