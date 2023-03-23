using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Omega_Drive_Client
{
    public partial class Password_Window : Window
    {
        private static Application_Cryptographic_Services application_cryptographic_services = new Application_Cryptographic_Services();

        private string Option;
        private string Path;





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
                    bool cetificate_upload_result = await application_cryptographic_services.Load_Certificate_Authority(Path, Password_TextBox.Text);

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
