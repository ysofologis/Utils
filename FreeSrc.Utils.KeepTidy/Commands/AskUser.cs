using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FreeSrc.Utils.KeepTidy.Commands
{
   /// <summary>
   /// 
   /// </summary>
   public class AskUser : UiCommand
   {
      /// <summary>
      /// 
      /// </summary>
      public string promptMessage { get; set; }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <param name="commandParameter"></param>
      /// <returns></returns>
      protected override bool doExecute(FreeSrc.Utils.KeepTidy.AppModel appModel, object commandParameter)
      {
         if (MessageBox.Show(this.promptMessage, appModel.productName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
         {
            return false;
         }
         else
         {
            return true;
         }
      }
   }
}
