using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SaleController : ControllerBase
	{
		private readonly ISaleData _saleData;

		public SaleController(ISaleData saleData)
		{
			_saleData = saleData;
		}

		[Authorize(Roles = "Cashier")]
		[HttpPost]
		public void Post(SaleModel sale)
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			_saleData.SaveSale(sale, userId);
		}

		[Authorize(Roles = "Admin,Manager")]
		[Route("GetSalesReport")]
		[HttpGet]
		public List<SaleReportModel> GetSalesReport()
		{
			return _saleData.GetSaleReport();
		}
	}
}
