using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
	{
		private IEventAggregator _events;
		private SalesViewModel _salesVM;
		private ILoggedInUserModel _user;

		public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user)
		{
			_events = events;
			_salesVM = salesVM;
			_user = user;

			_events.Subscribe(this);

			ActivateItem(IoC.Get<LoginViewModel>());
		}

		public bool IsLoggedIn => string.IsNullOrWhiteSpace(_user.Token) == false;

		public void ExitApplication()
		{
			TryClose();
		}

		public void LogOut()
		{
			_user.LogOffUser();
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
