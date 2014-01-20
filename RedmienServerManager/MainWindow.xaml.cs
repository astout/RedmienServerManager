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
            InitializeVMStatusIndicators();
            CheckSettings();
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
        /// Sets the VM Status indicator colors.
        /// </summary>
        private void InitializeVMStatusIndicators()
        {
            //Create a Solid Color Brush for setting custom colors by the hex value selected from the Designer tool.
            SolidColorBrush b = new SolidColorBrush();

            var vmStatus = controller.CheckIsRunning();
            logLine(vmStatus.Item2);

            //if VM is running, set 'on' indicator to green and rest to white
            //otherwise set the 'off' indicator to red and the rest to white
            if(vmStatus.Item1)
            {
                //Set Each indicator to White, except the 'off' indicator.
                ind_off.Fill = Brushes.White;
                ind_booting.Fill = Brushes.White;
                ind_archiving.Fill = Brushes.White;
                ind_shuttingDown.Fill = Brushes.White;

                //Set a brush color to green hex and apply the brush to the 'on' indicator
                b.Color = (Color)ColorConverter.ConvertFromString("#FF74AE53");
                ind_running.Fill = b;

            }
            else
            {
                //Set Each indicator to White, except the 'off' indicator.
                ind_running.Fill = Brushes.White;
                ind_booting.Fill = Brushes.White;
                ind_archiving.Fill = Brushes.White;
                ind_shuttingDown.Fill = Brushes.White;

                //Set a brush color to the red hex value and apply the brush to the 'off' indicator
                b.Color = (Color)ColorConverter.ConvertFromString("#FFE04141");
                ind_off.Fill = b;
            }
        }

        private void btn_stopServer_click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(!controller.Running)
            {
                return;
            }
            var vmstop = controller.StopVM();

            logLine(vmstop.Item2);

            if(vmstop.Item1)
            {
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
            txt_log.Text += text;
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
            txt_log.Text += "\n" + text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_startServer_click(object sender, RoutedEventArgs e)
        {
            logLine("Start button clicked");
        }

        private void btn_archiveNow_click(object sender, RoutedEventArgs e)
        {
            logLine("Archive Now button clicked");
        }

        private void btn_ArchiveSettings_click(object sender, RoutedEventArgs e)
        {
            logLine("Archive Settings button clicked");

            showSettings();
        }
    }
}
