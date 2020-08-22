using Caliburn.Micro;
using RMS_DESKTOP_UI.Library.Api;
using RMS_DESKTOP_UI.Library.Models;
using System;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMS_DESKTOP_UI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
		private StatusInfoViewModel _status;
		private IWindowManager _window;
		private IUserEndpoint _userEndpoint;
		private BindingList<UserModel> _users;

		public BindingList<UserModel> Users
		{
			get
			{
				return _users;
			}
			set
			{
				_users = value;
				NotifyOfPropertyChange(() => Users);
			}
		}

		public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IUserEndpoint userEndpoint)
		{
			_status = status;
			_window = window;
			_userEndpoint = userEndpoint;
		}

		protected override async void OnViewLoaded(object view)
		{
			// when view is loaded, asynchronously load items from api
			base.OnViewLoaded(view);
			try
			{
				await LoadUsers();
			}
			catch (Exception ex)
			{
				dynamic settings = new ExpandoObject();

				settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				settings.ResizeMode = ResizeMode.NoResize;
				settings.Title = "System Error";

				if (ex.Message == "Unauthorized")
				{
					_status.UpdateMessage("Unauthorized Access", "You do not have permission to use this feature");
					_window.ShowDialog(_status, null, settings);
				}
				else
				{
					_status.UpdateMessage("Fatal Error", ex.Message);
					_window.ShowDialog(_status, null, settings);
				}

				TryClose();

				// var info = IoC.Get<StatusInfoViewModel>()
				// get a new instance of the error msg without overriding the one from ctor
			}
		}

		private async Task LoadUsers()
		{
			var userList = await _userEndpoint.GetAll();
			Users = new BindingList<UserModel>(userList);
		}
	}
}
