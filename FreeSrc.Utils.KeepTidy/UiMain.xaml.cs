﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using MahApps.Metro.Controls;

namespace FreeSrc.Utils.KeepTidy
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class UiMain : MetroWindow
   {
      public UiMain()
      {
         InitializeComponent();

         this.writeLog(Log.Severity.Debug, "App started");

         (this.DataContext as AppModel).isDesignMode = DesignerProperties.GetIsInDesignMode(this); 
      }
   }
}
