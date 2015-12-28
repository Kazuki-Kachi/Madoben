using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Server.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Reactive.Linq;
using Microsoft.AspNet.SignalR.Client;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Server
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; } = new MainPageViewModel();
        IHubProxy hub;
        public MainPage()
        {
            InitializeComponent();

            var connection = new HubConnection("http://serverofwhitenoodlehub.azurewebsites.net/");
            //var connection = new HubConnection("http://localhost:2773/");
            hub = connection.CreateHubProxy("NoodlesServerHub");

            hub.On<string>("info", info => ViewModel.ServedInformation += info);
            connection.Start().Wait();
        }
    }
}
