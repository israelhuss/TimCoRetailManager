﻿using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;
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

			_events.SubscribeOnPublishedThread(this);

			ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
		}

		public bool IsLoggedIn => string.IsNullOrWhiteSpace(_user.Token) == false;

		public void ExitApplication()
		{
			TryCloseAsync();
		}

		public async Task UserManagement()
		{
			await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(), new CancellationToken());
		}

		public async Task LogOut()
		{
			_user.ResetUserModel();
			_apiHelper.LogOffUser();
			await ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
			NotifyOfPropertyChange(() => IsLoggedIn);
		}

		//public void Handle(LogOnEvent message)
		//{
		//	ActivateItem(_salesVM);
		//	NotifyOfPropertyChange(() => IsLoggedIn);
		//}

		public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
		{
			await ActivateItemAsync(_salesVM, cancellationToken);
			NotifyOfPropertyChange(() => IsLoggedIn);
		}
	}
}
