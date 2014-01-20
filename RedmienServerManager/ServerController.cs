using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RedmineServerManager
{
    class ServerController
    {
        bool running;
        string vmname;

        Process child;

        public ServerController()
        {
            this.vmname = Properties.Settings.Default.VMName;
            running = false;
        }

        public bool Running
        {
            get { return running; }
            set { running = value; }
        }

        /// <summary>
        /// Runs a child process to see if the virtual machine is running.
        /// 
        /// </summary>
        /// <returns>
        /// 2-tuple. 
        /// bool is true if running, else false.
        /// string is return message
        /// </returns>
        public Tuple<bool, string> CheckIsRunning()
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

            //if at least one vm running, return true
            if (num > 0)
                running = true;
            else 
                running = false;

            return new Tuple<bool, string>(running, msg);
        }

        public Tuple<bool, string> StopVM()
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

            running = false;
            return new Tuple<bool, string>(true, msg);
        }

    }
}
