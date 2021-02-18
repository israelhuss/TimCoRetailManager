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
		private readonly IProductData _productData;

		public ProductController(IProductData productData)
		{
			_productData = productData;
		}

		[HttpGet]
		public List<ProductModel> Get()
		{
			return _productData.GetProducts();
		}
	}
}
