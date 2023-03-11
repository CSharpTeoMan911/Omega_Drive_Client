using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;

namespace Omega_Drive_Client
{
    public partial class Settings : Window
    {

       

                                    /*
                                    var certStore = new System.Security.Cryptography.X509Certificates.X509Store(System.Security.Cryptography.X509Certificates.StoreName.Root, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);
                                    certStore.Open(System.Security.Cryptography.X509Certificates.OpenFlags.MaxAllowed);

                                    var certs = string.Empty;
                                    foreach (var c in certStore.Certificates)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Cert: " + c);
                                    }
                                    */

        public Settings()
        {
            InitializeComponent();
        }



        private void Window_Initialised(object obj, EventArgs e)
        {
            
        }

        private void IP_TextBox_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {
            string buffer = String.Empty;

            for(int index = 0; index < IP_TextBox.Text.Length; index++)
            {
                if(IP_TextBox.Text[index] != '.')
                {

                }
            }
        }


        private void Port_TextBox_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {

        }


        private async void Load_SSL_Certificate(object obj, RoutedEventArgs e)
        {
            OpenFileDialog certificate_file_dialog = new OpenFileDialog();

            FileDialogFilter filter = new FileDialogFilter();
            filter.Extensions = new List<string>() {".crt"};

            try
            {
                if (filter != null && certificate_file_dialog != null)
                {

                    certificate_file_dialog.Filters.Add(filter);
                    string[] certificate_path = await certificate_file_dialog.ShowAsync(this);
                    
                    if(certificate_path != null)
                    {
                        try
                        {
                            System.Security.Cryptography.X509Certificates.X509Certificate2 server_certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate_path[0], "OMEGA");

                            try
                            {



                                System.Security.Cryptography.X509Certificates.X509Store certificate_store = new System.Security.Cryptography.X509Certificates.X509Store( System.Security.Cryptography.X509Certificates.StoreName.Root, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);

                                try
                                {
                                    certificate_store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadWrite);
                                    certificate_store.Add(server_certificate);
                                }
                                catch (Exception E)
                                {
                                    
                                }
                                finally
                                {
                                    if (certificate_store != null)
                                    {
                                        certificate_store.Dispose();
                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                
                            }
                            finally
                            {
                                if (server_certificate != null)
                                {
                                    server_certificate.Dispose();
                                }
                            }
                        }
                        catch (Exception E)
                        {
                            
                        }
                    }
                }
            }
            catch
            {

            }
           
        }
    }
}
