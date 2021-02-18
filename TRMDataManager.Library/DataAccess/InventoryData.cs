using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
	public class InventoryData : IInventoryData
	{
		private readonly IConfiguration _config;
		private readonly ISqlDataAccess _sql;

		public InventoryData(IConfiguration config, ISqlDataAccess sql)
		{
			_config = config;
			_sql = sql;
		}

		public List<InventoryModel> GetInventory()
		{
			List<InventoryModel> output = _sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "TRMData");

			return output;
		}

		public void SaveInventoryRecord(InventoryModel item)
		{
			_sql.SaveData("dbo.spInventory_Insert", item, "TRMData");
		}
	}
}
