﻿using System;
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

namespace RedmineServerManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckSettings();
            if(!isVMRunning())
            {
                InitializeVMStatusIndicators();
            }
        }

        private void CheckSettings()
        {
            var set = Properties.Settings.Default;
            if(set.VMlocation == "" || set.VMName == "" || set.SaveLocation == "")
            {
                ArchiveSettingsWindow win_archive = new ArchiveSettingsWindow();
                win_archive.Show();
            }
            
        }

        private bool isVMRunning()
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C vboxmanage list runningvms | find \"redmine\" | find /c /v \"~~~\"";
            process.Start();
            int num = -1;
            string output = "";
            if (process.StandardOutput != null)
            {
                output = process.StandardOutput.ReadLine();
            }
            Regex r = new Regex(@"\d");
            logLine(output);
            Match m = r.Match(output);
            if (m.Success)
            {
                try
                {
                    num = Convert.ToInt32(m.Value);
                }
                catch (FormatException e)
                {
                    logLine("The number of virtual machine's by the name 'redmine', couldn't be parsed.");
                    num = -1;
                }
            }
            logLine(process.StandardOutput.ReadToEnd());
            process.WaitForExit();

            if (num > -1)
            {
                if (num == 0)
                    return false;
                else
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the VM Status indicator colors.
        /// </summary>
        private void InitializeVMStatusIndicators()
        {
            //Create a Solid Color Brush for setting custom colors by the hex value selected from the Designer tool.
            SolidColorBrush b = new SolidColorBrush();

            //Set Each indicator to White, except the 'off' indicator.
            ind_running.Fill = Brushes.White;
            ind_booting.Fill = Brushes.White;
            ind_archiving.Fill = Brushes.White;
            ind_shuttingDown.Fill = Brushes.White;

            //Set a brush color to the red hex value and apply the brush to the 'off' indicator
            b.Color = (Color)ColorConverter.ConvertFromString("#FFE04141");
            ind_off.Fill = b;
        }

        private void btn_stopServer_click(object sender, System.Windows.RoutedEventArgs e)
        {
            logLine("Stop button clicked");
        }
		
		public bool isServerRunning()
		{
			Console.WriteLine();
            return true; //:STUMP
		}

		private void exp_log_mouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
		}

        private void exp_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height += 355;
        }

        private void exp_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height -= 355;
        }

        public void log(string text)
        {
            txt_log.Text += text;
        }

        public void logLine(string text)
        {
            if(text.Length < 1)
                return;
            txt_log.Text += "\n" + text;
        }

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
        }
    }
}
