using Caliburn.Micro;
using RMS_DESKTOP_UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMS_DESKTOP_UI
{
    public class Bootstrapper: BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        // Sets an instance of itself to itself, passes out this reference to requesting components
        protected override void Configure()
        {
            _container.Instance(_container);

            // Get from caliburn micro, a singleton of WindowManager and EventAggregator
            _container
                // specify interface, provide implementation
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();

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
