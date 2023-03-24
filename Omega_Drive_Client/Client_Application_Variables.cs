using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    class Client_Application_Variables
    {
        private static string file_name = "app_settings.json";

        private static Application_Settings application_Settings = new Application_Settings();


        protected static System.Net.IPAddress ip_address = System.Net.IPAddress.Parse("127.0.0.1");

        protected static int port_number = 1024;

        protected static List<string> list_of_available_protocols = new List<string>() { "Tls 1.3", "Tls 1.2", "Tls 1.1", "Tls", "Ssl V3", "Ssl V2" };

        protected static int current_protocol = 0; 

        protected static System.Security.Authentication.SslProtocols ssl_protocol = System.Security.Authentication.SslProtocols.Tls13;





        internal static Task<string> Set_Current_Protocol()
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

            return Task.FromResult(current_selected_protocol);
        }



        protected static async Task<bool> Create_Application_Settings_File()
        {
            if(System.IO.File.Exists(file_name) == false)
            {
                application_Settings.IP_ADDRESS = ip_address.ToString();
                application_Settings.PORT_NUMBER = port_number.ToString();
                application_Settings.PROTOCOL_INDEX = current_protocol.ToString();

                await Set_Current_Protocol();

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

        protected static async Task<bool> Read_Application_Settings_File()
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

                    await Set_Current_Protocol();
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

        protected static async Task<bool> Update_Application_Settings_File()
        {
            System.IO.File.Delete(file_name);
            await Create_Application_Settings_File();
            return true;
        }
    }
}
