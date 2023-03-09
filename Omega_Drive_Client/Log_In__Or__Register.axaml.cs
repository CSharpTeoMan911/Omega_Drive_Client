using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Omega_Drive_Client
{
    public partial class Log_In__Or__Register : Window
    {

        public Log_In__Or__Register()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("User: " + Environment.UserName);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            
        }

        private void Open_Log_In_Panel(object sender, RoutedEventArgs e)
        {
            Registration_Panel.IsVisible = false;
            Log_In_Panel.IsVisible = true;
        }


        private void Open_Registration_Panel(object sender, RoutedEventArgs e)
        {
            Log_In_Panel.IsVisible = false;
            Registration_Panel.IsVisible = true;
        }

        private void Log_In_User(object sender, RoutedEventArgs e)
        {
            Registration_Panel.IsVisible = false;
            Log_In_Panel.IsVisible = true;
        }

        private void Keep_User_Logged_In(object sender, RoutedEventArgs e)
        {
            Registration_Panel.IsVisible = false;
            Log_In_Panel.IsVisible = true;
        }


        private void Resgister_User(object sender, RoutedEventArgs e)
        {
            Registration_Panel.IsVisible = false;
            Log_In_Panel.IsVisible = true;
        }

        private void Open_Settings_Page(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog(this);
        }
    }
}
