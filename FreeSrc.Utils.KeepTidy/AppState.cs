using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSrc.Utils.KeepTidy
{
   /// <summary>
   /// 
   /// </summary>
   public class AppState
   {
      /// <summary>
      /// 
      /// </summary>
      public static AppModel appModel { get; set; }
      /// <summary>
      /// 
      /// </summary>
      public static string appDirectory
      {
         get
         {
            return AppDomain.CurrentDomain.BaseDirectory;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="x"></param>
      public static void traceException(Exception x)
      {
      }
   }
}
