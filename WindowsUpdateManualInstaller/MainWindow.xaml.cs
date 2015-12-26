using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private UpdateManagerAsyncWrapper updateManager = new UpdateManagerAsyncWrapper();
        private bool isAwaiting = false;
        private bool closeAfterAwait = false;


        public MainWindow()
        {
            InitializeComponent();



            // Search for Updates.
            SearchForUpdates();
        }

        private async void SearchForUpdates()
        {
            ProgressBarControl ctrl = new ProgressBarControl("Searching for Updates…");
            mainGrid.Children.Clear();
            mainGrid.Children.Add(ctrl);

            List<UpdateManager.UpdateEntry> entries = null;
            await RunUpdateManagerActionAsync(async () =>
            {
                 entries = await updateManager.GetAvailableUpdatesAsync();
            });

            ShowUpdates(entries);
        }

        private void ShowUpdates(List<UpdateManager.UpdateEntry> updates)
        {
            
            UpdateList list = new UpdateList(updates, InstallUpdates);
            mainGrid.Children.Clear();
            mainGrid.Children.Add(list);
        }

        private async void InstallUpdates(List<UpdateManager.UpdateEntry> updates)
        {
            ProgressBarControl list = new ProgressBarControl("Downloading Updates…");
            mainGrid.Children.Clear();
            mainGrid.Children.Add(list);

            await RunUpdateManagerActionAsync(async () =>
            {
                await updateManager.DownloadUpdatesAsync(updates);
            });


            list = new ProgressBarControl("Installing Updates…");
            mainGrid.Children.Clear();
            mainGrid.Children.Add(list);

            UpdateManager.InstallResult result = null;
            await RunUpdateManagerActionAsync(async () =>
            {
                result = await updateManager.InstallUpdatesAsync(updates);
            });


            InstallResultControl ctrl = new InstallResultControl(result);
            mainGrid.Children.Clear();
            mainGrid.Children.Add(ctrl);
        }

        private async Task RunUpdateManagerActionAsync(Func<Task> action)
        {
            isAwaiting = true;
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                if (!closeAfterAwait)
                    MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                isAwaiting = false;
                if (closeAfterAwait)
                {
                    Close();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isAwaiting)
            {
                // Wait for the background thread.
                closeAfterAwait = true;
                e.Cancel = true;
                Hide();
            }
            else
            {
                updateManager.Dispose();
            }
        }
    }
}
