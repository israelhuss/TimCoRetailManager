using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Cashier")]
	public class ProductController : ControllerBase
	{
		public List<ProductModel> Get()
		{
			ProductData data = new ProductData();

			return data.GetProducts();
		}
	}
}
