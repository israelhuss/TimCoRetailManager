using System;
using System.Net.Http;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api
{
	public class SaleEndpoint : ISaleEndpoint
	{
		private IAPIHelper _apiHelper;

		public SaleEndpoint(IAPIHelper apiHelper)
		{
			_apiHelper = apiHelper;
		}

		public async Task PostSale(SaleModel sale)
		{
			using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/sale", sale))
			{
				if (response.IsSuccessStatusCode)
				{
					//Log successful call??
				}
				else
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}
	}
}
