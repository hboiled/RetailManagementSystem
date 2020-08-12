using Caliburn.Micro;
using RMS_DESKTOP_UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS_DESKTOP_UI.ViewModels
{
    public class LoginViewModel : Screen
    {
		private string _username;
		private string _password;
		private IAPIHelper _apiHelper;

		public LoginViewModel(IAPIHelper apiHelpeer)
		{
			_apiHelper = apiHelpeer;
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
				var result = await _apiHelper.Authenticate(UserName, Password);
			}
			catch (Exception e)
			{
				Console.WriteLine();
			}
		}
	}
}
