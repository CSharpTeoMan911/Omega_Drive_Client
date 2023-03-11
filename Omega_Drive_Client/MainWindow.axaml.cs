using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Media;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omega_Drive_Client
{
    public partial class MainWindow : Window
    {

        private static List<Files> database_files = new List<Files>();


        public class Files
        {
            public string FileName { get; set; }

            public string Size { get; set; }

            public string Date { get; set; }


            public CheckBox Check { get; set; }
        }



        public MainWindow()
        {
            InitializeComponent();
        }


    }
}
