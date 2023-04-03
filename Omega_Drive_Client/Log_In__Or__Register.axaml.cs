using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using System.Text;
using System.Drawing;
using System.Security.AccessControl;

namespace Omega_Drive_Client
{
    public partial class Log_In__Or__Register : Window
    {
        private static Server_Connections server_connections = new Server_Connections();

        private sealed class Client_Application_Variables_Mitigator : Client_Application_Variables
        {
            internal static async Task<bool> Load_Application_File_Settings_Initiator()
            {
                return await Read_Application_Settings_File();
            }
        }

        public Log_In__Or__Register()
        {
            InitializeComponent();
        }

        private async void Window_Opened(object sender, EventArgs e)
        {
            Log_In_Email_TextBox.Text = String.Empty;
            Log_In_Password_TextBox.Text = String.Empty;

            Register_Email_TextBox.Text = String.Empty;
            Register_Password_TextBox.Text = String.Empty;
            Register_Repeat_Password_TextBox.Text = String.Empty;

            await Client_Application_Variables_Mitigator.Load_Application_File_Settings_Initiator();
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


        private async void Log_In_User(object sender, RoutedEventArgs e)
        {
            string server_payload = Encoding.UTF8.GetString(await server_connections.Secure_Server_Connections("Log in", Log_In_Email_TextBox.Text, Encoding.UTF8.GetBytes(Log_In_Password_TextBox.Text)));

            if(server_payload != "Log in successful")
            {
                Notification_Window notification_Window = new Notification_Window(server_payload);
                await notification_Window.ShowDialog(this);

                if (server_payload == "Un-validated account")
                {
                    Password_Window password_Window = new Password_Window(server_payload, null);
                    await password_Window.ShowDialog(this);
                }
            }
        }



        private void Keep_User_Logged_In(object sender, RoutedEventArgs e)
        {
            Registration_Panel.IsVisible = false;
            Log_In_Panel.IsVisible = true;
        }



        private async void Resgister_User(object sender, RoutedEventArgs e)
        {
            if(Register_Password_TextBox.Text == Register_Repeat_Password_TextBox.Text)
            {
                byte[] server_payload = await server_connections.Secure_Server_Connections("Register", Register_Email_TextBox.Text, Encoding.UTF8.GetBytes(Register_Repeat_Password_TextBox.Text));
                string account_registration_result = Encoding.UTF8.GetString(server_payload);



                Notification_Window notification_Window = new Notification_Window(account_registration_result);
                await notification_Window.ShowDialog(this);

                if (account_registration_result == "Registration successful")
                {
                    Register_Email_TextBox.Text = String.Empty;
                    Register_Password_TextBox.Text = String.Empty;
                    Register_Repeat_Password_TextBox.Text = String.Empty;

                    Open_Log_In_Panel(this, e);
                }
            }
            else
            {
                Notification_Window notification_Window = new Notification_Window("Passwords do not match");
                await notification_Window.ShowDialog(this);
            }
        }

        private void Open_Settings_Page(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog(this);
        }
    }
}
