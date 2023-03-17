using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    public partial class App : Application
    {
        private sealed class Client_Application_Variables_Mitigator : Client_Application_Variables
        {
            internal static async Task<bool> Load_Application_File_Settings_Initiator()
            {
                return await Read_Application_Settings_File();
            }
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            Task.Run(async () =>
            {
                await Client_Application_Variables_Mitigator.Load_Application_File_Settings_Initiator();
            });

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new Log_In__Or__Register();
            }
        }

    }
}
