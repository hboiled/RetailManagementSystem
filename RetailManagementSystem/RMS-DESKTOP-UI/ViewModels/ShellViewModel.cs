using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMS_DESKTOP_UI.EventModels;

namespace RMS_DESKTOP_UI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private SimpleContainer _container;

        public ShellViewModel(LoginViewModel loginVM, IEventAggregator events, SalesViewModel salesVM,
            SimpleContainer container)
        {
            _events = events;
            _salesVM = salesVM;
            _container = container;
            _events.Subscribe(this);
            
            ActivateItem(_container.GetInstance<LoginViewModel>());
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);            
        }
    }
}
