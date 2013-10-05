using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FreeSrc.Utils.KeepTidy.Commands
{
   /// <summary>
   /// 
   /// </summary>
   public class KillCommandFiles : UiCommand
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <param name="commandParameter"></param>
      /// <returns></returns>
      protected override bool doExecute(FreeSrc.Utils.KeepTidy.AppModel appModel, object commandParameter)
      {
         foreach (var f in appModel.commandFiles)
         {
            if (appModel.cancelProcessing)
            {
               break;
            }

            if (f.isSelected)
            {
               try
               {
                  if (f.processId > 0)
                  {
                     var p = Process.GetProcessById(f.processId);

                     if (p != null)
                     {
                        p.Kill();
                     }
                  }

               }
               catch (Exception x)
               {
                  this.writeLog(Log.Severity.Error, x);
               }

               f.processId = 0;
            }
         }

         return true;
      }
   }
}
