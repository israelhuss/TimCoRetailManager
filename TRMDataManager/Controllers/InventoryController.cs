using System.Collections.Generic;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
	[Authorize]
	public class InventoryController : ApiController
	{
		public List<InventoryModel> Get()
		{
			InventoryData data = new InventoryData();
			return data.GetInventory();
		}

		public void Post(InventoryModel item)
		{
			InventoryData data = new InventoryData();
			data.SaveInventoryRecord(item);
		}
	}
}
