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
        private bool preventWindowHide = false;
        private bool isAwaiting = false;
        private bool closeAfterAwait = false;

        private Action handleCloseAction = null;


        public MainWindow()
        {
            InitializeComponent();
            

            // Search for Updates.
            SearchForUpdates();
        }

        private async void SearchForUpdates()
        {
            try {
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
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private void ShowUpdates(List<UpdateManager.UpdateEntry> updates)
        {
            
            UpdateList list = new UpdateList(updates, InstallUpdates);
            mainGrid.Children.Clear();
            mainGrid.Children.Add(list);
        }

        private async void InstallUpdates(List<UpdateManager.UpdateEntry> updates)
        {
            preventWindowHide = true;
            try
            {
                bool cancelRequested = false;
                Action cancelAction = () => cancelRequested = true;

                ProgressBarControl list = new ProgressBarControl("Downloading Updates…", cancelAction);
                mainGrid.Children.Clear();
                mainGrid.Children.Add(list);
                handleCloseAction = list.HandleClose;
                try
                {
                    await RunUpdateManagerActionAsync(async () =>
                    {
                        await updateManager.DownloadUpdatesAsync(updates);
                    });
                }
                finally
                {
                    handleCloseAction = null;
                }

                // If cancelling has been requested we don't install the updates but
                // instead simply close the window.
                if (cancelRequested)
                {
                    preventWindowHide = false;
                    Close();
                    return;
                }


                list = new ProgressBarControl("Installing Updates…");
                mainGrid.Children.Clear();
                mainGrid.Children.Add(list);

                // TODO: Disable the window's close button through winAPI while installing
                // updates since this cannot be canceled.
                UpdateManager.InstallResult result = null;
                await RunUpdateManagerActionAsync(async () =>
                {
                    result = await updateManager.InstallUpdatesAsync(updates);
                });

                InstallResultControl ctrl = new InstallResultControl(null, result, updates);
                mainGrid.Children.Clear();
                mainGrid.Children.Add(ctrl);
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
            finally
            {
                preventWindowHide = false;
            }
        }

        private void ShowException(Exception ex)
        {
            InstallResultControl ctrl = new InstallResultControl(ex, null, null);
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
            if (handleCloseAction != null)
                handleCloseAction();

            if (preventWindowHide)
            {
                e.Cancel = true;
            }
            else if (isAwaiting)
            {
                // Wait for the background thread.
                e.Cancel = true;
                closeAfterAwait = true;
                Hide();
            }
            else
            {
                updateManager.Dispose();
            }
        }
    }
}
