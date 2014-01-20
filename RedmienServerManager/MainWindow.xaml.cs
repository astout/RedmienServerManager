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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.WindowsAPICodePack.Dialogs;

/*
 * Remember: go away from the server controller method and use the BackgroundWorker in the MainWindow.
 * Use a separate bgworker for each lengthy process.
 */

namespace RedmineServerManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //SETTINGS DEFAULTS
        //string def_vmNAme = "redmine";
        string def_vmDirectory = "Where the Virtual Machine Exists";
        string def_archiveDirectory = "Where to save the archives";
        ServerController controller = new ServerController();
        

        public MainWindow()
        {
            InitializeComponent();
            //Properties.Settings.Default.Reset();
            Tuple<int, string> status = controller.CheckStatus();
            logLine(status.Item2);
            UpdateStatusIndicators(status.Item1);
            CheckSettings();
            controller.ArchiveComplete +=controller_ArchiveComplete;
        }

        private void controller_ArchiveComplete(int status, string msg)
        {
            string s = msg;
            logLine(s);
            UpdateStatusIndicators(status);
        }

        /// <summary>
        /// Checks the saved settings for valid entries.  If not all valid, then launch the 
        /// settings window.  
        /// </summary>
        private void CheckSettings()
        {
            var set = Properties.Settings.Default;
            if(set.VMlocation == "" || set.VMlocation == def_vmDirectory || set.SaveLocation == def_archiveDirectory || set.SaveLocation == "")
            {
                showSettings();
            }
        }

        /// <summary>
        /// Shows the settings window and hides the controller window.
        /// The controller window then reveals upon settings window close.
        /// </summary>
        private void showSettings()
        {
            ArchiveSettingsWindow win_archive = new ArchiveSettingsWindow();
            this.Visibility = System.Windows.Visibility.Hidden;
            win_archive.ShowDialog();
            this.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status">
        /// 0: off
        /// 1: running
        /// 2: archiving
        /// 3: booting
        /// 4: shutting down
        /// </param>
        private void UpdateStatusIndicators(int status)
        {
            //Create a Solid Color Brush for setting custom colors by the hex value selected from the Designer tool.
            SolidColorBrush b = new SolidColorBrush();

            //var vmStatus = controller.CheckStatus();
            //logLine(vmStatus.Item2);
            //Set Each indicator to White, except the 'off' indicator.


            Dispatcher.BeginInvoke(new Action(() => 
            {
                ind_running.Fill = Brushes.White;
                ind_off.Fill = Brushes.White;
                ind_booting.Fill = Brushes.White;
                ind_archiving.Fill = Brushes.White;
                ind_shuttingDown.Fill = Brushes.White;
            }));
            

            switch(status)
            {
                case 0:
                    b.Color = (Color)ColorConverter.ConvertFromString("#FFE04141");
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ind_off.Fill = b;
                    }));
                    break;
                case 1:
                    b.Color = (Color)ColorConverter.ConvertFromString("#FF74AE53");
                    Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ind_running.Fill = b;
                        }));
                    break;
                case 2:
                    b.Color = (Color)ColorConverter.ConvertFromString("#FF50C1DA");
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ind_archiving.Fill = b;
                    }));
                    break;
                case 3:
                    b.Color = (Color)ColorConverter.ConvertFromString("#FFE0D941");
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ind_booting.Fill = b;
                    }));
                    break;
                case 4:
                    b.Color = (Color)ColorConverter.ConvertFromString("#FFD47C62");
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ind_shuttingDown.Fill = b;
                    }));
                    break;
            }

        }

        /// <summary>
        /// If the server is not running, returns
        /// else stops the server, and updates the status indicators.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stopServer_click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(controller.Status != 1)
            {
                return;
            }
            var vmstop = controller.StopVM();

            logLine(vmstop.Item2);

            if(controller.Status == 0)
            {
                UpdateStatusIndicators(0);
                logLine(Properties.Settings.Default.VMName + " has stopped");
            }
        }
		
        private void exp_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height += 355;
        }

        private void exp_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height -= 355;
        }

        /// <summary>
        /// logs the provided string to the log console.
        /// A new line is not implied.
        /// </summary>
        /// <param name="text"></param>
        public void log(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    txt_log.Text += text;
                }));
        }

        /// <summary>
        /// logs the provided string to the log console.
        /// A new line is implied.
        /// </summary>
        /// <param name="text"></param>
        public void logLine(string text)
        {
            if(text.Length < 1)
                return;
            Dispatcher.BeginInvoke(new Action(() => 
                {
                    txt_log.Text += "\n" + text;
                }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_startServer_click(object sender, RoutedEventArgs e)
        {
            logLine(controller.Status.ToString());
            if (controller.Status != 0)
            {
                return;
            }
            var vmstart = controller.StartVM();

            logLine(vmstart.Item2);

            if (controller.Status == 1)
            {
                UpdateStatusIndicators(controller.Status);
                logLine("'" + Properties.Settings.Default.VMName + "' has started");
            }
        }

        private void btn_archiveNow_click(object sender, RoutedEventArgs e)
        {
            if(controller.Status == 1)
            {
                string txt = "The Virtual Machine is currently running.  It must be stopped to archive.\n Stop now?";
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(txt, "Stop Virtual Machine?",MessageBoxButton.YesNo);
                if(result == MessageBoxResult.Yes)
                {
                    btn_stopServer_click(sender, e);
                }
                else
                {
                    return;
                }
            }
            if(controller.Status == 2)
            {
                string txt = "The Virtual Machine is currently archiving.";
                System.Windows.MessageBox.Show(txt, "Archiving");
                return;
            }
            logLine("Got Here.");
            Tuple<int, string> arch = controller.ArchiveVM();

            //controller
            
            logLine(arch.Item2);
            UpdateStatusIndicators(arch.Item1);
        }

        private void btn_ArchiveSettings_click(object sender, RoutedEventArgs e)
        {
            logLine("Archive Settings button clicked");

            showSettings();
        }
    }
}
