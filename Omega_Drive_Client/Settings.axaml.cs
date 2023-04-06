using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    public partial class Settings : Window
    {   /*
        var certStore = new System.Security.Cryptography.X509Certificates.X509Store(System.Security.Cryptography.X509Certificates.StoreName.Root, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);
        certStore.Open(System.Security.Cryptography.X509Certificates.OpenFlags.MaxAllowed);

        var certs = string.Empty;
        foreach (var c in certStore.Certificates)
        {
            System.Diagnostics.Debug.WriteLine("Cert: " + c);
        }
        */

        private static System.Net.IPAddress address = System.Net.IPAddress.Parse("127.0.0.1");

        public Settings()
        {
            InitializeComponent();
        }

        private void Window_Opened(object obj, EventArgs e)
        {
            address = Client_Application_Variables.Get_IP_Address();
            IP_TextBox.Text = address.ToString();
            Port_TextBox.Text = (Client_Application_Variables.Get_Port_Number()).ToString();
            Choose_Protocol("Current");
        }

        private void IP_TextBox_Lost_Focus(object obj, RoutedEventArgs e)
        {
            string buffer = address.ToString();
            System.Net.IPAddress.TryParse(IP_TextBox.Text, out address);

            if(address != null)
            {
                IP_TextBox.Text = address.ToString();
                Client_Application_Variables.Set_IP_Address(address);
            }
            else
            {
                System.Net.IPAddress.TryParse(buffer, out address);
                IP_TextBox.Text = address.ToString();
            }
        }

        private void Port_TextBox_Lost_Focus(object? sender, RoutedEventArgs e)
        {
            try
            {
                double port_number = Convert.ToDouble(Port_TextBox.Text);

                if(port_number < 65535)
                {
                    Client_Application_Variables.Set_Port_Number((int)port_number);
                }
                else
                {
                    Port_TextBox.Text = (Client_Application_Variables.Get_Port_Number()).ToString();
                }
            }
            catch
            {
                Port_TextBox.Text = (Client_Application_Variables.Get_Port_Number()).ToString();
            }
        }


        private void Previous_Protocol(object obj, RoutedEventArgs e)
        {

            Choose_Protocol("Previous");
        }

        private void Next_Protocol(object obj, RoutedEventArgs e)
        {
            Choose_Protocol("Next");
        }


        private async void Load_SSL_Certificate(object obj, RoutedEventArgs e)
        {
            OpenFileDialog certificate_file_dialog = new OpenFileDialog();

            FileDialogFilter filter = new FileDialogFilter();
            filter.Extensions = new List<string>() {"crt"};

            try
            {
                if (filter != null && certificate_file_dialog != null)
                {
                    Point settings_window_coordinates = new Point(this.Width * 2, this.Screens.All[0].Bounds.Height / 1.33 - this.Height);
                    int one_to_one_scale = 1;

                    this.Position = Avalonia.PixelPoint.FromPoint(settings_window_coordinates, one_to_one_scale);

                    certificate_file_dialog.Filters.Add(filter);
                    certificate_file_dialog.AllowMultiple = false;

                    string[] certificate_path = await certificate_file_dialog.ShowAsync(this);
                    

                    if(certificate_path != null)
                    {
                        if(this != null)
                        {
                            Password_Window password = new Password_Window(Password_Window.Password_Function_Selection.SslCertificate, certificate_path[0]);
                            await password.ShowDialog(this);
                        }
                    }
                }
            }
            catch(Exception E)
            {
                System.Diagnostics.Debug.WriteLine(E.Message);
            }
           
        }


        private async void Choose_Protocol(string option)
        {
            await Dispatcher.UIThread.InvokeAsync(async() =>
            {
                switch(option)
                {
                    case "Next":
                        Client_Application_Variables.Increment_Current_Protocol_Index();
                        Current_Protocol_TextBlock.Text = Client_Application_Variables.Get_And_Set_Current_Protocol();
                        await Client_Application_Variables.Settings_File_Operation_Selector(Client_Application_Variables.Settings_File_Option.Update_Settings_File);
                        break;

                    case "Current":
                        Current_Protocol_TextBlock.Text = Client_Application_Variables.Get_And_Set_Current_Protocol();
                        break;

                    case "Previous":
                        Client_Application_Variables.Decrement_Current_Protocol_Index();
                        Current_Protocol_TextBlock.Text = Client_Application_Variables.Get_And_Set_Current_Protocol();
                        await Client_Application_Variables.Settings_File_Operation_Selector(Client_Application_Variables.Settings_File_Option.Update_Settings_File);
                        break;
                }
            });
        }
    }
}
