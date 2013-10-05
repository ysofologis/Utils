using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSrc.Utils.KeepTidy
{
   public static class Log
   {
      /// <summary>
      /// 
      /// </summary>
      static Log()
      {
         initLog();
      }
      /// <summary>
      /// 
      /// </summary>
      public class NDC : IDisposable
      {
         /// <summary>
         /// 
         /// </summary>
         /// <param name="message"></param>
         public NDC(string message)
         {
            log4net.NDC.Push(message);
         }
         /// <summary>
         /// 
         /// </summary>
         public void Dispose()
         {
            log4net.NDC.Pop();
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public enum Severity
      {
         Debug,
         Info,
         Warn,
         Error,
         Fatal,
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="confFile"></param>
      public static void initLog()
      {
         log4net.Config.XmlConfigurator.Configure();
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="classType"></param>
      /// <param name="severity"></param>
      /// <param name="message"></param>
      /// <param name="x"></param>
      public static void writeLine(Type classType, Log.Severity severity, object message, Exception x)
      {
         log4net.ILog logger = log4net.LogManager.GetLogger(classType);

         switch (severity)
         {
            case Severity.Debug:
               if (x == null)
               {
                  logger.Debug(message);
               }
               else
               {
                  logger.Debug(message, x);
               }
               break;
            case Severity.Error:
               if (x == null)
               {
                  logger.Error(message);
               }
               else
               {
                  logger.Error(message, x);
               }
               break;
            case Severity.Fatal:
               if (x == null)
               {
                  logger.Fatal(message);
               }
               else
               {
                  logger.Fatal(message, x);
               }
               break;
            case Severity.Info:
               if (x == null)
               {
                  logger.Info(message);
               }
               else
               {
                  logger.Info(message, x);
               }
               break;
            case Severity.Warn:
               if (x == null)
               {
                  logger.Warn(message);
               }
               else
               {
                  logger.Warn(message, x);
               }
               break;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <param name="severity"></param>
      /// <param name="message"></param>
      /// <param name="x"></param>
      public static void writeLog(this object instance, Log.Severity severity, object message, Exception x)
      {
         writeLine(instance.GetType(), severity, message, x);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="classType"></param>
      /// <param name="severity"></param>
      /// <param name="message"></param>
      public static void writeLine(Type classType, Log.Severity severity, object message)
      {
         writeLine(classType, severity, message, null);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="instance"></param>
      /// <param name="severity"></param>
      /// <param name="message"></param>
      public static void writeLog(this object instance, Log.Severity severity, object message)
      {
         writeLog(instance, severity, message, null);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <typeparam name="InstanceType"></typeparam>
      /// <param name="severity"></param>
      /// <param name="message"></param>
      /// <param name="x"></param>
      public static void writeLine<InstanceType>(Log.Severity severity, object message, Exception x)
      {
         writeLine(typeof(InstanceType), severity, message, x);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <typeparam name="InstanceType"></typeparam>
      /// <param name="severity"></param>
      /// <param name="message"></param>
      public static void writeLine<InstanceType>(Log.Severity severity, object message)
      {
         writeLine(typeof(InstanceType), severity, message, null);
      }
   }
}
