using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Windows.Controls;

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
        DispatcherTimer _DP = new DispatcherTimer();
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
            ChangeStatusOfService(false);
            if(time.Text!=null)
            {
                _DP.Interval = new TimeSpan(0, Int32.Parse(time.Text), 0);
                _DP.Tick += new EventHandler(loopBlock);
                _DP.Start();
            }
            time.IsEnabled = false;
            Button thisb = sender as Button;
            thisb.IsEnabled = false;
            unBlock.IsEnabled = true;
        }

        private void loopBlock(object sender, EventArgs e)
        {
            MessageBox.Show("started");
            ChangeStatusOfService(false);
        }

        private void unBlock_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatusOfService(true);
            _DP.Stop();
            time.IsEnabled = true;
            Button thisb = sender as Button;
            thisb.IsEnabled = false;
            Block.IsEnabled = true;
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

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized) this.Hide();

            base.OnStateChanged(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Creator: Bartosz Wąsik \n mail: Wasik.Bartosz@outlook.com \n currently searching for apprenticeships");
        }
    }
}
