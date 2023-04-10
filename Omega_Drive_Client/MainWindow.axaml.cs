using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Omega_Drive_Client.Client_Application_Variables;

namespace Omega_Drive_Client
{
    public partial class MainWindow : Window
    {
        private static Server_Connections Server_Connections = new Server_Connections();


        private static List<Files> database_files = new List<Files>();

        private System.Timers.Timer Animation_Timer;

        private bool Expand_The_Menu_Label;

        private byte Main_Menu_Opened_Or_Closed;

        private bool Files_Loaded;

        public class Files
        {
            public string FileName { get; set; }

            public string Size { get; set; }

            public string Date { get; set; }


            public CheckBox Check { get; set; }
        }



        public MainWindow()
        {
            InitializeComponent();
        }


        private async void Window_Opened(object sender, EventArgs e)
        {
            Animation_Timer = new System.Timers.Timer();
            Animation_Timer.Elapsed += Animation_Timer_Elapsed;
            Animation_Timer.Interval = 10;
            Animation_Timer.Start();

            await Load_User_Files();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if(Animation_Timer != null)
            {
                Animation_Timer.Stop();
                Animation_Timer.Close();
                Animation_Timer.Dispose();
            }
        }

        private void Mouse_Over(object sender, PointerEventArgs e)
        {
            Expand_The_Menu_Label = true;
        }

        private void Mouse_Not_Over(object sender, PointerEventArgs e)
        {
            Expand_The_Menu_Label = false;
        }

        private void Open_Or_Close_The_Main_Menu(object sender, RoutedEventArgs e)
        {
            Main_Menu_Opened_Or_Closed++;
        }

        private async void Open_The_File_Download_Section(object sender, RoutedEventArgs e)
        {
            Download_Stackpanel.IsVisible = true;
            Download_Stackpanel.IsEnabled = true;

            Upload_Stackpanel.IsVisible = false;
            Upload_Stackpanel.IsEnabled = false;

            await Load_User_Files();
        }

        private void Open_The_File_Upload_Section(object sender, RoutedEventArgs e)
        {
            Upload_Stackpanel.IsVisible = true;
            Upload_Stackpanel.IsEnabled = true;

            Download_Stackpanel.IsVisible = false;
            Download_Stackpanel.IsEnabled = false;
        }

        private async void Open_The_Settings_Menu(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            await settings.ShowDialog(this);
        }

        private async void Log_Out(object sender, RoutedEventArgs e)
        {
            string result = Encoding.UTF8.GetString(await Server_Connections.Secure_Server_Connections("Log out", Client_Application_Variables.log_in_session_key, null));
            Client_Application_Variables.Function_Result_Processing(Client_Application_Variables.Selected_Function.Log_Out, result, this);
        }

        private async Task<bool> Load_User_Files()
        {
            string result = Encoding.UTF8.GetString(await Server_Connections.Secure_Server_Connections("Retrieve user files data", Client_Application_Variables.log_in_session_key, null));
            Client_Application_Variables.Function_Result_Processing(Client_Application_Variables.Selected_Function.User_Files_Information_Retrieval, result, this);
            System.Diagnostics.Debug.WriteLine("Result: " + result);
            return true;
        }

        private void Animation_Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {

                if (Expand_The_Menu_Label == true)
                {
                    Menu_TextBlock.Width += 10;
                }
                else
                {
                    if(Main_Menu_Opened_Or_Closed == 0)
                    {
                        Menu_TextBlock.Width = 0;
                    }
                }



                if (Main_Menu_Opened_Or_Closed == 1)
                {
                    if(Main_Menu_StackPanel.Width < 180)
                    {
                        Main_Menu_StackPanel.Width += 10;
                    }
                }
                else
                {
                    Main_Menu_StackPanel.Width = 0;
                    Main_Menu_Opened_Or_Closed = 0;
                }


            }, Avalonia.Threading.DispatcherPriority.Background);
        }
    }
}
