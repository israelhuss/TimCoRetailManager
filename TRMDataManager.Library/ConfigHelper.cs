using System.Configuration;

namespace TRMDataManager.Library
{
	public static class ConfigHelper
	{
		public static decimal GetTaxRate()
		{
			string rateText = ConfigurationManager.AppSettings["taxRate"];

			bool IsValidTaxRate = decimal.TryParse(rateText, out decimal output);

			if (!IsValidTaxRate)
			{
				throw new ConfigurationErrorsException("The tax rate is not set up properly.");
			}

			return output;
		}
	}
}
