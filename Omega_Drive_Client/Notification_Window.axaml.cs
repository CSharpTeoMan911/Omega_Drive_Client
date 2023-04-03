using Avalonia.Controls;
using System;

namespace Omega_Drive_Client
{
    public partial class Notification_Window : Window
    {
        private System.Collections.Concurrent.ConcurrentDictionary<string, string> notification_values = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
        
        private string Notification_Message;
        private string Notification_Message_Content;

        public Notification_Window(string notification_message)
        {
            bool error_messages_initiated = Initiate_Error_Messages();
            
            if(error_messages_initiated == true)
            {
                Notification_Message = notification_message;
                notification_values.TryGetValue(notification_message, out Notification_Message_Content);
            }

            InitializeComponent();
        }



        public Notification_Window()
        {
            InitializeComponent();
        }



        private bool Initiate_Error_Messages()
        {
            notification_values.TryAdd("Connection failed", "The connection to the server failed. Please ensure that the server's IP address and port number are valid, or that the SSL protocol used is the same as the one used by the server, or that the SSL certificate used is valid.");
            notification_values.TryAdd("Email already in use", "The email provided is already in use. Please use another email address.");
            notification_values.TryAdd("Registration successful", "Check your account validation code sent to this account's email address and log in.");
            notification_values.TryAdd("Invalid email address", "The email address provided is invalid.");
            notification_values.TryAdd("Passwords do not match", "The passwords provided do not match.");
            notification_values.TryAdd("Un-validated account", "The account is not validated. Check your email and insert your account validation code.");
            notification_values.TryAdd("Account validation successful", "Account validation is successful, please log in into your account.");
            notification_values.TryAdd("Invalid password", "The password provided isn't valid.");
            notification_values.TryAdd("Invalid account validation code", "This account validation code is not valid. Please enter a valid account validation code.");
            notification_values.TryAdd("Invalid password length", "The length of the password must be greater than 6 characters long.");

            return true;
        }



        private void Window_Opened(object obj, EventArgs e)
        {
            Error_Message_TextBlock.Text = Notification_Message;
            Error_Message_Content_TextBlock.Text = Notification_Message_Content;
        }
    }
}
