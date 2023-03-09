using Avalonia.Controls;

namespace Omega_Drive_Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {

            });
        }
    }
}
