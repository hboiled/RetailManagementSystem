using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMS_DESKTOP_UI.EventModels;
using RMS_DESKTOP_UI.Library.Api;
using RMS_DESKTOP_UI.Library.Models;

namespace RMS_DESKTOP_UI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;

        public ShellViewModel(LoginViewModel loginVM, IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user,
            IAPIHelper apiHelper)
        {
            _events = events;
            _salesVM = salesVM;
            _user = user;
            _apiHelper = apiHelper;
            _events.Subscribe(this);
            
            // IoC inversion of control container can be accessed without the simple container for DI
            ActivateItem(IoC.Get<LoginViewModel>());
        }

        public void UserManagement()
        {
            ActivateItem(IoC.Get<UserDisplayViewModel>());
        }

        public void ExitApplication()
        {
            TryClose();
        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;

                // token is the most reliable field to check for active user
                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    // if user logged in
                    return true;
                }

                return output;
            }
        }

        public void LogOut()
        {
            _user.ResetUserModel();
            _apiHelper.LogOffUser(); // clear token header so that it can't be reused when logged out
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        // handles logon event, message isn't important
        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
