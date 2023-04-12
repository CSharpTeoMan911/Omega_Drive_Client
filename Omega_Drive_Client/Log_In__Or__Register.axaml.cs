using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using System.Text;
using System.Drawing;
using System.Security.AccessControl;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Omega_Drive_Client
{
    public partial class Log_In__Or__Register : Window
    {
        private static Server_Connections server_connections = new Server_Connections();





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



            await Client_Application_Variables.Settings_File_Operation_Selector(Client_Application_Variables.Settings_File_Option.Read_Settings_File);

            string log_in_session_key = await Client_Application_Variables.Load_User_Log_In_Session_Key_Delegate_Invoker.Invoke();

            string result = Encoding.UTF8.GetString(await server_connections.Secure_Server_Connections("Verify log in session key", log_in_session_key, null));

            Client_Application_Variables.Function_Result_Processing(Client_Application_Variables.Selected_Function.Log_in_Session_Key_Verification, result, new Tuple<string, object>(log_in_session_key, this));

            this.IsEnabled = true;
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
            System.Threading.Thread paralel_processing = new System.Threading.Thread(async () =>
            {
                string server_payload = Encoding.UTF8.GetString(await server_connections.Secure_Server_Connections("Log in", Log_In_Email_TextBox.Text, Encoding.UTF8.GetBytes(Log_In_Password_TextBox.Text)));

                Client_Application_Variables.Function_Result_Processing(Client_Application_Variables.Selected_Function.Log_In, server_payload, this);
            });

            if(System.OperatingSystem.IsWindows() == true)
            {
                paralel_processing.SetApartmentState(System.Threading.ApartmentState.STA);
            }
            paralel_processing.Priority = System.Threading.ThreadPriority.AboveNormal;
            paralel_processing.IsBackground = true;
            paralel_processing.Start();
        }



        private void Keep_User_Logged_In(object sender, RoutedEventArgs e)
        {
            Client_Application_Variables.Set_Keep_User_Logged_In(true);
        }

        private void Do_Not_Keep_User_Logged_In(object sender, RoutedEventArgs e)
        {
            Client_Application_Variables.Set_Keep_User_Logged_In(false);
        }


        private void Resgister_User(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread paralel_processing = new System.Threading.Thread(async () =>
            {
                string server_payload = INotification_Messages.passwords_do_not_match;

                if (Register_Password_TextBox.Text == Register_Repeat_Password_TextBox.Text)
                {
                    server_payload = Encoding.UTF8.GetString(await server_connections.Secure_Server_Connections("Register", Register_Email_TextBox.Text, Encoding.UTF8.GetBytes(Register_Repeat_Password_TextBox.Text)));
                }


                Client_Application_Variables.Function_Result_Processing(Client_Application_Variables.Selected_Function.Register, server_payload, this);
            });

            if (System.OperatingSystem.IsWindows() == true)
            {
                paralel_processing.SetApartmentState(System.Threading.ApartmentState.STA);
            }
            paralel_processing.Priority = System.Threading.ThreadPriority.AboveNormal;
            paralel_processing.IsBackground = true;
            paralel_processing.Start();
        }


        
        internal void Close_Window()
        {
            if(this != null)
            {
                this.Close();
            }
        }

        internal void Open_Log_In_Panel_Mitigator()
        {
            Open_Log_In_Panel(this, new RoutedEventArgs());
        }


        private void Open_Settings_Page(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog(this);
        }
    }
}
