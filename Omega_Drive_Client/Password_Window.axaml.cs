using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Omega_Drive_Client
{
    public partial class Password_Window : Window
    {
        private Log_In__Or__Register current_log_in_or_register_instance;
        private static Application_Cryptographic_Services application_cryptographic_services = new Application_Cryptographic_Services();
        private static Server_Connections Server_Connections = new Server_Connections();
        private delegate void Option_Function_Invoker();


        private readonly Dictionary<Password_Function_Selection, string> option_value_pair = new Dictionary<Password_Function_Selection, string>()
        {
            { Password_Function_Selection.LogIn, "Log in code" },
            { Password_Function_Selection.UnValidatedAccount, "Account validation code" }
        };


        private readonly Dictionary<Password_Function_Selection, Option_Function_Invoker> option_delegate_value_pair = new Dictionary<Password_Function_Selection, Option_Function_Invoker>();

        private Password_Function_Selection Option;
        private string Path;




        public enum Password_Function_Selection
        {
            LogIn,
            UnValidatedAccount
        }





        public Password_Window()
        {
            InitializeComponent();
        }


        public Password_Window(Password_Function_Selection option, string path)
        {
            Option = option;
            Path = path;

            InitializeComponent();
        }


        public Password_Window(Password_Function_Selection option, string path, Log_In__Or__Register instance)
        {
            Option = option;
            Path = path;
            current_log_in_or_register_instance = instance;

            InitializeComponent();
        }



        private void Window_Opened(object obj, EventArgs e)
        {
            option_delegate_value_pair.TryAdd(Password_Function_Selection.LogIn, new Option_Function_Invoker(Accept_Log_In_Code));
            option_delegate_value_pair.TryAdd(Password_Function_Selection.UnValidatedAccount, new Option_Function_Invoker(Accept_Account_Validation_Code));



            string value = String.Empty;
            option_value_pair.TryGetValue(Option, out value);
            Window_TextBlock.Text = value;
        }




        private void Accept_Password(object obj, RoutedEventArgs e)
        {
            Option_Function_Invoker option_Function_Invoker = new Option_Function_Invoker(Accept_Log_In_Code);

            option_delegate_value_pair.TryGetValue(Option, out option_Function_Invoker);

            option_Function_Invoker.Invoke();
        }



        private async void Accept_Log_In_Code()
        {
            string result = System.Text.Encoding.UTF8.GetString(await Server_Connections.Secure_Server_Connections("Account authentification", Password_TextBox.Text, null));

            System.Diagnostics.Debug.WriteLine("Result: " + result);

            Client_Application_Variables.Function_Result_Processing(Client_Application_Variables.Selected_Function.Authentificate_Account, result, new Tuple<object, object>(this, current_log_in_or_register_instance));
        }




        private async void Accept_Account_Validation_Code()
        {
            string result = System.Text.Encoding.UTF8.GetString(await Server_Connections.Secure_Server_Connections("Account validation", Password_TextBox.Text, null));

            System.Diagnostics.Debug.WriteLine("Result: " + result);

            Client_Application_Variables.Function_Result_Processing(Client_Application_Variables.Selected_Function.Validate_Account, result, this);
        }
    }
}
