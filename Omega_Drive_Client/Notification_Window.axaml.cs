using Avalonia.Controls;
using System;

namespace Omega_Drive_Client
{
    public partial class Notification_Window : Window
    {
        private string Notification_Message;
        private string Notification_Message_Content;
        private bool IsNotification;

        public Notification_Window(string notification_message)
        {
            Notification_Message = notification_message;
            IsNotification = INotification_Messages.notifications_and_values.TryGetValue(notification_message, out Notification_Message_Content);

            InitializeComponent();
        }



        public Notification_Window()
        {
            InitializeComponent();
        }



        private void Window_Opened(object obj, EventArgs e)
        {
            if(IsNotification == true)
            {
                Error_Message_TextBlock.Text = Notification_Message;
                Error_Message_Content_TextBlock.Text = Notification_Message_Content;
            }
            else
            {
                this.Close();
            }
        }
    }
}
