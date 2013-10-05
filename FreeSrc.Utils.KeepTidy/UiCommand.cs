using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FreeSrc.Utils.KeepTidy
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandSequence : List<UiCommand>
    {

    }
    /// <summary>
    /// 
    /// </summary>
    public abstract class UiCommand : ICommand
    {
        /// <summary>
        /// = 2
        /// </summary>
        public const int LOAD_STEP_DELAY = 2;
        /// <summary>
        /// 
        /// </summary>
        public UiCommand()
        {
            this.runAsync = false;
            this.beforeExecute = new CommandSequence();
            this.afterExecute = new CommandSequence();
        }
        /// <summary>
        /// 
        /// </summary>
        public bool runAsync { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool showProcessing { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CommandSequence beforeExecute { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CommandSequence afterExecute { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string commandName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        event EventHandler ICommand.CanExecuteChanged
        {
            add { ; }
            remove { ; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        void ICommand.Execute(object parameter)
        {
            AppState.appModel.cancelProcessing = false;

            if (this.runAsync)
            {
                var task = new Task(() => { this.processExecute(null, AppState.appModel, parameter); });

                task.Start();
            }
            else
            {
                processExecute(null, AppState.appModel, parameter);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentCommand"></param>
        /// <param name="appModel"></param>
        /// <param name="commandParameter"></param>
        /// <returns></returns>
        protected bool processExecute(UiCommand parentCommand, AppModel appModel, object commandParameter)
        {
            var freezeUI = appModel.processingVisibility;

            if (this.runAsync || this.showProcessing)
            {
                appModel.processingMessage = "";
                appModel.processingVisibility = true;
            }

            try
            {
                this.writeLog(Log.Severity.Debug, string.Format("%% executing [{0}]", this.commandName));

                foreach (var c in this.beforeExecute)
                {
                    if (!c.processExecute(this, appModel, commandParameter))
                    {
                        return false;
                    }
                }

                var result = doExecute(AppState.appModel, commandParameter);

                if (result)
                {
                    foreach (var c in this.afterExecute)
                    {
                        if (!c.processExecute(this, appModel, commandParameter))
                        {
                            return false;
                        }
                    }
                }

                return result;
            }
            catch (Exception x)
            {
                this.writeLog(Log.Severity.Error, string.Format("%% exception occurred [{0}]", this.commandName));

                this.writeLog(Log.Severity.Error, x);

                appModel.errorMessage = x.Message;
                appModel.errorTrace = x.StackTrace;

                return false;
            }
            finally
            {
                this.writeLog(Log.Severity.Debug, string.Format("%% executed [{0}]", this.commandName));

                appModel.processingVisibility = freezeUI;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appModel"></param>
        /// <param name="commandParameter"></param>
        protected abstract bool doExecute(AppModel appModel, object commandParameter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootDir"></param>
        /// <param name="searchPattern"></param>
        /// <param name="excludeFolders"></param>
        /// <param name="fileNames"></param>
        public static void getFiles(AppModel appModel, string rootDir, string searchPattern, Regex excludeFolders, List<string> fileNames)
        {
            appModel.processingMessage = string.Format("Scanning directory '{0}'", rootDir);

            foreach (var f in Directory.GetFiles(rootDir, searchPattern, SearchOption.TopDirectoryOnly))
            {
                fileNames.Add(f);
            }

            foreach (var d in Directory.GetDirectories(rootDir))
            {
                if (excludeFolders != null)
                {
                    if (!excludeFolders.IsMatch(d))
                    {
                        getFiles(appModel, d, searchPattern, excludeFolders, fileNames);
                    }
                    else
                    {
                        appModel.processingMessage = string.Format("Skipping directory '{0}'", d);
                    }
                }
                else
                {
                    getFiles(appModel, d, searchPattern, excludeFolders, fileNames);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="rootDir"></param>
        /// <param name="searchFilter"></param>
        /// <param name="fileNames"></param>
        public static void loadFiles(AppModel viewModel, string rootDir, string searchFilter, List<string> fileNames, Action<string> onLoad, string excludeFolders = "")
        {
            Regex regFilter = new Regex(searchFilter);
            Regex regExclude = null;

            if (excludeFolders.Length > 0)
            {
                regExclude = new Regex(excludeFolders);
            }

            List<string> allFiles = new List<string>();

            getFiles(viewModel, viewModel.projectDirectory, "*", regExclude, allFiles);

            foreach (var f in allFiles)
            {
                if (!viewModel.cancelProcessing)
                {
                    viewModel.processingMessage = string.Format("Checking file '{0}'...", f);

                    string bareName = Path.GetFileName(f);

                    if (regFilter.IsMatch(f))
                    {
                        fileNames.Add(f);

                        Thread.Sleep(LOAD_STEP_DELAY);

                        onLoad(f);

                        viewModel.processingMessage = string.Format("Added file '{0}'", f);
                    }
                    else
                    {
                        Thread.Sleep(LOAD_STEP_DELAY);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
