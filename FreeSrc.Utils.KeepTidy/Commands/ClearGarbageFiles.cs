using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FreeSrc.Utils.KeepTidy.Commands
{
   /// <summary>
   /// 
   /// </summary>
   public class ClearGarbageFiles : UiCommand
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <param name="commandParameter"></param>
      /// <returns></returns>
      protected override bool doExecute(AppModel appModel, object commandParameter)
      {
         var list = appModel.garbageFiles.ToArray();

         foreach (var f in list)
         {
            if (f.isSelected)
            {
               try
               {
                  File.Delete(f.fileName);

                  appModel.garbageFiles.Remove(f);
               }
               catch (Exception x)
               {
                  this.writeLog(Log.Severity.Error, x);

                  appModel.errorMessage = x.Message;

               }
            }
         }

         return true;
      }
   }
}
