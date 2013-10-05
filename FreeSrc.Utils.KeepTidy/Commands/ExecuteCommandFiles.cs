using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace FreeSrc.Utils.KeepTidy.Commands
{
   /// <summary>
   /// 
   /// </summary>
   public class ExecuteCommandFiles : UiCommand
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <param name="commandParameter"></param>
      /// <returns></returns>
      protected override bool doExecute(AppModel appModel, object commandParameter)
      {
         foreach (var f in appModel.commandFiles)
         {
            if (appModel.cancelProcessing)
            {
               break;
            }

            if (f.isSelected)
            {
               executeProjectFile(appModel, f);
            }
         }

         return true;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="f"></param>
      protected void executeProjectFile(AppModel appModel, ProjectFile f)
      {
         try
         {
            Process proc = new Process();

            appModel.processingMessage = string.Format("Starting [{0}]...", f.fileName);

            proc.StartInfo = new ProcessStartInfo()
            {
               CreateNoWindow = false,
               FileName = f.fileName,
               UseShellExecute = false,
               WindowStyle = ProcessWindowStyle.Normal,
            };

            proc.Start();

            proc.WaitForInputIdle();

            f.processId = proc.Id;
            
            appModel.processingMessage = string.Format("[{0}] Started.", f.fileName);

         }
         catch (Exception x)
         {
            this.writeLog(Log.Severity.Error, x);

            appModel.processingMessage = "";
            
            appModel.errorMessage = x.Message;
            appModel.errorTrace = x.StackTrace;
         }
      }
   }
}
