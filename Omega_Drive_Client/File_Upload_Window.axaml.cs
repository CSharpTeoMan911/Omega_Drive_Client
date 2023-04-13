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
        public File_Upload_Window()
        {
            InitializeComponent();
        }


        private async void Upload_File(object sender, RoutedEventArgs e)
        {

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

                    StringBuilder serialized_virtualized_selected_user_directory_stringbuilder = new StringBuilder(System.Text.Json.JsonSerializer.Serialize(virtualized_selected_user_directory));





                    virtualized_selected_user_directory.directory_name = null;
                    virtualized_selected_user_directory.files_list.Clear();
                    virtualized_selected_user_directory.sub_directories.Clear();

                    serialized_virtualized_selected_user_directory_stringbuilder.Clear();

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
