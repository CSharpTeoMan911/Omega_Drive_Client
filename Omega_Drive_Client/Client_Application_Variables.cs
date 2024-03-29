﻿using Avalonia.Controls;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    class Client_Application_Variables
    {
        private static Notification_Messages_Processing Notification_Messages_Processing_Object = new Notification_Messages_Processing();

        private static string log_in_session_key;

        private static string file_name = "app_settings.json";

        private static string user_cache_file_name = "user_cache.json";




        protected static Payload_Serialization payload_serialization = new Payload_Serialization();

        private static Application_Settings application_Settings = new Application_Settings();



        protected static System.Net.IPAddress ip_address = System.Net.IPAddress.Parse("127.0.0.1");

        protected static int port_number = 1024;

        protected static int current_protocol = 0;

        private static bool keep_user_logged_in = false;

        private static bool allow_self_signed_certificates = false;

        protected static System.Security.Authentication.SslProtocols ssl_protocol = System.Security.Authentication.SslProtocols.Tls13;




        private delegate void Open_Log_In_Pannel();

        private delegate MainWindow Get_Current_MainWidow_Instance();

        private static Get_Current_MainWidow_Instance get_current_mainwindow_instace;

        private delegate void Load_User_Files_Delegate(string log_in_session_key, byte[] file_id);

        private delegate void Download_User_Files_Delegate(string log_in_session_key, string file_name, byte[] file_id);

        internal delegate Task<string> Load_User_Log_In_Session_Key_Delegate();

        internal static Load_User_Log_In_Session_Key_Delegate Load_User_Log_In_Session_Key_Delegate_Invoker = new Load_User_Log_In_Session_Key_Delegate(Load_User_Log_In_Session_Key);



        private static Dictionary<long, bool> file_structure_info = new Dictionary<long, bool>();



        public enum SslProtocols
        {
            Tls13 = 0,
            Tls12 = 1
        }




        private sealed class Notification_Messages_Processing : INotification_Messages
        {
            public async void Log_In_Result_Processing(string result, object obj)
            {
                if (result != INotification_Messages.login_successful)
                {
                    if (result == INotification_Messages.account_not_validated)
                    {
                        Password_Window password_Window = new Password_Window(Password_Window.Password_Function_Selection.UnValidatedAccount, null);
                        await password_Window.ShowDialog((Log_In__Or__Register)obj);
                    }
                    else
                    {
                        Notification_Window notification_Window = new Notification_Window(result);
                        await notification_Window.ShowDialog((Log_In__Or__Register)obj);
                    }
                }
                else
                {
                    Password_Window password_Window = new Password_Window(Password_Window.Password_Function_Selection.LogIn, null, (Log_In__Or__Register)obj);
                    await password_Window.ShowDialog((Log_In__Or__Register)obj);
                }
            }




            public async void Register_Result_Processing(string result, object obj)
            {
                Notification_Window notification_Window = new Notification_Window(result);
                await notification_Window.ShowDialog((Log_In__Or__Register)obj);

                if (result == INotification_Messages.account_registration_successful)
                {
                    ((Log_In__Or__Register)obj).Register_Email_TextBox.Text = String.Empty;
                    ((Log_In__Or__Register)obj).Register_Password_TextBox.Text = String.Empty;
                    ((Log_In__Or__Register)obj).Register_Repeat_Password_TextBox.Text = String.Empty;


                    Open_Log_In_Pannel open_log_in_panel = new Open_Log_In_Pannel(((Log_In__Or__Register)obj).Open_Log_In_Panel_Mitigator);
                    open_log_in_panel.Invoke();
                }
            }



            public async void SslCertificate_Result_Processing(string result, object obj)
            {
                Notification_Window notification_Window = new Notification_Window(result);
                await notification_Window.ShowDialog((Settings)obj);
            }



            public async void Log_In_Code_Result_Processing(string result, object obj)
            {
                if (result == INotification_Messages.invalid_log_in_code || result == INotification_Messages.connection_failed_message)
                {
                    Notification_Window notification_Window = new Notification_Window(result);
                    await notification_Window.ShowDialog((Password_Window)obj);
                }
                else
                {
                    ((Password_Window)obj).Close();
                }
            }



            public async void Account_Validation_Result_Processing(string result, object obj)
            {
                Notification_Window notification_Window = new Notification_Window(result);
                await notification_Window.ShowDialog((Password_Window)obj);

                if (result == INotification_Messages.account_validation_successful)
                {
                    ((Password_Window)obj).Close();
                }
            }



            public async void Password_Window_Account_Authentification_Result_Processing(string result, object obj)
            {
                Tuple<object, object> Object = (Tuple<object, object>)obj;


                if (result == INotification_Messages.invalid_log_in_code || result == INotification_Messages.connection_failed_message)
                {
                    Notification_Window notification_Window = new Notification_Window(result);
                    await notification_Window.ShowDialog((Password_Window)Object.Item1);
                }
                else
                {
                    log_in_session_key = result;

                    if (keep_user_logged_in == true)
                    {
                        User_Log_In_Key user_Log_In_Key = new User_Log_In_Key();
                        user_Log_In_Key.log_in_key = Encoding.UTF8.GetBytes(result);

                        await Create_User_Log_In_Session_Key_File(user_Log_In_Key);
                    }

                    MainWindow main = new MainWindow();
                    main.Show();

                    ((Password_Window)Object.Item1).Close();
                    ((Log_In__Or__Register)Object.Item2).Close();
                }
            }




            public async void Log_In_Or_Register_Window_Account_Authentification_Result_Processing(string result, object obj)
            {

                if (result == INotification_Messages.invalid_log_in_code || result == INotification_Messages.connection_failed_message)
                {
                    Notification_Window notification_Window = new Notification_Window(result);
                    await notification_Window.ShowDialog((Log_In__Or__Register)obj);
                }
                else
                {
                    MainWindow main = new MainWindow();
                    main.Show();

                    ((Password_Window)obj).Close();
                }
            }



            public void Log_In_Session_Key_Verification_Result_Processing(string result, object obj)
            {
                Tuple<string, object> Object = (Tuple<string, object>)obj;

                if (result == INotification_Messages.log_in_session_key_is_valid)
                {
                    log_in_session_key = Object.Item1;

                    MainWindow main = new MainWindow();
                    main.Show();

                    ((Log_In__Or__Register)Object.Item2).Close();
                }
            }



            public async void Log_Out_Result_Processing(string result, object obj)
            {
                if (result == INotification_Messages.connection_failed_message)
                {
                    Notification_Window notification_Window = new Notification_Window(result);
                    await notification_Window.ShowDialog((MainWindow)obj);
                }

                if (System.IO.File.Exists(user_cache_file_name) == true)
                {
                    System.IO.File.Delete(user_cache_file_name);
                }

                Log_In__Or__Register log_In__Or__Register = new Log_In__Or__Register();
                log_In__Or__Register.Show();

                ((MainWindow)obj).Close();
            }


            public async void Retrieve_User_Files_Information_Result_Processing(string result, object obj)
            {

                if (result != INotification_Messages.connection_failed_message && result != INotification_Messages.log_in_session_key_invalid)
                {

                    try
                    {
                        User_Files_Info user_Files_Info = Newtonsoft.Json.JsonConvert.DeserializeObject<User_Files_Info>(result);




                        Thickness forty_left_thickness = new Thickness(40, 0, 0, 0);
                        Thickness thirrty_left_thickness = new Thickness(40, 0, 0, 0);
                        Thickness ten_left_thickness = new Thickness(10, 0, 0, 0);
                        Thickness ten_up_thickness = new Thickness(0, 10, 0, 0);


                        file_structure_info.Clear();

                        ((MainWindow)obj).User_Files_StackPanel.BeginInit();

                        ((MainWindow)obj).User_Files_StackPanel.Children.Clear();



                        for (int index = 0; index < user_Files_Info.FILE_IDS.Length; index++)
                        {

                            Image delete_bin_image = new Image();
                            delete_bin_image.Classes = Classes.Parse("Delete_Bin_Style");
                            delete_bin_image.Width = 30;


                            Image download_arrow_image = new Image();
                            download_arrow_image.Classes = Classes.Parse("Download_Arrow_Style");
                            download_arrow_image.Width = 30;


                            StackPanel element = new StackPanel();
                            element.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                            element.Orientation = Avalonia.Layout.Orientation.Horizontal;
                            element.Background = Brushes.Black;
                            element.Margin = ten_up_thickness;
                            element.Width = ((MainWindow)obj).Download_Stackpanel.Width;





                            element.BeginInit();

                            TextBox filename_textbox = new TextBox();
                            filename_textbox.BorderBrush = Brushes.Black;
                            filename_textbox.Classes = Classes.Parse("Inivisible_Border");
                            filename_textbox.Width = 200;
                            filename_textbox.Text = Encoding.UTF8.GetString(user_Files_Info.FILE_NAMES[index]);
                            filename_textbox.IsReadOnly = true;
                            filename_textbox.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                            filename_textbox.Background = Brushes.Black;
                            filename_textbox.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#3490D1");
                            filename_textbox.Name = user_Files_Info.FILE_IDS[index].ToString() + "_file_name";

                            element.Children.Add(filename_textbox);




                            TextBlock file_size_label_textblock = new TextBlock();
                            file_size_label_textblock.Classes = Classes.Parse("Transparent_Blue_Foreground");
                            file_size_label_textblock.Margin = forty_left_thickness;
                            file_size_label_textblock.Text = "Size:";
                            file_size_label_textblock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

                            element.Children.Add(file_size_label_textblock);




                            TextBlock file_size_textblock = new TextBlock();
                            file_size_textblock.Classes = Classes.Parse("Transparent_Blue_Foreground");
                            file_size_textblock.Margin = ten_left_thickness;
                            file_size_textblock.Text = user_Files_Info.FILE_SIZES[index].ToString() + " MB";
                            file_size_textblock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

                            element.Children.Add(file_size_textblock);




                            TextBlock date_uploaded_label_textblock = new TextBlock();
                            date_uploaded_label_textblock.Classes = Classes.Parse("Transparent_Blue_Foreground");
                            date_uploaded_label_textblock.Margin = forty_left_thickness;
                            date_uploaded_label_textblock.Text = "Date uploaded:";
                            date_uploaded_label_textblock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

                            element.Children.Add(date_uploaded_label_textblock);




                            TextBlock date_uploaded_textblock = new TextBlock();
                            date_uploaded_textblock.Classes = Classes.Parse("Transparent_Blue_Foreground");
                            date_uploaded_textblock.Margin = ten_left_thickness;
                            date_uploaded_textblock.Text = Encoding.UTF8.GetString(user_Files_Info.FILE_UPLOAD_DATES[index]);
                            date_uploaded_textblock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

                            element.Children.Add(date_uploaded_textblock);




                            Button file_download_button = new Button();
                            file_download_button.Margin = thirrty_left_thickness;
                            file_download_button.Classes = Classes.Parse("Transparent_Thin_Border");
                            file_download_button.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                            file_download_button.Content = download_arrow_image;
                            file_download_button.Name = user_Files_Info.FILE_IDS[index].ToString() + "_download";
                            file_download_button.Click += File_download_button_Click;

                            lock(file_structure_info)
                            {
                                file_structure_info.Add(user_Files_Info.FILE_IDS[index], user_Files_Info.IS_DIRECTORY[index]);
                            }
                            
                            element.Children.Add(file_download_button);




                            Button file_delete_button = new Button();
                            file_delete_button.Margin = ten_left_thickness;
                            file_delete_button.Classes = Classes.Parse("Transparent_Thin_Border");
                            file_delete_button.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                            file_delete_button.Content = delete_bin_image;
                            file_delete_button.Name = user_Files_Info.FILE_IDS[index].ToString() + "_delete";
                            file_delete_button.Click += File_delete_button_Click;

                            element.Children.Add(file_delete_button);

                            element.EndInit();


                            ((MainWindow)obj).User_Files_StackPanel.Children.Add(element);

                        }

                        ((MainWindow)obj).User_Files_StackPanel.EndInit();

                        get_current_mainwindow_instace = new Get_Current_MainWidow_Instance(((MainWindow)obj).Return_Current_Instace);

                    }
                    catch
                    {

                    }
                }
                else
                {
                    Notification_Window notification_Window = new Notification_Window(result);
                    await notification_Window.ShowDialog(((MainWindow)obj));
                }
            }

            public async void User_File_Download_Result_Porcessing(string result, object obj)
            {
                Tuple<string, object> Object = (Tuple<string, object>)obj;


                if (Encoding.UTF8.GetString(Convert.FromBase64String(result)) != INotification_Messages.connection_failed_message)
                {
                    OpenFolderDialog save_the_file = new OpenFolderDialog();

                    string save_file_path = await save_the_file.ShowAsync((MainWindow)Object.Item2);

                    if (save_file_path != null)
                    {
                        StringBuilder file_path_builder = new StringBuilder();

                        file_path_builder.Append(save_file_path);

                        if (System.OperatingSystem.IsWindows() == true)
                        {
                            file_path_builder.Append("\\");
                        }
                        else
                        {
                            file_path_builder.Append("/");
                        }

                        file_path_builder.Append(Object.Item1);

                        System.IO.FileStream fs = System.IO.File.Create(file_path_builder.ToString());

                        try
                        {
                            await fs.WriteAsync(Convert.FromBase64String(result));

                            await fs.FlushAsync();
                        }
                        catch
                        {

                        }
                        finally
                        {
                            if (fs != null)
                            {
                                fs.Close();
                                await fs.DisposeAsync();
                            }
                        }
                    }
                }
                else
                {
                    Notification_Window notification_Window = new Notification_Window(Encoding.UTF8.GetString(Convert.FromBase64String(result)));
                    await notification_Window.ShowDialog((MainWindow)Object.Item2);
                }
            }

            public async void User_File_Upload_Result_Porcessing(string result, object obj)
            {

            }

            private async void File_delete_button_Click(object? sender, RoutedEventArgs e)
            {
                StringBuilder file_id_builder = new StringBuilder(((Button)sender).Name);
                file_id_builder.Remove(file_id_builder.Length - "_delete".Length, "_delete".Length);

                Load_User_Files_Delegate load_User_Files_Delegate = new Load_User_Files_Delegate(get_current_mainwindow_instace.Invoke().Delete_User_Files_Implementor);
                load_User_Files_Delegate.Invoke(log_in_session_key, Encoding.UTF8.GetBytes(file_id_builder.ToString()));
            }

            private void File_download_button_Click(object? sender, RoutedEventArgs e)
            {
                StringBuilder file_id_builder = new StringBuilder(((Button)sender).Name);
                file_id_builder.Remove(file_id_builder.Length - "_download".Length, "_download".Length);


                bool is_directory = false;
                file_structure_info.TryGetValue(Convert.ToInt64(file_id_builder.ToString()),out is_directory);


                List<IControl> stackpanel_elements = get_current_mainwindow_instace.Invoke().User_Files_StackPanel.Children.ToList();

                for (int index = 0; index < stackpanel_elements.Count; index++)
                {
                    List<IControl> current_element = ((StackPanel)stackpanel_elements[index]).Children.ToList();

                    StringBuilder file_name_builder = new StringBuilder(((TextBox)current_element[0]).Name);
                    file_name_builder.Remove(file_name_builder.Length - "_file_name".Length, "_file_name".Length);


                    if (file_name_builder.ToString() == file_id_builder.ToString())
                    {
                        if (is_directory == true)
                        {
                           
                        }
                        else
                        {
                            Download_User_Files_Delegate download_User_Files_Delegate = new Download_User_Files_Delegate(get_current_mainwindow_instace.Invoke().Download_User_Files_Implementor);
                            download_User_Files_Delegate.Invoke(log_in_session_key, ((TextBox)current_element[0]).Text, Encoding.UTF8.GetBytes(file_id_builder.ToString()));
                            break;
                        }
                    }
                }



            }
        }









        internal enum Settings_File_Option
        {
            Read_Settings_File,
            Create_Settings_File,
            Update_Settings_File
        }

        public enum Selected_Function
        {
            Log_In,
            Log_Out,
            Register,
            Validate_Account,
            Authentificate_Account,
            LoadSslCertificate,
            Log_in_Session_Key_Verification,
            User_Files_Information_Retrieval,
            User_File_Download,
            User_File_Upload
        }




        internal static bool Get_If_Self_Signed_Certificates_Are_Allowed()
        {
            return allow_self_signed_certificates;
        }

        internal static void Set_If_Self_Signed_Certificates_Are_Allowed(bool are_self_signed_certificates_allowed)
        {
            allow_self_signed_certificates = are_self_signed_certificates_allowed;
        }

        internal static void Set_Port_Number(int port)
        {
            port_number = port;
        }

        internal static int Get_Port_Number()
        {
            return port_number;
        }

        internal static System.Net.IPAddress Get_IP_Address()
        {
            return ip_address;
        }

        internal static async void Set_IP_Address(System.Net.IPAddress ip_addr)
        {
            ip_address = ip_addr;

            await Update_Application_Settings_File();
        }

        internal static void Set_Keep_User_Logged_In(bool is_kept_logged_in)
        {
            keep_user_logged_in = is_kept_logged_in;
        }

        internal static string Get_Log_In_Session_Key()
        {
            return log_in_session_key;
        }









        internal static void Increment_Current_Protocol_Index()
        {
            if (current_protocol < 1)
            {
                current_protocol++;
            }
        }


        internal static void Decrement_Current_Protocol_Index()
        {
            if (current_protocol > 0)
            {
                current_protocol--;
            }
        }






        internal static string Get_And_Set_Current_Protocol()
        {
            SslProtocols current_selected_protocol = (SslProtocols)current_protocol;

            if (current_protocol == (int)SslProtocols.Tls13)
            {
                ssl_protocol = System.Security.Authentication.SslProtocols.Tls13;
            }
            else
            {
                ssl_protocol = System.Security.Authentication.SslProtocols.Tls12;
            }

            return current_selected_protocol.ToString();
        }







        internal static void Function_Result_Processing(Selected_Function option, string result, object obj)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {

                if (option == Selected_Function.Log_In)
                {
                    Notification_Messages_Processing_Object.Log_In_Result_Processing(result, obj);
                }
                else if (option == Selected_Function.Log_Out)
                {
                    Notification_Messages_Processing_Object.Log_Out_Result_Processing(result, obj);
                }
                else if (option == Selected_Function.Authentificate_Account)
                {
                    Notification_Messages_Processing_Object.Password_Window_Account_Authentification_Result_Processing(result, obj);
                }
                else if (option == Selected_Function.Log_in_Session_Key_Verification)
                {
                    Notification_Messages_Processing_Object.Log_In_Session_Key_Verification_Result_Processing(result, obj);
                }
                else if (option == Selected_Function.Register)
                {
                    Notification_Messages_Processing_Object.Register_Result_Processing(result, obj);
                }
                else if(option == Selected_Function.LoadSslCertificate)
                {
                    Notification_Messages_Processing_Object.SslCertificate_Result_Processing(result, obj);
                }
                else if (option == Selected_Function.Validate_Account)
                {
                    Notification_Messages_Processing_Object.Account_Validation_Result_Processing(result, obj);
                }
                else if (option == Selected_Function.User_Files_Information_Retrieval)
                {
                    Notification_Messages_Processing_Object.Retrieve_User_Files_Information_Result_Processing(result, obj);
                }
                else if(option == Selected_Function.User_File_Download)
                {
                    Notification_Messages_Processing_Object.User_File_Download_Result_Porcessing(result, obj);
                }
                else if(option == Selected_Function.User_File_Upload)
                {
                    Notification_Messages_Processing_Object.User_File_Upload_Result_Porcessing(result, obj);
                }

            }, Avalonia.Threading.DispatcherPriority.Background);
        }








        internal static async Task<bool> Settings_File_Operation_Selector(Settings_File_Option option)
        {
            if(option == Settings_File_Option.Read_Settings_File)
            {
                return await Read_Application_Settings_File();
            }
            else if(option == Settings_File_Option.Create_Settings_File)
            {
                return await Create_Application_Settings_File();
            }
            else
            {
                return await Update_Application_Settings_File();
            }
        }







        private static async Task<bool> Create_Application_Settings_File()
        {
            if(System.IO.File.Exists(file_name) == false)
            {
                application_Settings.IP_ADDRESS = ip_address.ToString();
                application_Settings.PORT_NUMBER = port_number.ToString();
                application_Settings.PROTOCOL_INDEX = current_protocol.ToString();

                Get_And_Set_Current_Protocol();

                byte[] json_formated_settings = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(application_Settings, Newtonsoft.Json.Formatting.Indented));
                System.IO.FileStream settings_file_stream = System.IO.File.Create(file_name, json_formated_settings.Length, System.IO.FileOptions.Asynchronous);

                try
                {
                    await settings_file_stream.WriteAsync(json_formated_settings, 0, json_formated_settings.Length);
                    await settings_file_stream.FlushAsync();
                }
                catch
                {
                    if (settings_file_stream != null)
                    {
                        await settings_file_stream.FlushAsync();
                        settings_file_stream.Close();
                    }
                }
                finally
                {
                    if (settings_file_stream != null)
                    {
                        await settings_file_stream.FlushAsync();
                        settings_file_stream.Close();
                        await settings_file_stream.DisposeAsync();
                    }
                }
            }

            return true;
        }





        private static async Task<bool> Read_Application_Settings_File()
        {
            if (System.IO.File.Exists(file_name) == true)
            {
                System.IO.StreamReader settings_stream_reader = new System.IO.StreamReader(file_name);

                try
                {
                    string serialized_settings_file = await settings_stream_reader.ReadToEndAsync();

                    application_Settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Application_Settings>(serialized_settings_file);

                    System.Net.IPAddress address = null;
                    System.Net.IPAddress.TryParse(application_Settings.IP_ADDRESS, out address);

                    ip_address = address;
                    port_number = Convert.ToInt32(application_Settings.PORT_NUMBER);
                    current_protocol = Convert.ToInt32(application_Settings.PROTOCOL_INDEX);

                    Get_And_Set_Current_Protocol();
                }
                catch
                {
                    if (settings_stream_reader != null)
                    {
                        settings_stream_reader.Close();
                    }
                }
                finally
                {
                    if (settings_stream_reader != null)
                    {
                        settings_stream_reader.Close();
                        settings_stream_reader.Dispose();
                    }
                }
            }
            else
            {
                await Create_Application_Settings_File();
            }

            return true;
        }


        private static async Task<string> Load_User_Log_In_Session_Key()
        {
            string log_in_session_key = INotification_Messages.error_loading_user_cache;

            if (System.IO.File.Exists(user_cache_file_name) == true)
            {
                System.IO.FileStream log_in_session_key_file = System.IO.File.Open(user_cache_file_name, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                try
                {
                    byte[] log_in_session_key_bytes = new byte[log_in_session_key_file.Length];

                    await log_in_session_key_file.ReadAsync(log_in_session_key_bytes, 0, log_in_session_key_bytes.Length);

                    User_Log_In_Key deserialized_log_in_session_key = Newtonsoft.Json.JsonConvert.DeserializeObject<User_Log_In_Key>(Encoding.UTF8.GetString(log_in_session_key_bytes));

                    log_in_session_key = Encoding.UTF8.GetString(deserialized_log_in_session_key.log_in_key);
                }
                catch
                {
                    if (log_in_session_key_file != null)
                    {
                        log_in_session_key_file.Close();
                    }
                }
                finally
                {
                    if (log_in_session_key_file != null)
                    {
                        log_in_session_key_file.Close();
                        await log_in_session_key_file.DisposeAsync();
                    }
                }
            }

            return log_in_session_key;
        }



        private static async Task<bool> Create_User_Log_In_Session_Key_File(User_Log_In_Key log_in_session_key_object)
        {
            byte[] serialized_log_in_session_key = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(log_in_session_key_object));


            System.IO.FileStream log_in_session_key_filestream = System.IO.File.Create(user_cache_file_name);

            try
            {
                await log_in_session_key_filestream.WriteAsync(serialized_log_in_session_key, 0, serialized_log_in_session_key.Length);
                await log_in_session_key_filestream.FlushAsync();
            }
            catch
            {
                if (log_in_session_key_filestream != null)
                {
                    log_in_session_key_filestream.Close();
                }
            }
            finally
            {
                if (log_in_session_key_filestream != null)
                {
                    log_in_session_key_filestream.Close();
                    await log_in_session_key_filestream.DisposeAsync();
                }
            }

            return true;
        }


        private static async Task<bool> Update_Application_Settings_File()
        {
            System.IO.File.Delete(file_name);
            await Create_Application_Settings_File();
            return true;
        }
    }
}
