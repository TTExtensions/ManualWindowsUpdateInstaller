using System;
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

        public List<ResultWrapper> ResultEntries { get; set; }

        public InstallResultControl(Exception ex, UpdateManager.InstallResult result, List<UpdateManager.UpdateEntry> updates)
        {
            if (ex == null)
            {
                ResultEntries = new List<ResultWrapper>();
                for (int i = 0; i < result.EntryResults.Length; i++)
                {
                    ResultEntries.Add(new ResultWrapper()
                    {
                        Item = updates[result.EntryResults[i].OriginalListIndex],
                        result = result.EntryResults[i]
                    });
                }
            }

            DataContext = this;
            InitializeComponent();
            resultSummaryLabel.Content = "Result: " + result?.Result ?? "Error, no result available.";
            

            if (!(result?.RebootRequired == true))
            {
                rebootBtn.Visibility = Visibility.Hidden;
                rebootLbl.Visibility = Visibility.Hidden;
            }
            if (ex != null)
            {
                resultListView.Visibility = Visibility.Hidden;
                resultSummaryLabel.Content = "Error: " + ex.Message;
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



        public class ResultWrapper
        {
            public UpdateManager.InstallEntryResult result;
            public UpdateManager.UpdateEntry Item;

            public string Description => Item.Title;

            public string Result => result.Result.ToString();
        }
    }
}
