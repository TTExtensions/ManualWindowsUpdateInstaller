using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
    /// Interaktionslogik für UpdateList.xaml
    /// </summary>
    public partial class UpdateList : UserControl
    {

        public List<UpdateEntrySelectionWrapper> UpdateEntries { get; set; }

        private UpdateEntrySelectionWrapper currentSelection = null;

        private Action<List<UpdateManager.UpdateEntry>> installCallback;

        public UpdateList(List<UpdateManager.UpdateEntry> updates, Action<List<UpdateManager.UpdateEntry>> installCallback)
        {
            this.installCallback = installCallback;

            UpdateEntries = new List<UpdateEntrySelectionWrapper>();
            for (int i = 0; i < updates.Count; i++)
            {
                var wrp = new UpdateEntrySelectionWrapper() { Item = updates[i] };
                wrp.IsChecked = wrp.Item.AutoSelectOnWebSites;
                wrp.IsCheckedChanged += RefreshSummaryLabel;
                UpdateEntries.Add(wrp);
            }

            DataContext = this;

            InitializeComponent();

            ShowSelection(null);
            RefreshSummaryLabel();
        }
        

        private static string FormatFileSize(long size)
        {
            if (size < 1000)
                return size + " Bytes"; // TODO
            else if (size < 1000L * 1024)
                return ((double)size / (1024L)).ToString("0") + " KiB";
            else
            {
                return ((double)size / (1024L * 1024)).ToString("0.0") + " MiB";
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Contains(currentSelection))
            {
                ShowSelection(null);
            }

            if (e.AddedItems.Count > 0)
                ShowSelection((UpdateEntrySelectionWrapper)e.AddedItems[0]);
        }

        private void ShowSelection(UpdateEntrySelectionWrapper selection)
        {
            currentSelection = selection;
            if (selection == null)
            {
                selectionDetailsTextblock.Text = string.Empty;
                linkLabel.Text = string.Empty;
            }
            else
            {
                selectionDetailsTextblock.Text = "Categories: " + string.Join(", ", selection.Item.Categories) 
                    + "\r\n\r\n" + selection.Item.Description;
                linkLabel.Text = selection.Item.SupportUrl;
            }
        }

        private void RefreshSummaryLabel()
        {
            int updateCount = 0;
            long updateSizeMin = 0;
            long updateSizeMax = 0;
            foreach (UpdateEntrySelectionWrapper wrp in UpdateEntries)
            {
                if (wrp.IsChecked)
                {
                    updateCount++;
                    updateSizeMax += wrp.Item.MaxDownloadSize;
                    updateSizeMin += wrp.Item.MinDownloadSize == 0 ? wrp.Item.MaxDownloadSize : wrp.Item.MinDownloadSize;
                }
            }

            if (updateCount == 0)
            {
                summaryLabel.Content = "No updates selected.";
                installUpdatesBtn.IsEnabled = false;
            }
            else
            {
                installUpdatesBtn.IsEnabled = true;
                string size = updateSizeMin == updateSizeMax ? FormatFileSize(updateSizeMax) : $"{FormatFileSize(updateSizeMin)} – {FormatFileSize(updateSizeMax)}";
                summaryLabel.Content = $"{updateCount} Updates selected; Size: {size}";
            }
        }
        
        private void installUpdatesBtn_Click(object sender, RoutedEventArgs e)
        {
            List<UpdateManager.UpdateEntry> updatesToInstall = new List<UpdateManager.UpdateEntry>();
            foreach (UpdateEntrySelectionWrapper wrp in UpdateEntries)
            {
                if (wrp.IsChecked)
                    updatesToInstall.Add(wrp.Item);
            }


            installCallback(updatesToInstall);
        }

        private void linkLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentSelection != null && currentSelection.Item.SupportUrl.StartsWith("http://") || currentSelection.Item.SupportUrl.StartsWith("https://"))
            {
                ProcessStartInfo pinf = new ProcessStartInfo(currentSelection.Item.SupportUrl);
                pinf.UseShellExecute = true;
                Process.Start(pinf);
            }
        }




        public class UpdateEntrySelectionWrapper
        {
            public event Action IsCheckedChanged;

            private bool isChecked;
            public bool IsChecked {
                get
                {
                    return isChecked;
                }
                set
                {
                    isChecked = value;
                    if (IsCheckedChanged != null)
                        IsCheckedChanged();
                }
            }
            public UpdateManager.UpdateEntry Item { get; set; }

            public string Description
            {
                get
                {
                    return $"[{Item.DownloadPriority}] {Item.Title}";
                }
            }

            public string DownloadSize
            {
                get
                {
                    if (Item.MaxDownloadSize == Item.MinDownloadSize || Item.MinDownloadSize == 0)
                        return FormatFileSize(Item.MaxDownloadSize);
                    else
                        return $"{FormatFileSize(Item.MinDownloadSize)} – {FormatFileSize(Item.MaxDownloadSize)}";
                }
            }
        }
    }
}
