using System;
using System.Collections.Generic;
using System.Linq;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
	public class SaleData
	{
		public void SaveSale(SaleModel saleInfo, string cashierId)
		{
			//TODO - Make this better

			//start filling in the sale detail models we'll save to the database
			List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
			ProductData products = new ProductData();
			decimal taxRate = ConfigHelper.GetTaxRate() / 100;

			foreach (var item in saleInfo.saleDetails)
			{
				var detail = new SaleDetailDBModel
				{
					ProductId = item.ProductId,
					Quantity = item.Quantity
				};

				var productInfo = products.GetProductById(detail.ProductId);

				if (productInfo == null)
				{
					throw new Exception($"The product Id of {detail.ProductId} could not be found.");
				}

				detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

				if (productInfo.IsTaxable)
				{
					detail.Tax = (detail.PurchasePrice * taxRate);
				}

				details.Add(detail);
			}

			// Create a SaleModel
			SaleDBModel sale = new SaleDBModel
			{
				SubTotal = details.Sum(x => x.PurchasePrice),
				Tax = details.Sum(x => x.Tax),
				CashierId = cashierId
			};

			sale.Total = sale.SubTotal + sale.Tax;

			// Save the sale to the database
			SqlDataAccess sql = new SqlDataAccess();
			sql.SaveData("dbo.spSale_Insert", sale, "TRMData");

			// Get the Id from the sale model
			sale.Id = sql.LoadData<int, dynamic>("dbo.spSale_Lookup", new { sale.CashierId, sale.SaleDate }, "TRMData").FirstOrDefault();

			// Finnish filling in the sale detail model
			foreach (var item in details)
			{
				item.SaleId = sale.Id;
				// Save the sale detail model
				sql.SaveData("dbo.spSaleDetail_Insert", item, "TRMData");
			}
		}
	}
}
