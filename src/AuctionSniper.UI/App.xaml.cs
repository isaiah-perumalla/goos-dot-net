using System;
using System.Threading;
using System.Windows;
using AuctionSniper.UI.ViewModels;

namespace AuctionSniper.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            var mainWindow = new MainWindow() {DataContext = new MainWindowModel(SynchronizationContext.Current)};
            mainWindow.Show();
        }
    }
}
