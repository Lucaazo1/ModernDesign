using Microsoft.Azure.Amqp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModernDesign.Helpers;
using ModernDesign.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ModernDesign
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? app { get; private set; }

        public App()
        {
            app = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                //Singleton = en endaste instans av någonting i hela programmet. Tills vi startar om Server/Webservern
                //Scoped = en ny instans för varje gång vi går in på en kontroller men våra partial views får samma information 
                services.AddSingleton<MainWindow>();

                services.AddSingleton<NavigationStore>();

            }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {

            var navigationStore = app!.Services.GetRequiredService<NavigationStore>();
            navigationStore.CurrentViewModel = new KitchenViewModel(navigationStore);

            await app!.StartAsync();
            var MainWindow = app.Services.GetRequiredService<MainWindow>();
            MainWindow.DataContext = new MainViewModel(navigationStore);
            MainWindow.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await app!.StopAsync();
            base.OnExit(e);
        }

    }
}
