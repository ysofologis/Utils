using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSrc.Utils.KeepTidy.Commands
{
   /// <summary>
   /// 
   /// </summary>
   public class SelectProjectDirectory : UiCommand
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <param name="commandParameter"></param>
      /// <returns></returns>
      protected override bool doExecute(AppModel appModel, object commandParameter)
      {
         var dialog = new System.Windows.Forms.FolderBrowserDialog();

         System.Windows.Forms.DialogResult result = dialog.ShowDialog();

         if (result == System.Windows.Forms.DialogResult.OK)
         {
            string filename = dialog.SelectedPath;

            appModel.projectDirectory = filename;
         }

         return true;
      }
   }
}
