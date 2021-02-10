using System;

namespace TRMDesktopUI.Library.Models
{
	public class LoggedInUserModel : ILoggedInUserModel
	{
		public string Token { get; set; }
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public DateTime CreatedDate { get; set; }

		public void LogOffUser()
		{
			Token = string.Empty;
			Id = string.Empty;
			FirstName = string.Empty;
			LastName = string.Empty;
			EmailAddress = string.Empty;
			CreatedDate = DateTime.MinValue;
		}
	}
}
