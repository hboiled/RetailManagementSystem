using Caliburn.Micro;
using RMS_DESKTOP_UI.EventModels;
using RMS_DESKTOP_UI.Helpers;
using RMS_DESKTOP_UI.Library.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS_DESKTOP_UI.ViewModels
{
    public class LoginViewModel : Screen
    {
		// default dummy creds to make testing faster
		private string _username = "a@a.com";
		private string _password = "Pwd11111";
		private IAPIHelper _apiHelper;
		private IEventAggregator _events;

		public LoginViewModel(IAPIHelper apiHelpeer, IEventAggregator events)
		{
			_apiHelper = apiHelpeer;
			_events = events;
		}

		public string UserName
		{
			get { return _username; }
			set { 
				_username = value;
				// Caliburn micro 2 way binding to login view field name
				// for this prop, it maps to Username
				NotifyOfPropertyChange(() => CanLogIn);
				//CanLogIn(Username, Password);
			}
		}

		public string Password
		{
			get { return _password; }
			set { 
				_password = value;
				NotifyOfPropertyChange(() => CanLogIn);
				;
			}
		}


		public bool IsErrorVisible
		{
			get {
				bool output = false;

				if (ErrorMessage?.Length > 0)
				{
					output = true;
				}

				return output;
			}
		
		}

		private string _errorMessage;

		public string ErrorMessage
		{
			get { return _errorMessage; }
			set {
				_errorMessage = value;				
				NotifyOfPropertyChange(() => ErrorMessage);
				NotifyOfPropertyChange(() => IsErrorVisible);
			}
		}



		public bool CanLogIn
		{
			get
			{
				bool output = false;

				if (UserName?.Length > 0 && Password?.Length > 0)
				{
					output = true;
				}

				return output;
			}
			
		}
		public async Task LogIn()
		{
			try
			{
				ErrorMessage = "";
				var result = await _apiHelper.Authenticate(UserName, Password);

				await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

				_events.PublishOnUIThread(new LogOnEvent());
			}
			catch (Exception e)
			{
				ErrorMessage = e.Message;
			}
		}
	}
}
