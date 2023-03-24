using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    public partial class Log_In__Or__Register : Window
    {
        private static Server_Connections server_connections = new Server_Connections();
        private static Payload_Serialization client_payload = new Payload_Serialization();

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
            byte[] serialized_client_payload = await client_payload.Serialize_Payload<string>("Log in", Log_In_Email_TextBox.Text, Log_In_Password_TextBox.Text);
            byte[] serialized_server_payload = await server_connections.Secure_Server_Connections(serialized_client_payload);

            Server_WSDL_Payload server_WSDL_Payload = await client_payload.Deserialize_Payload(serialized_server_payload);

            System.Diagnostics.Debug.WriteLine("SERVER PAYLOAD IS: " + server_WSDL_Payload.Server_Payload);
        }

        private void Keep_User_Logged_In(object sender, RoutedEventArgs e)
        {
            Registration_Panel.IsVisible = false;
            Log_In_Panel.IsVisible = true;
        }


        private void Resgister_User(object sender, RoutedEventArgs e)
        {
            
        }

        private void Open_Settings_Page(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog(this);
        }
    }
}
