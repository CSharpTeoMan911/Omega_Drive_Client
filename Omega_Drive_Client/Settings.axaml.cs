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

        private sealed class Client_Application_Variables_Mitigator : Client_Application_Variables
        {
            internal static Task<int> Get_Port_Number()
            {
                return Task.FromResult(port_number);
            }

            internal static async Task<bool> Set_Port_Number(int port_num)
            {
                port_number = port_num;

                await Update_Application_Settings_File();

                return true;
            }

            internal static Task<System.Net.IPAddress> Get_IP_Address()
            {
                return Task.FromResult(ip_address);
            }

            internal static async Task<bool> Set_IP_Address(System.Net.IPAddress ip_addr)
            {
                ip_address = ip_addr;

                await Update_Application_Settings_File();

                return true;
            }

            internal static Task<bool> Increment_Current_Protocol_Index()
            {
                lock (list_of_available_protocols)
                {
                    if (current_protocol + 1 < list_of_available_protocols.Count)
                    {
                        current_protocol += 1;
                    }
                }

                return Task.FromResult(true);
            }


            internal static Task<bool> Decrement_Current_Protocol_Index()
            {
                lock (list_of_available_protocols)
                {
                    if (current_protocol > 0)
                    {
                        current_protocol -= 1;
                    }
                }

                return Task.FromResult(true);
            }

            internal static async Task<string> Get_Current_Protocol()
            {
                string current_selected_protocol = String.Empty;

                lock (list_of_available_protocols)
                {
                    current_selected_protocol = list_of_available_protocols[current_protocol];

                    switch (current_selected_protocol)
                    {
                        case "Tls 1.3":
                            ssl_protocol = System.Security.Authentication.SslProtocols.Tls13;
                            break;

                        case "Tls 1.2":
                            ssl_protocol = System.Security.Authentication.SslProtocols.Tls12;
                            break;

                        case "Tls 1.1":
                            ssl_protocol = System.Security.Authentication.SslProtocols.Tls11;
                            break;

                        case "Tls":
                            ssl_protocol = System.Security.Authentication.SslProtocols.Tls;
                            break;

                        case "Ssl V3":
                            #pragma warning disable CS0618 // Type or member is obsolete
                            ssl_protocol = System.Security.Authentication.SslProtocols.Ssl3;
                            #pragma warning restore CS0618 // Type or member is obsolete
                            break;

                        case "Ssl V2":
                            #pragma warning disable CS0618 // Type or member is obsolete
                            ssl_protocol = System.Security.Authentication.SslProtocols.Ssl2;
                            #pragma warning restore CS0618 // Type or member is obsolete
                            break;
                    }
                }

                await Update_Application_Settings_File();

                return current_selected_protocol;
            }
        }

        private async void Window_Opened(object obj, EventArgs e)
        {
            address = await Client_Application_Variables_Mitigator.Get_IP_Address();
            IP_TextBox.Text = address.ToString();
            Port_TextBox.Text = (await Client_Application_Variables_Mitigator.Get_Port_Number()).ToString();
            Choose_Protocol("Current");
        }

        private async void IP_TextBox_Lost_Focus(object obj, RoutedEventArgs e)
        {
            string buffer = address.ToString();
            System.Net.IPAddress.TryParse(IP_TextBox.Text, out address);

            if(address != null)
            {
                IP_TextBox.Text = address.ToString();
                await Client_Application_Variables_Mitigator.Set_IP_Address(address);
            }
            else
            {
                System.Net.IPAddress.TryParse(buffer, out address);
                IP_TextBox.Text = address.ToString();
            }
        }

        private async void Port_TextBox_Lost_Focus(object? sender, RoutedEventArgs e)
        {
            try
            {
                double port_number = Convert.ToDouble(Port_TextBox.Text);

                if(port_number < 65535)
                {
                    await Client_Application_Variables_Mitigator.Set_Port_Number((int)port_number);
                }
                else
                {
                    Port_TextBox.Text = (await Client_Application_Variables_Mitigator.Get_Port_Number()).ToString();
                }
            }
            catch
            {
                Port_TextBox.Text = (await Client_Application_Variables_Mitigator.Get_Port_Number()).ToString();
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
                        Password_Window password = new Password_Window("Ssl Certificate", certificate_path[0]);
                        await password.ShowDialog(this);
                    }
                }
            }
            catch
            {

            }
           
        }


        private async void Choose_Protocol(string option)
        {
            await Dispatcher.UIThread.InvokeAsync(async() =>
            {
                switch(option)
                {
                    case "Next":
                        await Client_Application_Variables_Mitigator.Increment_Current_Protocol_Index();
                        Current_Protocol_TextBlock.Text = await Client_Application_Variables_Mitigator.Get_Current_Protocol();
                        break;

                    case "Current":
                        Current_Protocol_TextBlock.Text = await Client_Application_Variables_Mitigator.Get_Current_Protocol();
                        break;

                    case "Previous":
                        await Client_Application_Variables_Mitigator.Decrement_Current_Protocol_Index();
                        Current_Protocol_TextBlock.Text = await Client_Application_Variables_Mitigator.Get_Current_Protocol();
                        break;
                }
            });
        }
    }
}
