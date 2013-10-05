using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace FreeSrc.Utils.KeepTidy.Commands
{
   /// <summary>
   /// 
   /// </summary>
   public class LoadServices : UiCommand
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <param name="commandParameter"></param>
      /// <returns></returns>
      protected override bool doExecute(AppModel appModel, object commandParameter)
      {
         var allServices = ServiceController.GetServices();

         appModel.serviceInstances.Clear();

         List<ServiceInstance> instanceList = new List<ServiceInstance>();

         Regex regex = new Regex(appModel.serviceFilter);

         foreach (var s in allServices)
         {
            if (appModel.cancelProcessing)
            {
               break;
            }

            if ( regex.IsMatch(s.DisplayName) )
            {
               instanceList.Add(new ServiceInstance()
                                                   {
                                                      isSelected = true,
                                                      serviceDisplayName = s.DisplayName,
                                                      serviceName = s.ServiceName,
                                                      serviceStatus = s.Status.ToString(),
                                                   });
            }
            else
            {
               instanceList.Add(new ServiceInstance()
               {
                  isSelected = false,
                  serviceDisplayName = s.DisplayName,
                  serviceName = s.ServiceName,
                  serviceStatus = s.Status.ToString(),
               });
            }
         }

         foreach (var s in instanceList.OrderByDescending(x => x.isSelected))
         {
             try
             {
                 appModel.serviceInstances.Add(s);

                 System.Threading.Thread.Sleep(5);

                 System.Diagnostics.Debug.WriteLine(s);
             }
             catch(Exception x0)
             {
                 System.Diagnostics.Debug.WriteLine(x0);

                 throw x0;
             }
         }

         return true;
      }
   }
}
