using Caliburn.Micro;
using System.ComponentModel;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private BindingList<string> _products;
		private string _itemQuantity;
		private BindingList<string> _cart;

		public BindingList<string> Products
		{
			get { return _products; }
			set
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		public string ItemQuantity
		{
			get { return _itemQuantity; }
			set
			{
				_itemQuantity = value;
				NotifyOfPropertyChange(() => ItemQuantity);
			}
		}


		public BindingList<string> Cart
		{
			get { return _cart; }
			set
			{
				_cart = value;
				NotifyOfPropertyChange(() => Cart);
			}
		}

		public string SubTotal => "$0.00";
		public string Tax => "$0.00";
		public string Total => "$0.00";



		// TODO - Make sure something is selected...
		public bool CanAddToCart => true;
		public bool CanRemoveFromCart => true;
		public bool CanCheckOut => true;

		public void AddToCart()
		{

		}

		public void RemoveFromCart()
		{

		}

		public void CheckOut()
		{

		}
	}
}
