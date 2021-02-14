using System.Collections.Generic;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
	public class InventoryData
	{
		public List<InventoryModel> GetInventory()
		{
			SqlDataAccess sql = new SqlDataAccess();

			List<InventoryModel> output = sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "TRMData");

			return output;
		}

		public void SaveInventoryRecord(InventoryModel item)
		{
			SqlDataAccess sql = new SqlDataAccess();

			sql.SaveData("dbo.spInventory_Insert", item, "TRMData");
		}
	}
}
