using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class UserDisplayViewModel : Screen
	{
		private readonly StatusInfoViewModel _status;
		private readonly IWindowManager _window;
		private readonly IUserEndpoint _userEndpoint;
		private BindingList<UserModel> _users;
		private UserModel _selectedUser;
		private BindingList<string> _userRoles = new BindingList<string>();
		private BindingList<string> _availableRoles = new BindingList<string>();
		private string _selectedUserRole;
		private string _selectedAvailableRole;


		public BindingList<UserModel> Users
		{
			get { return _users; }
			set
			{
				_users = value;
				NotifyOfPropertyChange(() => Users);
			}
		}

		public UserModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
				LoadRoles();
				NotifyOfPropertyChange(() => SelectedUser);
			}
		}

		public string SelectedUserRole
		{
			get { return _selectedUserRole; }
			set
			{
				_selectedUserRole = value;
				NotifyOfPropertyChange(() => SelectedUserRole);
			}
		}

		public string SelectedAvailableRole
		{
			get { return _selectedAvailableRole; }
			set
			{
				_selectedAvailableRole = value;
				NotifyOfPropertyChange(() => _selectedAvailableRole);
			}
		}

		public BindingList<string> UserRoles
		{
			get { return _userRoles; }
			set
			{
				_userRoles = value;
				NotifyOfPropertyChange(() => UserRoles);
			}
		}


		public BindingList<string> AvailableRoles
		{
			get { return _availableRoles; }
			set
			{
				_availableRoles = value;
				NotifyOfPropertyChange(() => AvailableRoles);
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
					_status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form.");
					_window.ShowDialog(_status, null, settings);
				}
				else
				{
					_status.UpdateMessage("Fatal Exception", ex.Message);
					_window.ShowDialog(_status, null, settings);
				}

				TryClose();
			}
		}

		private async Task LoadUsers()
		{
			var userList = await _userEndpoint.GetAll();
			Users = new BindingList<UserModel>(userList);
		}

		private async Task LoadRoles()
		{
			var roles = await _userEndpoint.GetAllRoles();

			foreach (var role in roles)
			{
				if (UserRoles.IndexOf(role.Value) < 0)
				{
					AvailableRoles.Add(role.Value);
				}
			}
		}

		public async void AddSelectedRole()
		{
			await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);

			UserRoles.Add(SelectedAvailableRole);
			AvailableRoles.Remove(SelectedAvailableRole);
		}

		public async void RemoveSelectedRole()
		{
			await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);

			AvailableRoles.Add(SelectedUserRole);
			UserRoles.Remove(SelectedUserRole);
		}
	}
}
