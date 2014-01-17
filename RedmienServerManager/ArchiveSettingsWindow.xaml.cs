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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Configuration;
using System.Text.RegularExpressions;

namespace RedmineServerManager
{
    /// <summary>
    /// Interaction logic for ArchiveSettingsWindow.xaml
    /// </summary>
    public partial class ArchiveSettingsWindow : Window
    {
        public ArchiveSettingsWindow()
        {

            InitializeComponent();
            CheckSettings();

            DateTime t = DateTime.Now;
            //MessageBox.Show(t.ToString()); //added comment

        }

        private void CheckSettings()
        {
            var set = Properties.Settings.Default;

            //Initialize the VM location field
            if(set.VMlocation == "")
            {
                txt_VMlocation.Text = "Where the Virtual Machine Exists";
                txt_VMlocation.Foreground = Brushes.Gray;
            }
            else
            {
                txt_VMlocation.Text = set.VMlocation;
                txt_VMlocation.Foreground = Brushes.Black;
            }

            //Initialize the VM Name field
            if(set.VMName == "")
            {
                txt_VMName.Text = "redmine";
                txt_VMName.Foreground = Brushes.Gray;
            }
            else
            {
                txt_VMName.Text = set.VMName;
                txt_VMName.Foreground = Brushes.Black;
            }

            //Initialize the Archive Location Field
            if(set.SaveLocation == "")
            {
                txt_archLocation.Text = "Where to save the archives";
                txt_archLocation.Foreground = Brushes.Gray;
            }
            else
            {
                txt_archLocation.Text = set.SaveLocation;
                txt_archLocation.Foreground = Brushes.Black;
            }

            //Initialize Archive frequency combo box
            //The value is stored as the selection index
            //Monthly: 0
            //Weekly: 1
            //Daily: 2
            if(!(set.ArchiveFreq >=0 && set.ArchiveFreq <= 2))
            {
                combo_ArchiveFreq.SelectedIndex = -1;
            }
            else
            {
                combo_ArchiveFreq.SelectedIndex = set.ArchiveFreq;
            }

            //
            if(set.ArchiveTime == "")
            {
                combo_ArchiveTime_H.SelectedIndex = 0;
                combo_ArchiveTime_M.SelectedIndex = 0;
                combo_ArchiveTime_AP.SelectedIndex = 0;
            }
            else
            {
                //stub
                combo_ArchiveTime_H.SelectedIndex = 0;
                combo_ArchiveTime_M.SelectedIndex = 0;
                combo_ArchiveTime_AP.SelectedIndex = 0;
            }

        }

        private void combo_ArchiveFreq_changed(object sender, SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.ArchiveFreq = combo_ArchiveFreq.SelectedIndex;

            var set = Properties.Settings.Default;

            //If 'Monthly' selected
            if(combo_ArchiveFreq.SelectedIndex == 0)
            {
                if (set.ArchiveMDays == "")
                {
                    txt_archiveMDays.Text = "1,11,21";
                    txt_archiveMDays.Foreground = Brushes.Gray;
                }
                else
                {
                    txt_archiveMDays.Text = set.ArchiveMDays;
                    txt_archiveMDays.Foreground = Brushes.Black;
                }
                txt_archiveMDays.Visibility = System.Windows.Visibility.Visible;
            }

            //If 'Weekly' Selected
            if(combo_ArchiveFreq.SelectedIndex == 1)
            {
                if (set.ArchiveWDays != "")
                {
                    string[] days = set.ArchiveWDays.Split(',');

                    //string day_string = "";

                    chk_Sun.IsChecked = days[0] == "1" ? true : false;
                    chk_Mon.IsChecked = days[1] == "1" ? true : false;
                    chk_Tue.IsChecked = days[2] == "1" ? true : false;
                    chk_Wed.IsChecked = days[3] == "1" ? true : false;
                    chk_Thu.IsChecked = days[4] == "1" ? true : false;
                    chk_Fri.IsChecked = days[5] == "1" ? true : false;
                    chk_Sat.IsChecked = days[6] == "1" ? true : false;
                    System.Windows.MessageBox.Show(days[6]);
                }

                txt_archiveMDays.Visibility = System.Windows.Visibility.Hidden;
                view_ArchWDays.Visibility = System.Windows.Visibility.Visible;

            }
        }

        private void combo_ArchiveTimeH_changed(object sender, SelectionChangedEventArgs e)
        {
            RebuildTimeString();
        }

        private void RebuildTimeString()
        {
            if(combo_ArchiveTime_M == null|| combo_ArchiveTime_H == null || combo_ArchiveTime_AP == null)
            {
                return;
            }
            int h = combo_ArchiveTime_H.SelectedIndex == 0 ? 12 : combo_ArchiveTime_H.SelectedIndex;
            int m = combo_ArchiveTime_M.SelectedIndex * 5;
            string ap = combo_ArchiveTime_AP.SelectedIndex == 0 ? "AM" : "PM";

            //MessageBox.Show(combo_ArchiveTime_AP.Text);

            string time = h + ":" + m.ToString("00") + " " + ap;

            txt_ArchiveTime.Text = time;

      

            //Regex rx = new Regex("[:]");

            //Match m = rx.Match(txt_ArchiveTime.Text);

            //if (m.Success)
            //{

            //}
        }

        private void combo_ArchiveTimeM_changed(object sender, SelectionChangedEventArgs e)
        {
            RebuildTimeString();
        }

        private void combo_ArchiveTimeAP_changed(object sender, SelectionChangedEventArgs e)
        {
            RebuildTimeString();
        }

        private void btn_VmDirSelect_click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog vmDirDialog = new FolderBrowserDialog();
            vmDirDialog.ShowDialog();

            if (vmDirDialog.SelectedPath.Length > 1)
            {
                txt_VMlocation.Text = vmDirDialog.SelectedPath;
                txt_VMlocation.Foreground = Brushes.Black;
            }
        }

        private void btn_archLocation_click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog archDirDialog = new FolderBrowserDialog();
            archDirDialog.ShowDialog();

            if(archDirDialog.SelectedPath.Length > 1)
            {
                txt_archLocation.Text = archDirDialog.SelectedPath;
                txt_VMlocation.Foreground = Brushes.Black;
            }
        }

        private void btn_saveSettings_click(object sender, RoutedEventArgs e)
        {
            //Save all text fields, trim off any excess whitespace
            Properties.Settings.Default.VMName = txt_VMName.Text.Trim();
            Properties.Settings.Default.VMlocation = txt_VMlocation.Text.Trim();
            Properties.Settings.Default.SaveLocation = txt_archLocation.Text.Trim();

            //Save the time settings

            //Archive Frequency, Monthly: 0, Weekly: 1, Daily: 2
            Properties.Settings.Default.ArchiveFreq = combo_ArchiveFreq.SelectedIndex; //An integer 0-2
            int h = combo_ArchiveTime_H.SelectedIndex == 0 ? 12 : combo_ArchiveTime_H.SelectedIndex;
            int m = combo_ArchiveTime_M.SelectedIndex * 5;
            string ap = combo_ArchiveTime_AP.SelectedIndex == 0 ? "AM" : "PM";
            string time = h.ToString("00") + ":" + m.ToString("00") + " " + ap;
            Properties.Settings.Default.ArchiveTime = time;

            switch(combo_ArchiveFreq.SelectedIndex)
            {
                case 0:
                    Properties.Settings.Default.ArchiveMDays = txt_archiveMDays.Text.Replace(@"\s", "");
                    break;
                case 1:
                    Properties.Settings.Default.ArchiveWDays = GetWDaysString();
                    break;
                default:
                    break;
            }

            string text = "";
            foreach (SettingsProperty setting in Properties.Settings.Default.Properties)
            {
                text += "Setting: " + setting.Name + ": " + Properties.Settings.Default[setting.Name] + "\n";
            }

            System.Windows.MessageBox.Show(text);

            Properties.Settings.Default.Save();

            this.Close();
        }

        private string GetWDaysString()
        {
            string days = "";

            days += chk_Sun.IsChecked == true ? 1 : 0;
            days += chk_Mon.IsChecked == true ? 1 : 0;
            days += chk_Tue.IsChecked == true ? 1 : 0;
            days += chk_Wed.IsChecked == true ? 1 : 0;
            days += chk_Thu.IsChecked == true ? 1 : 0;
            days += chk_Fri.IsChecked == true ? 1 : 0;
            days += chk_Sat.IsChecked == true ? 1 : 0;

            return days;
        }
    }
}
