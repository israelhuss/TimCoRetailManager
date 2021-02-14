using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
	{
		private IEventAggregator _events;
		private SalesViewModel _salesVM;
		private ILoggedInUserModel _user;
		private IAPIHelper _apiHelper;

		public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user, IAPIHelper apiHelper)
		{
			_events = events;
			_salesVM = salesVM;
			_user = user;
			_apiHelper = apiHelper;

			_events.Subscribe(this);

			ActivateItem(IoC.Get<LoginViewModel>());
		}

		public bool IsLoggedIn => string.IsNullOrWhiteSpace(_user.Token) == false;

		public void ExitApplication()
		{
			TryClose();
		}

		public void UserManagement()
		{
			ActivateItem(IoC.Get<UserDisplayViewModel>());
		}

		public void LogOut()
		{
			_user.ResetUserModel();
			_apiHelper.LogOffUser();
			ActivateItem(IoC.Get<LoginViewModel>());
			NotifyOfPropertyChange(() => IsLoggedIn);
		}

		public void Handle(LogOnEvent message)
		{
			ActivateItem(_salesVM);
			NotifyOfPropertyChange(() => IsLoggedIn);
		}
	}
}
