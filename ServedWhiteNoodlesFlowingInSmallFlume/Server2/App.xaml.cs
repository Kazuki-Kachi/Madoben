using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;

namespace Server2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var connection = new HubConnection("http://localhost:2773/");
            var hub = connection.CreateHubProxy("NoodlesServerHub");
            await connection.Start();
            var main = new Server.MainWindow();
            var con = main.DataContext as Server.ViewModel.MainPageViewModel;
            if(con == null) return;
            await con.SetHub(hub);
            main.ShowDialog();
        }
    }
}
