using System;
using System.Collections.Generic;
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
    /// Interaktionslogik für SearchingForUpdates.xaml
    /// </summary>
    public partial class ProgressBarControl : UserControl
    {
        private string text;
        private Action cancelAction;
        private bool cancelHandled = false;

        public ProgressBarControl(string text, Action cancelAction = null)
        {
            InitializeComponent();
            this.text = text;
            this.cancelAction = cancelAction;

            label.Content = text;
            if (cancelAction == null)
            {
                cancelBtn.Visibility = Visibility.Hidden;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            HandleClose();
        }

        public void HandleClose()
        {
            if (cancelAction != null && !cancelHandled)
            {
                cancelHandled = true;

                cancelBtn.IsEnabled = false;
                cancelAction();

                label.Content = text + " – Canceling, please wait…";
            }
        }
    }
}
