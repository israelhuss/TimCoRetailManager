using System.Configuration;

namespace TRMDesktopUI.Library.Helpers
{
	public class ConfigHelper : IConfigHelper
	{
		public decimal GetTaxRate()
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
