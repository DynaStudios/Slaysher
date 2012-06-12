using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using updateSystemDotNet;

namespace Launcher
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();
        }

        private void LoadLauncher(object sender, EventArgs e)
        {
            button1.Enabled = false;
            label1.Text = "Checking for Updates...";

            updateController1.checkForUpdatesAsync();
        }

        private void DownloadProgressUpdate(object sender, updateSystemDotNet.appEventArgs.downloadUpdatesProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void UpdateCheckComplete(object sender, updateSystemDotNet.appEventArgs.checkForUpdatesCompletedEventArgs e)
        {
            if (!updateController1.currentUpdateResult.UpdatesAvailable)
            {
                progressBar1.Value = 100;
                label1.Text = "No new Updates available";
                button1.Enabled = true;
            }
        }

        private void updateController1_updateFound(object sender, updateSystemDotNet.appEventArgs.updateFoundEventArgs e)
        {
            label1.Text = "Downloading new Update";
            updateController1.downloadUpdates();
        }

        private void DownloadUpdatesCompleted(object sender, AsyncCompletedEventArgs e)
        {
            label1.Text = "Download complete. Start Patching...";
            updateController1.applyUpdate();
        }

        private void PlayGameClicked(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = Application.StartupPath + @"\Game";
            startInfo.FileName = "Slaysher.exe";
            Process.Start(startInfo);

            Close();
        }
    }
}