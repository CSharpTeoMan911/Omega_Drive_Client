using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Omega_Drive_Client
{
    public partial class Password_Window : Window
    {
        private string Option;
        private string Path;


        private sealed class Application_Cryptographic_Services_Mitigator : Application_Cryptographic_Services
        {
            internal static async Task<bool> Load_Certificate_Authority_Initiator(string certificate_path, string password)
            {
                return await Load_Certificate_Authority(certificate_path, password);
            }
        }


        public Password_Window()
        {
            InitializeComponent();
        }


        public Password_Window(string option, string path)
        {
            Option = option;
            Path = path;

            InitializeComponent();
        }

        private void Window_Opened(object obj, EventArgs e)
        {
            switch(Option)
            {
                case "Ssl Certificate":
                    Window_TextBlock.Text = "Certificate password";
                    break;

                case "Log in":
                    Window_TextBlock.Text = "Log in code";
                    break;

                case "Account activation":
                    Window_TextBlock.Text = "Account activation code";
                    break;
            }
        }

        private async void Accept_Password(object obj, RoutedEventArgs e)
        {
            switch (Option)
            {
                case "Ssl Certificate":
                    bool cetificate_upload_result = await Application_Cryptographic_Services_Mitigator.Load_Certificate_Authority_Initiator(Path, Password_TextBox.Text);

                    if(cetificate_upload_result == true)
                    {
                        this.Close();
                    }
                    break;

                case "Log in":
                    break;

                case "Account activation":
                    break;
            }
        }
    }
}
