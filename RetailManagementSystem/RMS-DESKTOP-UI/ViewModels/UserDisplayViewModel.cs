using Caliburn.Micro;
using RMS_DESKTOP_UI.Library.Api;
using RMS_DESKTOP_UI.Library.Models;
using System;
using System.Collections.Generic;
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

		private UserModel _selectedUser;

		public UserModel SelectedUser
		{
			get { return _selectedUser; }
			set { 
				_selectedUser = value;
				SelectedUserName = value.Email;
				//SelectedUserRoles?.Clear();
				UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
				LoadRoles();
				NotifyOfPropertyChange(() => SelectedUser);
			}
		}

		private string _selectedUserName;

		public string SelectedUserName
		{
			get { return _selectedUserName; }
			set {
				_selectedUserName = value;
				NotifyOfPropertyChange(() => SelectedUserName);
			}
		}

		private BindingList<string> _userRoles = new BindingList<string>();

		public BindingList<string> UserRoles
		{
			get { return _userRoles; }
			set {
				_userRoles = value;
				NotifyOfPropertyChange(() => UserRoles);
			}
		}

		private BindingList<string> _availableRoles = new BindingList<string>();

		public BindingList<string> AvailableRoles
		{
			get { return _availableRoles; }
			set
			{
				_availableRoles = value;
				NotifyOfPropertyChange(() => AvailableRoles);
			}
		}

		private string _selectedUserRole;

		public string SelectedUserRole
		{
			get { return _selectedUserRole; }
			set {
				_selectedUserRole = value;
				NotifyOfPropertyChange(() => SelectedUserRole);
			}
		}

		private string _selectedAvailableRole;

		public string SelectedAvailableRole
		{
			get { return _selectedAvailableRole; }
			set
			{
				_selectedAvailableRole = value;
				NotifyOfPropertyChange(() => SelectedAvailableRole);
			}
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
					await _window.ShowDialogAsync(_status, null, settings);
				}
				else
				{
					_status.UpdateMessage("Fatal Error", ex.Message);
					await _window.ShowDialogAsync(_status, null, settings);
				}

				TryCloseAsync();

				// var info = IoC.Get<StatusInfoViewModel>()
				// get a new instance of the error msg without overriding the one from ctor
			}
		}

		private async Task LoadUsers()
		{
			var userList = await _userEndpoint.GetAll();
			Users = new BindingList<UserModel>(userList);
		}

		private async Task LoadRoles()
		{
			AvailableRoles.Clear();
			var roles = await _userEndpoint.GetAllRoles();

			foreach (var role in roles)
			{
				// if cannot find current role
				if (UserRoles.IndexOf(role.Value) < 0)
				{
					// populate available roles with roles user does not have
					AvailableRoles.Add(role.Value);
				}
			}
		}
		
		public async Task AddSelectedRole()
		{
			await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);

			UserRoles.Add(SelectedAvailableRole);
			AvailableRoles.Remove(SelectedAvailableRole);
		}

		public async Task RemoveSelectedRole()
		{
			await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);

			UserRoles.Remove(SelectedUserRole);
			AvailableRoles.Add(SelectedUserRole);
		}
	}
}
