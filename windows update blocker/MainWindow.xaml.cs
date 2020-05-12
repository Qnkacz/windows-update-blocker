using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
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
using Microsoft.Win32;

namespace windows_update_blocker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServiceController[] scServices;
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
            foreach (var item in scServices)
            {
                if(item.DisplayName == "Windows Update")
                {
                    item.Refresh();
                    if (item.Status.Equals(ServiceControllerStatus.Stopped) || item.Status.Equals(ServiceControllerStatus.StopPending))
                    {
                        MessageBox.Show("Allready stopped babyyyy","Status",MessageBoxButton.OK);
                    }
                    else
                    {
                        item.Stop();
                    }
                }
            }
        }

        private void unBlock_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in scServices)
            {
                if (item.DisplayName == "Windows Update")
                {
                    item.Refresh();
                    if (item.Status.Equals(ServiceControllerStatus.Running))
                    {
                        MessageBox.Show("Allready running babyyyy", "Status", MessageBoxButton.OK);
                    }
                    else
                    {
                        item.Start();
                        MessageBox.Show("started");
                    }
                }
            }
        }
        private void ChangeStatusOfService(bool status)
        {

        }
    }
}
