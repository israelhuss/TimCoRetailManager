using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class InventoryController : ControllerBase
	{
		private readonly IConfiguration _config;

		public InventoryController(IConfiguration config)
		{
			_config = config;
		}

		[Authorize(Roles = "Manager,Admin")]
		[HttpGet]
		public List<InventoryModel> Get()
		{
			InventoryData data = new InventoryData(_config);
			return data.GetInventory();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public void Post(InventoryModel item)
		{
			InventoryData data = new InventoryData(_config);
			data.SaveInventoryRecord(item);
		}
	}
}
