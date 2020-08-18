using Caliburn.Micro;
using RMS_DESKTOP_UI.Helpers;
using RMS_DESKTOP_UI.Library.Api;
using RMS_DESKTOP_UI.Library.Helpers;
using RMS_DESKTOP_UI.Library.Models;
using RMS_DESKTOP_UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RMS_DESKTOP_UI
{
    public class Bootstrapper: BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();
        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
                PasswordBoxHelper.BoundPasswordProperty,
                "Password",
                "PasswordChanged");

        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // Inits base view model with corresponding view, 
            // whose ctor must be parameterless (the one with initialize method call)
            DisplayRootViewFor<ShellViewModel>();
        }

        // DI container
        // Sets an instance of itself to itself, passes out this reference to requesting components
        protected override void Configure()
        {
            _container.Instance(_container)
                .PerRequest<IProductEndpoint, ProductEndpoint>();

            // Get from caliburn micro, a singleton of WindowManager and EventAggregator
            _container
                // specify interface, provide implementation
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>()
                .Singleton<IConfigHelper, ConfigHelper>()
                .Singleton<IAPIHelper, APIHelper>();

            // reflection on current application instance
            GetType().Assembly.GetTypes()
                // get types that are classes and end with view model
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                // 
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        // Caliburn micro set up DI for an instance
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        // Caliburn micro set up DI for instances
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
