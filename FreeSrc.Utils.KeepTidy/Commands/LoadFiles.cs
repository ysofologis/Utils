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
   public class LoadGarbageFiles : UiCommand
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <param name="commandParameter"></param>
      /// <returns></returns>
      protected override bool doExecute(AppModel appModel, object commandParameter)
      {
         List<string> fileNames = new List<string>();

         var fileList = getFileCollection(appModel);

         fileList.Clear();

         string filter = getFilter(appModel);

         string[] subFilters = filter.Split('|');

         Action<string> onLoad = (f) =>
            {
               var p = new ProjectFile() { isSelected = false, fileName = f };

               prepareFile(p);

               fileList.Add(p);
            };

            
         loadFiles(appModel, appModel.projectDirectory, filter, fileNames, onLoad, appModel.excludeFolders);

         return true;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="f"></param>
      protected virtual void prepareFile(ProjectFile f)
      {
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
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <returns></returns>
      protected virtual string getFilter(AppModel appModel)
      {
         return appModel.garbageFilter;
      }
   }
   /// <summary>
   /// 
   /// </summary>
   public class LoadCommandFiles : LoadGarbageFiles
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
      /// <summary>
      /// 
      /// </summary>
      /// <param name="appModel"></param>
      /// <returns></returns>
      protected override string getFilter(AppModel appModel)
      {
         return appModel.commandFilter;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="f"></param>
      protected override void prepareFile(ProjectFile f)
      {
         var all = Process.GetProcesses();

         foreach (var p in all)
         {
            try
            {
               if (p.MainModule.FileName.ToUpper() == f.fileName.ToUpper())
               {
                  f.processId = p.Id;

                  break;
               }
            }
            catch (Exception x)
            {
               System.Diagnostics.Debug.WriteLine(x);
            }

         }
      }
   }
}
