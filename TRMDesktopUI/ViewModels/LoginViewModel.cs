using Caliburn.Micro;
using System;
using System.Threading;
using System.Threading.Tasks;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class LoginViewModel : Screen
	{
		private string _userName = "israelmhuss@gmail.com";
		private string _password = "Pwd12345.";
		private readonly IAPIHelper _apiHelper;
		private string _errorMessage;
		private readonly IEventAggregator _events;

		public bool IsErrorVisible => ErrorMessage?.Length > 0;
		public bool CanLogIn => UserName?.Length > 0 && Password?.Length > 5;

		public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
		{
			_apiHelper = apiHelper;
			_events = events;
		}

		public string UserName
		{
			get { return _userName; }
			set
			{
				_userName = value;
				NotifyOfPropertyChange(() => UserName);
				NotifyOfPropertyChange(() => CanLogIn);
			}
		}

		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				NotifyOfPropertyChange(() => Password);
				NotifyOfPropertyChange(() => CanLogIn);
			}
		}

		public string ErrorMessage
		{
			get { return _errorMessage; }
			set
			{
				_errorMessage = value;
				NotifyOfPropertyChange(() => IsErrorVisible);
				NotifyOfPropertyChange(() => ErrorMessage);
			}
		}

		public async Task LogIn()
		{
			try
			{
				AuthenticatedUser result = await _apiHelper.Authenticate(UserName, Password);
				ErrorMessage = "";

				//Capture information about the logged in user
				await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

				await _events.PublishOnUIThreadAsync(new LogOnEvent(), new CancellationToken());
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}
	}
}
