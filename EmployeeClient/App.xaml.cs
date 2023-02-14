using Application.Repositories;
using Infrastructure;
using System;
using System.Windows;
using Unity;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private IUnityContainer? _container;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _container = new UnityContainer();
            _container.RegisterType<IEmployeeRepository, EmployeeRepository>();
            _container.RegisterType<ITokenProvider,TokenProvider>();
            var mainWindow = _container.Resolve<MainWindow>();
            mainWindow.Show();
        }
    }
    
}
