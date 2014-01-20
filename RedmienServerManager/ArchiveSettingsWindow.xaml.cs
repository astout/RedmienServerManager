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
using Microsoft.WindowsAPICodePack.Dialogs;

namespace RedmineServerManager
{
    /// <summary>
    /// Interaction logic for ArchiveSettingsWindow.xaml
    /// </summary>
    public partial class ArchiveSettingsWindow : Window
    {

        private bool Saved;

        //FIELD DEFAULTS
        string def_vmNAme = "redmine";
        string def_vmDirectory = "Where the Virtual Machine Exists";
        string def_archiveDirectory = "Where to save the archives";

        public ArchiveSettingsWindow()
        {

            InitializeComponent();
            CheckSettings();

            Saved = false;

            //this.Focus();
            //DateTime t = DateTime.Now;
            //MessageBox.Show(t.ToString()); //added comment

        }

        private void CheckSettings()
        {
            var set = Properties.Settings.Default;

            //Initialize the VM location field
            if(set.VMlocation == "")
            {
                txt_VMlocation.Text = def_vmDirectory; //set to defualt name
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
                txt_archLocation.Text = def_archiveDirectory;
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
            //Properties.Settings.Default.ArchiveFreq = combo_ArchiveFreq.SelectedIndex;

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
                view_Mdays.Visibility = System.Windows.Visibility.Visible;
            }

            //If 'Weekly' Selected
            else if(combo_ArchiveFreq.SelectedIndex == 1)
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
                }

                view_Mdays.Visibility = System.Windows.Visibility.Hidden;
                view_ArchWDays.Visibility = System.Windows.Visibility.Visible;

            }

            else
            {
                view_ArchWDays.Visibility = System.Windows.Visibility.Hidden;
                view_Mdays.Visibility = System.Windows.Visibility.Hidden;
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
            //FolderBrowserDialog vmDirDialog = new FolderBrowserDialog();
            //vmDirDialog.ShowDialog();

            //if (vmDirDialog.SelectedPath.Length > 1)
            //{
            //    txt_VMlocation.Text = vmDirDialog.SelectedPath;
            //    txt_VMlocation.Foreground = Brushes.Black;
            //}
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "Select Virtual Machine Directory";
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\VirtualBox VMs";

            dialog.AddToMostRecentlyUsedList = false;
            dialog.AllowNonFileSystemItems = false;
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txt_VMlocation.Text = dialog.FileName;
                txt_VMlocation.Foreground = Brushes.Black;
            }

        }

        private void btn_archLocation_click(object sender, RoutedEventArgs e)
        {
            //FolderBrowserDialog archDirDialog = new FolderBrowserDialog();
            //archDirDialog.ShowDialog();

            //if(archDirDialog.SelectedPath.Length > 1)
            //{
            //    txt_archLocation.Text = archDirDialog.SelectedPath;
            //    txt_archLocation.Foreground = Brushes.Black;
            //}

            var dialog = new CommonOpenFileDialog();
            dialog.Title = "Select Virtual Machine Directory";
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

            dialog.AddToMostRecentlyUsedList = false;
            dialog.AllowNonFileSystemItems = false;
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txt_archLocation.Text = dialog.FileName;
                txt_archLocation.Foreground = Brushes.Black;
            }
        }

        /// <summary>
        /// Parses the data entered in the form and saves it to User Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_saveSettings_click(object sender, RoutedEventArgs e)
        {
            //Validate
            if (txt_VMName.Text.Length < 1)
            {
                System.Windows.MessageBox.Show("Please enter a valid name in the 'VM Name' field.", "Invalid Entry");
                txt_VMName.Focus();
                return;
            }
            if (txt_VMlocation.Text == def_vmDirectory || txt_VMlocation.Text.Length < 1)
            {
                System.Windows.MessageBox.Show("Please enter a valid path in the 'VM Directory' field.", "Invalid Entry");
                txt_VMlocation.Focus();
                return;
            }
            if (txt_archLocation.Text == def_archiveDirectory || txt_archLocation.Text.Length < 1)
            {
                System.Windows.MessageBox.Show("Please enter a valid path in the 'Archive Directory' field.", "Invalid Entry");
                txt_archLocation.Focus();
                return;
            }
            if(combo_ArchiveFreq.SelectedIndex == 0) //monthly
            {
                var result = GetMDays(txt_archiveMDays.Text); //returns a 3-tuple
                string txt = "";
                switch (result.Item2)
                {
                    case 0: //valid
                        break;
                    case 1: //invalid syntax
                        txt = "Please Enter a valid list of month days separated by commas.\n";
                        txt += "(For example: 1,11,21).";
                        System.Windows.MessageBox.Show(txt, "Invalid Entry");
                        view_ArchWDays.Visibility = System.Windows.Visibility.Hidden;
                        view_Mdays.Visibility = System.Windows.Visibility.Visible;
                        txt_archiveMDays.Focus();
                        return;
                    case 2: //invalid, numbers out of range
                        txt = "The following numbers are out of range for a month:\n";
                        txt += result.Item3 + "\n";
                        txt += "The numbers should be comma separated and in the range 1-31.\n";
                        txt += "(For example: 1,11,21).";
                        System.Windows.MessageBox.Show(txt, "Days out of Range");
                        view_ArchWDays.Visibility = System.Windows.Visibility.Hidden;
                        view_Mdays.Visibility = System.Windows.Visibility.Visible;
                        txt_archiveMDays.Focus();
                        return;
                }
            }
            if(combo_ArchiveFreq.SelectedIndex == 1) //weekly
            {
                int sum = 0;
                sum += chk_Sun.IsChecked == true ? 1 : 0;
                sum += chk_Mon.IsChecked == true ? 1 : 0;
                sum += chk_Tue.IsChecked == true ? 1 : 0;
                sum += chk_Wed.IsChecked == true ? 1 : 0;
                sum += chk_Thu.IsChecked == true ? 1 : 0;
                sum += chk_Fri.IsChecked == true ? 1 : 0;
                sum += chk_Sat.IsChecked == true ? 1 : 0;
                if(sum == 7 || sum == 0)
                {
                    string txt = "If you select every day or no days when you have chosen 'Weekly'\n";
                    txt += "for 'Archive Frequency,' It will be treated as a 'daily' archive setting.";
                    System.Windows.MessageBoxResult r = System.Windows.MessageBox.Show(txt, "Notice: Archives will be Daily!", MessageBoxButton.OKCancel);
                    if(r == System.Windows.MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
            }

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

            //Switch on Archive Frequency: Monthly: 0, Weekly: 1, Daily: 2
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

            System.Windows.MessageBox.Show(text, "Debug");

            Properties.Settings.Default.Save();
            Saved = true;

            this.Close();
        }

        /// <summary>
        /// Checks that the string in txt_archiveMDays is valid.
        /// The string should be comma separated numbers between 0-31
        /// 
        /// </summary>
        /// <returns>
        /// A two-part tuple where Item1 is a string, Item2 is an int, Item3 is a string.
        /// Item1 will depend on the result code.
        /// Item1 will be the formatted string if Item2 = 0,
        /// Else, it will return the original string.
        /// Item2 is a result code on the result of testing the field.
        /// If the string is formatted sufficiently, Item2 = 0.
        /// If the string isn't formatted sufficiently, Item2 = 1.
        /// If the string contains numbers out of the range 1-31, Item2 = 2.
        /// If the string has numbers out of range, Item3 is a comma separated string
        /// indicating which numbers are out of range.
        /// </returns>
        private Tuple<string, int, string> GetMDays(string mDayString)
        {
            mDayString.Trim();
            string dayString = "";

            //pattern for comma separated numbers that are one or two digits in length
            Regex rx = new Regex(@"^[\s]*\d{1,2}[\s]*([,]\s*\d{1,2})*\s*$");

            Match m = rx.Match(mDayString);

            if(!m.Success)
            {
                return new Tuple<string, int, string>(mDayString, 1, "");
            }

            //Remove whitespace
            dayString = mDayString.Replace(" ", "");

            string[] days = dayString.Split(',');

            List<string> invalidDays = new List<string>();

            //Check that each number is in the range 1-31
            foreach(string day in days)
            {
                int d = 0;
                try
                {
                    d = Convert.ToInt32(day);
                }
                catch(FormatException e)
                {
                    //Do nothing, the number will be 0;
                }
                if(d < 1 || d > 31)
                {
                    invalidDays.Add(day);
                    //return new Tuple<string, int, string>(mDayString, 2, day);
                }
            }

            string invalidDaysString = string.Join(", ", invalidDays.ToArray());
            if(invalidDaysString.Length > 0)
            {
                return new Tuple<string, int, string>(mDayString, 2, invalidDaysString);
            }

            return new Tuple<string, int, string>(dayString, 0, "");
        }

        /// <summary>
        /// Returns the string of comma separated boolean values (as 0,1) of days to backup
        /// </summary>
        /// <returns></returns>
        private string GetWDaysString()
        {
            string days = "";

            days += chk_Sun.IsChecked == true ? 1 : 0;
            days += ",";
            days += chk_Mon.IsChecked == true ? 1 : 0;
            days += ",";
            days += chk_Tue.IsChecked == true ? 1 : 0;
            days += ",";
            days += chk_Wed.IsChecked == true ? 1 : 0;
            days += ",";
            days += chk_Thu.IsChecked == true ? 1 : 0;
            days += ",";
            days += chk_Fri.IsChecked == true ? 1 : 0;
            days += ",";
            days += chk_Sat.IsChecked == true ? 1 : 0;

            return days;
        }
    }
}
