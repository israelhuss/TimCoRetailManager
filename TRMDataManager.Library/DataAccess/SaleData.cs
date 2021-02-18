using System;
using System.Collections.Generic;
using System.Linq;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
	public class SaleData : ISaleData
	{
		private readonly IProductData _productData;
		private readonly ISqlDataAccess _sql;

		public SaleData(IProductData productData, ISqlDataAccess sql)
		{
			_productData = productData;
			_sql = sql;
		}

		public void SaveSale(SaleModel saleInfo, string cashierId)
		{
			//TODO - Make this better

			//start filling in the sale detail models we'll save to the database
			List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
			decimal taxRate = ConfigHelper.GetTaxRate() / 100;

			foreach (var item in saleInfo.saleDetails)
			{
				var detail = new SaleDetailDBModel
				{
					ProductId = item.ProductId,
					Quantity = item.Quantity
				};

				var productInfo = _productData.GetProductById(detail.ProductId);

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


			try
			{
				_sql.StartTransaction("TRMData");

				//Save the sale model
				_sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

				// Get the Id from the sale model
				sale.Id = _sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

				// Finnish filling in the sale detail model
				foreach (var item in details)
				{
					item.SaleId = sale.Id;
					// Save the sale detail model
					_sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
				}

				_sql.CommitTransaction();
			}
			catch
			{
				_sql.RollBackTransaction();
				throw;
			}

		}

		public List<SaleReportModel> GetSaleReport()
		{
			var output = _sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");

			return output;
		}
	}
}
