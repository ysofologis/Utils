using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FreeSrc.Utils.KeepTidy
{
   /// <summary>
   /// 
   /// </summary>
   public class ProjectFile : TemplateVM
   {
      /// <summary>
      /// 
      /// </summary>
      public ProjectFile()
      {
      }
      /// <summary>
      /// 
      /// </summary>
      public string fileName
      {
         get
         {
            return createProperty<string>("fileName").currentValue;
         }

         set
         {
            createProperty<string>("fileName").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public bool isSelected
      {
         get
         {
            return createProperty<bool>("isSelected").currentValue;
         }

         set
         {
            createProperty<bool>("isSelected").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public int processId
      {
         get
         {
            return createProperty<int>("processId").currentValue;
         }

         set
         {
            createProperty<int>("processId").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      protected override void cleanupProperties()
      {
         
      }
   }
   /// <summary>
   /// 
   /// </summary>
   public class ServiceInstance : TemplateVM
   {
      /// <summary>
      /// 
      /// </summary>
      public string serviceName
      {
         get
         {
            return createProperty<string>("serviceName").currentValue;
         }

         set
         {
            createProperty<string>("serviceName").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string serviceDisplayName
      {
         get
         {
            return createProperty<string>("serviceDisplayName").currentValue;
         }

         set
         {
            createProperty<string>("serviceDisplayName").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string serviceStatus
      {
         get
         {
            return createProperty<string>("serviceStatus").currentValue;
         }

         set
         {
            createProperty<string>("serviceStatus").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public bool isSelected
      {
         get
         {
            return createProperty<bool>("isSelected").currentValue;
         }

         set
         {
            createProperty<bool>("isSelected").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      protected override void cleanupProperties()
      {
         
      }

      public override string ToString()
      {
          return string.Format(" {0} ( {1} ) --> {2}", this.serviceDisplayName, this.serviceName, this.serviceStatus);
      }
   }
   /// <summary>
   /// 
   /// </summary>
   public class FileCollection : MTObservableCollection<ProjectFile>
   {
   }
   /// <summary>
   /// 
   /// </summary>
   public class ServiceCollection : MTObservableCollection<ServiceInstance>
   {
   }
   /// <summary>
   /// 
   /// </summary>
   public class AppModel : TemplateVM
   {
      /// <summary>
      /// 
      /// </summary>
      public AppModel()
      {
         string prod = (this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0] as AssemblyProductAttribute).Product;
         string ver = (this.GetType().Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0] as AssemblyFileVersionAttribute).Version;

         this.productName = string.Format("{0} ({1})", prod, ver);

         this.processingVisibility = false;
         
         this.garbageFilter = Properties.Settings.Default.garbageFilter;
         this.excludeFolders = Properties.Settings.Default.excludeFolders;
         this.garbageFiles = new FileCollection();

         this.commandFilter = Properties.Settings.Default.commandFilter;
         this.commandFiles = new FileCollection();

         this.serviceFilter = Properties.Settings.Default.serviceFilter;
         this.serviceInstances = new ServiceCollection();

         this.isDesignMode = false;
         this.projectDirectory = Properties.Settings.Default.projectDirectory;

         lock (typeof(AppState))
         {
            if (AppState.appModel != null)
            {
               AppState.appModel.Dispose();
            }

            AppState.appModel = this;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string productName
      {
         get
         {
            return createProperty<string>("productName").currentValue;
         }
         protected set
         {
            createProperty<string>("productName").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public bool processingVisibility
      {
         get
         {
            return createProperty<bool>("processingVisibility").currentValue;
         }

         set
         {
            createProperty<bool>("processingVisibility").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string processingMessage
      {
         get
         {
            return createProperty<string>("processingMessage").currentValue;
         }

         set
         {
            createProperty<string>("processingMessage").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public bool cancelProcessing
      {
         get
         {
            return createProperty<bool>("cancelProcessing").currentValue;
         }

         set
         {
            createProperty<bool>("cancelProcessing").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public bool isDesignMode
      {
         get
         {
            return createProperty<bool>("isDesignMode").currentValue;
         }
         set
         {
            createProperty<bool>("isDesignMode").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string projectDirectory
      {
         get
         {
            return createProperty<string>("projectDirectory").currentValue;
         }

         set
         {
            createProperty<string>("projectDirectory").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string errorMessage
      {
         get
         {
            return createProperty<string>("errorMessage").currentValue;
         }

         set
         {
            createProperty<string>("errorMessage").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string errorTrace
      {
         get
         {
            return createProperty<string>("errorTrace").currentValue;
         }

         set
         {
            createProperty<string>("errorTrace").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string garbageFilter
      {
         get
         {
            return createProperty<string>("garbageFilter").currentValue;
         }

         set
         {
            createProperty<string>("garbageFilter").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string excludeFolders
      {
         get
         {
            return createProperty<string>("excludeFolders").currentValue;
         }

         set
         {
            createProperty<string>("excludeFolders").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public FileCollection garbageFiles
      {
         get
         {
            return createProperty<FileCollection>("garbageFiles").currentValue;
         }

         set
         {
            createProperty<FileCollection>("garbageFiles").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string commandFilter
      {
         get
         {
            return createProperty<string>("commandFilter").currentValue;
         }

         set
         {
            createProperty<string>("commandFilter").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public FileCollection commandFiles
      {
         get
         {
            return createProperty<FileCollection>("commandFiles").currentValue;
         }

         set
         {
            createProperty<FileCollection>("commandFiles").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public string serviceFilter
      {
         get
         {
            return createProperty<string>("serviceFilter").currentValue;
         }

         set
         {
            createProperty<string>("serviceFilter").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public ServiceCollection serviceInstances
      {
         get
         {
            return createProperty<ServiceCollection>("serviceInstances").currentValue;
         }

         set
         {
            createProperty<ServiceCollection>("serviceInstances").currentValue = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      protected override void cleanupProperties()
      {
         
      }
   }
}
