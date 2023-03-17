using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omega_Drive_Client
{
    public partial class MainWindow : Window
    {
        public static string TEST_RESULT;
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


        private void Window_Initialized(object sender, EventArgs e)
        {
            //TEST.Text = TEST_RESULT;
        }

        private void Window_Opened(object sender, EventArgs e)
        {
            TEST.Text = TEST_RESULT;
        }

    }
}
