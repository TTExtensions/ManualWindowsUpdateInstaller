﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsUpdateManualInstaller
{
    /// <summary>
    /// Interaktionslogik für InstallResultControl.xaml
    /// </summary>
    public partial class InstallResultControl : UserControl
    {

        private UpdateManager.InstallResult result;

        public InstallResultControl(UpdateManager.InstallResult result)
        {
            InitializeComponent();
            this.result = result;

            resultSummaryLabel.Content = "Result: " + result?.Result ?? "Error, no result available.";

            if (result?.RebootRequired == true)
            {
                rebootBtn.Visibility = Visibility.Visible;
                rebootLbl.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo pinf = new ProcessStartInfo(System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System), "shutdown.exe"), "/r /t 0");
            pinf.UseShellExecute = false;
            pinf.CreateNoWindow = true;

            Process.Start(pinf);
        }
    }
}
