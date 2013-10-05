using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSrc.Utils.KeepTidy.Commands
{
   /// <summary>
   /// 
   /// </summary>
   public class SelectGarbageFiles : UiCommand
   {
      /// <summary>
      /// 
      /// </summary>
      public bool selectAll { get; set; }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <param name="commandParameter"></param>
      /// <returns></returns>
      protected override bool doExecute(FreeSrc.Utils.KeepTidy.AppModel appModel, object commandParameter)
      {
         foreach (var f in this.getFileCollection(appModel))
         {
            f.isSelected = this.selectAll;
         }

         return true;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <returns></returns>
      protected virtual FileCollection getFileCollection(AppModel appModel)
      {
         return appModel.garbageFiles;
      }
   }
   /// <summary>
   /// 
   /// </summary>
   public class SelectCommandFiles : SelectGarbageFiles
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <returns></returns>
      protected override FileCollection getFileCollection(AppModel appModel)
      {
         return appModel.commandFiles;
      }
   }
   /// <summary>
   /// 
   /// </summary>
   public class SelectServiceInstances : UiCommand
   {
      /// <summary>
      /// 
      /// </summary>
      public bool selectAll { get; set; }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <param name="commandParameter"></param>
      /// <returns></returns>
      protected override bool doExecute(AppModel appModel, object commandParameter)
      {
         foreach (var s in appModel.serviceInstances)
         {
            s.isSelected = this.selectAll;
         }

         return true;
      }
   }
}
