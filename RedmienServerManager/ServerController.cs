using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;

namespace RedmineServerManager
{
    class ServerController
    {
        int status;
        string vmname;

        public event Action<int, string> ArchiveComplete;

        BackgroundWorker archiver;

        Process child;

        public ServerController()
        {
            this.vmname = Properties.Settings.Default.VMName;
            status = 0;
            archiver = new BackgroundWorker();
            archiver.DoWork += new DoWorkEventHandler(archiver_DoWork);
            archiver.ProgressChanged += new ProgressChangedEventHandler(archiver_ProgressChanged);
            archiver.RunWorkerCompleted += new RunWorkerCompletedEventHandler(archiver_RunWorkerCompleted);
            archiver.WorkerReportsProgress = true;
            archiver.WorkerSupportsCancellation = true;
        }

        void archiver_DoWork(object sender, DoWorkEventArgs e)
        {
            string msg = "";
            //set up child process
            string sourceName = Properties.Settings.Default.VMlocation;
            string targetName = Properties.Settings.Default.SaveLocation;

            // Initialize process information.
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = "7z.exe";
            p.RedirectStandardOutput = true;
            p.UseShellExecute = false;
            p.CreateNoWindow = true;

            // Use 7-zip
            // specify a=archive and -t7z=7z
            // and then target file in quotes followed by source file in quotes
            p.Arguments = "a -t7z -mx3 \"" + targetName + "\" \"" + sourceName + "\" ";
            p.WindowStyle = ProcessWindowStyle.Normal;

            // Start process and wait for it to exit
            Process x = Process.Start(p);
            x.EnableRaisingEvents = true;
            x.OutputDataReceived += x_Output;
            x.Exited += x_Exited;

            string output = "";

            //if (x.StandardOutput != null)
            //{
            //    output = x.StandardOutput.ReadToEnd();
            //}

            this.status = 2;

            //x.WaitForExit();
            System.Windows.MessageBox.Show("Nothing seems to work");
            msg += output;

            msg += "VM loaction: " + sourceName + "\n";
            msg += "Archive Target: " + targetName + "\n";
            msg += "Arguments: " + p.Arguments + "\n";

            for(int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                archiver.ReportProgress(i);
                if(archiver.CancellationPending)
                {
                    e.Cancel = true;
                    archiver.ReportProgress(0);
                    return;
                }
            }

            archiver.ReportProgress(100);
        }

        void archiver_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            System.Windows.MessageBox.Show(e.ProgressPercentage.ToString());
        }

        void archiver_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string msg = "";
            if(e.Cancelled)
            {
                msg = "The archiving process was cancelled.";
            }
            else if(e.Error != null)
            {
                msg = "Error While performing archival procedure.";
            }
            else
            {
                msg = "The task was completed.";
            }
            System.Windows.MessageBox.Show(msg, "Notice");
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// Runs a child process to see if the virtual machine is running.
        /// 
        /// </summary>
        /// <returns>
        /// 2-tuple. 
        /// int::
        /// 0: off
        /// 1: running
        /// 2: archiving
        /// 3: booting
        /// 4: shutting down
        /// string is return message
        /// </returns>
        public Tuple<int, string> CheckStatus()
        {
            string msg = "";
            //Check if the vm is running
            Tuple<int, string> check = CheckIsRunning();

            msg += check.Item2;

            //if vm is not running, check if it's getting archived
            if (status == 0)
            {
                msg += "'" + vmname + "' is not running. Checking archive status.\n";
                //Check if 7zip is archiving, append the output
                msg += CheckIsArchiving().Item2;
            }


            return new Tuple<int, string>(status, msg);
        }

        private Tuple<int, string> CheckIsArchiving()
        {
            string msg = "";
            //set up child process
            child = new Process();
            child.StartInfo.UseShellExecute = false;
            child.StartInfo.RedirectStandardOutput = true;
            child.StartInfo.CreateNoWindow = true;

            //Run hidden command prompt
            child.StartInfo.FileName = "cmd.exe";

            //Using vboxmanage, check if there is a virtual machine by the name in settings running
            //this command will return the number of machines running by the given name
            child.StartInfo.Arguments = "/C tasklist | find \"7z\" | find /c /v \"~~~\"";
            child.Start();

            int num = -1;
            string output = "";

            //if the process has output, get the first line
            if (child.StandardOutput != null)
            {
                output = child.StandardOutput.ReadLine();
            }

            //Regex for a digit (number)
            Regex r = new Regex(@"\d");

            //debug
            msg += output + "\n";

            //Check if the output contains a number
            //if so, store the number and convert it to an int
            //otherwise, log an error that the output couldn't be parsed.
            Match m = r.Match(output);
            if (m.Success)
            {
                try
                {
                    num = Convert.ToInt32(m.Value);
                }
                catch (FormatException e)
                {
                    msg += "The number of archiving tasks couldn't be parsed.\n";
                    num = -1;
                }
            }

            //log the rest of the output
            msg += child.StandardOutput.ReadToEnd();

            //wait for the command to finish
            child.WaitForExit();

            //if at least one vm running, return status running
            if (num > 0)
                return new Tuple<int, string>(status = 2, msg);
            return new Tuple<int, string>(status, msg);
            
        }

        private Tuple<int, string> CheckIsRunning()
        {
            string msg = "";
            //set up child process
            child = new Process();
            child.StartInfo.UseShellExecute = false;
            child.StartInfo.RedirectStandardOutput = true;
            child.StartInfo.CreateNoWindow = true;

            //Run hidden command prompt
            child.StartInfo.FileName = "cmd.exe";

            //Using vboxmanage, check if there is a virtual machine by the name in settings running
            //this command will return the number of machines running by the given name
            child.StartInfo.Arguments = "/C vboxmanage list runningvms | find \"" + vmname + "\" | find /c /v \"~~~\"";
            child.Start();

            int num = -1;
            string output = "";

            //if the process has output, get the first line
            if (child.StandardOutput != null)
            {
                output = child.StandardOutput.ReadLine();
            }

            //Regex for a digit (number)
            Regex r = new Regex(@"\d");

            //debug
            msg += output + "\n";

            //Check if the output contains a number
            //if so, store the number and convert it to an int
            //otherwise, log an error that the output couldn't be parsed.
            Match m = r.Match(output);
            if (m.Success)
            {
                try
                {
                    num = Convert.ToInt32(m.Value);
                }
                catch (FormatException e)
                {
                    msg += "The number of virtual machine's by the name '" + vmname + "', couldn't be parsed.\n";
                    num = -1;
                }
            }

            //log the rest of the output
            msg += child.StandardOutput.ReadToEnd();

            //wait for the command to finish
            child.WaitForExit();

            //if at least one vm running, return status running
            if (num > 0)
                return new Tuple<int, string>(status = 1, msg);
            return new Tuple<int, string>(status = 0, msg);
        }

        public Tuple<int, string> StopVM()
        {
            string msg = "";
            //set up child process
            child = new Process();
            child.StartInfo.UseShellExecute = false;
            child.StartInfo.RedirectStandardOutput = true;
            child.StartInfo.CreateNoWindow = true;

            //Run hidden command prompt
            child.StartInfo.FileName = "cmd.exe";

            child.StartInfo.Arguments = "/C vboxmanage controlvm " + vmname + " poweroff";
            child.Start();

            string output = "";

            //if the process has output, get the first line
            if (child.StandardOutput != null)
            {
                output = child.StandardOutput.ReadToEnd();
            }

            msg += output + "\n";

            child.WaitForExit();

            CheckStatus(); //updates global status
            return new Tuple<int, string>(status, msg);
        }

        public Tuple<int, string> StartVM()
        {
            string msg = "";
            //set up child process
            child = new Process();
            child.StartInfo.UseShellExecute = false;
            child.StartInfo.RedirectStandardOutput = true;
            child.StartInfo.CreateNoWindow = true;

            //Run hidden command prompt
            child.StartInfo.FileName = "cmd.exe";

            child.StartInfo.Arguments = "/C vboxmanage startvm " + vmname /*+ "--type headless" */;
            child.Start();

            string output = "";

            //if the process has output, get it
            if (child.StandardOutput != null)
            {
                output = child.StandardOutput.ReadToEnd();
            }

            msg += output + "\n";

            child.WaitForExit();

            CheckStatus(); //updates global status
            return new Tuple<int, string>(status, msg);
        }


        /// <summary>
        /// Runs the archive procedure using the embedded 7zip component.
        /// </summary>
        /// <returns></returns>
        internal Tuple<int, string> ArchiveVM()
        {
            archiver.RunWorkerAsync();

            string msg = "no message yet..";

            //CheckStatus(); //updates global status
            return new Tuple<int, string>(status, msg);
        }

        private void x_Output(object sender, DataReceivedEventArgs e)
        {
            System.Windows.MessageBox.Show(e.Data);
        }

        private void x_Exited(object sender, EventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            CheckStatus();
            ArchiveComplete(status, "It worked!");
        }

    }
}
