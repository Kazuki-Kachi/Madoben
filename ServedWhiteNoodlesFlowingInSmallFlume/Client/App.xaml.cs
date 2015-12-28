using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Client.ViewModel;
using Microsoft.AspNet.SignalR.Client;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var frm = new MainWindow();
            frm.ShowDialog();
            //var connection = new HubConnection("http://serverofwhitenoodlehub.azurewebsites.net/");
            var connection = new HubConnection("http://localhost:2773/");
            var hub = connection.CreateHubProxy("NoodlesServerHub");
            await connection.Start();

            var main = new Guest();
            var bcon = frm.DataContext as MainPageViewModel;
            var con = main.DataContext as GuestViewModel;
            if(bcon == null || con == null) Current.Shutdown();
            /*hub["UserName"] =*/ con.Guest.Name = bcon.Name;
            con.Guest.Allergy = bcon.AllergyType;
            
            con.SetHub(hub);

            main.ShowDialog();

            Current.Shutdown();

        }
    }
}
