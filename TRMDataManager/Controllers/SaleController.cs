using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
	[Authorize]
	public class SaleController : ApiController
	{
		public void Post(SaleModel sale)
		{
			SaleData data = new SaleData();
			string userId = RequestContext.Principal.Identity.GetUserId();

			data.SaveSale(sale, userId);
		}

		[Route("GetSalesReport")]
		public List<SaleReportModel> GetSalesReport()
		{
			SaleData data = new SaleData();
			return data.GetSaleReport();
		}
	}
}
