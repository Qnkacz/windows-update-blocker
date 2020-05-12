using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace windows_update_blocker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServiceController[] scServices;
        ServiceController wu = null;
        private static readonly Regex _regex = new Regex("[^0-9]");
        public bool shouldloop=false;
        System.Timers.Timer timer = null;
        private void SetStartup(bool mode)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", mode);

            rk.SetValue("Windows Update blocker", Process.GetCurrentProcess().MainModule.FileName.ToString());
        }
        public MainWindow()
        {
            InitializeComponent();
            GetServices();
        }

        private void GetServices()
        {
            try
            {
                scServices = ServiceController.GetServices();
                foreach (var item in scServices)
                {
                    if (item.DisplayName == "Windows Update")
                    {
                        wu = item;
                    }
                }
            }
            catch
            {

            }
        }

        private void autostart_no_Checked(object sender, RoutedEventArgs e)
        {
            SetStartup(false);
            MessageBox.Show("This programm will not open on startup", "message", MessageBoxButton.OK);
        }

        private void autostart_yes_Checked(object sender, RoutedEventArgs e)
        {
            SetStartup(true);
            MessageBox.Show("This programm will open on startup", "message", MessageBoxButton.OK);
        }

        private void Block_Click(object sender, RoutedEventArgs e)
        {
            shouldloop = true;
            ChangeStatusOfService(false);
            loopBlock();
        }

        private void unBlock_Click(object sender, RoutedEventArgs e)
        {
            shouldloop = false;
            ChangeStatusOfService(true);
        }
        private void ChangeStatusOfService(bool status)
        {
            wu.Refresh();
            if (status == false)
            {
                if (wu.Status.Equals(ServiceControllerStatus.Stopped) || wu.Status.Equals(ServiceControllerStatus.StopPending))
                {
                    
                }
                else
                {
                    wu.Stop();
                }
            }
            else
            {
                if (wu.Status.Equals(ServiceControllerStatus.Running))
                {
                    
                }
                else
                {
                    wu.Start();
                }
            }
           
        }

        private void time_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void time_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Space)
            {
                time.Text = time.Text.Substring(0, time.Text.Length - 1);
            }
        }
        private void loopBlock()
        {
            if(time.Text!=null)
            {
                while (shouldloop == true)
                {
                    Thread.Sleep(Int32.Parse(time.Text) * 60 * 1000);
                    ChangeStatusOfService(false);
                }
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized) this.Hide();

            base.OnStateChanged(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            // setting cancel to true will cancel the close request
            // so the application is not closed
            e.Cancel = true;

            this.Hide();

            base.OnClosing(e);
        }
    }
}
