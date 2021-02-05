namespace TRMDesktopUI.Library.Models
{
	public class CartItemModel
	{
		public ProductModel Product { get; set; }
		public int QuantityInCart { get; set; }
		public string DisplayText => $"{ Product.ProductName } ({ QuantityInCart })";
	}
}
