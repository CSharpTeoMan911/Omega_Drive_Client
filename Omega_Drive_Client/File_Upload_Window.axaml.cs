using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    public partial class File_Upload_Window : Window
    {
        private static Newtonsoft.Json.Serialization.DefaultContractResolver defaultContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
        Server_Connections Server_Connections = new Server_Connections();


        public File_Upload_Window()
        {
            InitializeComponent();
        }

        private async void Upload_File(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file_selector = new OpenFileDialog();

            string file_path = (await file_selector.ShowAsync(this))[0];


            if (file_path != null)
            {
                System.IO.FileStream selected_file_stream = System.IO.File.OpenRead(file_path);
                try
                {
                    byte[] selected_file = new byte[selected_file_stream.Length];

                    await selected_file_stream.ReadAsync(selected_file, 0, selected_file.Length);

                    await selected_file_stream.FlushAsync();

                    User_File user_file = new User_File();

                    user_file.File_Name = System.IO.Path.GetFileName(file_path);
                    user_file.File_Size = selected_file.Length / 1000000;
                    user_file.File_Binaries = selected_file;

                    byte[] serialized_selected_file = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(user_file));

                    string result = Encoding.UTF8.GetString(await Server_Connections.Secure_Server_Connections("Upload user file", Client_Application_Variables.Get_Log_In_Session_Key(), serialized_selected_file));

                    System.Diagnostics.Debug.WriteLine(result);

                    Client_Application_Variables.Function_Result_Processing(Client_Application_Variables.Selected_Function.User_File_Upload, result, this);

                    // IMPLEMENT Function_Result_Processing
                }
                catch
                {

                }
                finally
                {
                    selected_file_stream.Close();
                    await selected_file_stream.DisposeAsync();
                }
            }

        }

        private async void Upload_Folder(object sender, RoutedEventArgs e)
        {

            OpenFolderDialog root_directory_selector = new OpenFolderDialog();

            string root_directory_path = await root_directory_selector.ShowAsync(this);



            if(root_directory_path != null)
            {
                System.Threading.Thread parallel_processing = new System.Threading.Thread(async() =>
                {

                    User_Directory virtualized_selected_user_directory = await Directory_Dissasembly_Operation(root_directory_path);


                    if(System.IO.File.Exists("User_Selected_Serialized_Directory.json") == true)
                    {
                        System.IO.File.Delete("User_Selected_Serialized_Directory.json");
                    }


                    System.IO.TextWriter payload_stream_writer = new System.IO.StreamWriter("User_Selected_Serialized_Directory.json");

                    try
                    {
                        Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer();
                        js.Formatting = Newtonsoft.Json.Formatting.Indented;
                        js.Serialize(payload_stream_writer, virtualized_selected_user_directory, typeof(User_Directory));
                        js.ContractResolver = defaultContractResolver;

                        await payload_stream_writer.FlushAsync();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (payload_stream_writer != null)
                        {
                            payload_stream_writer.Close();
                            await payload_stream_writer.DisposeAsync();
                        }
                    }


                    virtualized_selected_user_directory.directory_name = null;
                    virtualized_selected_user_directory.files_list.Clear();
                    virtualized_selected_user_directory.sub_directories.Clear();

                    GC.Collect(4, GCCollectionMode.Forced);
                });
                parallel_processing.SetApartmentState(System.Threading.ApartmentState.MTA);
                parallel_processing.Priority = System.Threading.ThreadPriority.Highest;
                parallel_processing.IsBackground = false;
                parallel_processing.Start();
            }
        }




        private async Task<User_Directory> Directory_Dissasembly_Operation(string root_directory_path)
        {

            User_Directory current_directory = new User_Directory();

            current_directory.directory_name = System.IO.Path.GetFileName(root_directory_path);





            StringBuilder string_builder = new StringBuilder(root_directory_path);



            IEnumerator files_enumerator = System.IO.Directory.GetFiles(string_builder.ToString()).GetEnumerator();

            while(files_enumerator.MoveNext() == true)
            {
                string_builder.Clear();
                string_builder.Append((string)files_enumerator.Current);

                System.Threading.Thread.Sleep(10);

                Tuple<string, byte[]> current_item = new Tuple<string, byte[]>(System.IO.Path.GetFileName(string_builder.ToString()), await System.IO.File.ReadAllBytesAsync(string_builder.ToString()));
                current_directory.files_list.Add(current_item);
            }

            files_enumerator.Reset();





            IEnumerator directories_enumerator = System.IO.Directory.GetDirectories(root_directory_path).GetEnumerator();

            while(directories_enumerator.MoveNext() == true)
            {
                string_builder.Clear();
                string_builder.Append((string)directories_enumerator.Current);

                System.Threading.Thread.Sleep(10);

                current_directory.sub_directories.Add(await Directory_Dissasembly_Operation(string_builder.ToString()));
            }

            directories_enumerator.Reset();



            string_builder.Clear();

            return current_directory;
        }
    }
}
