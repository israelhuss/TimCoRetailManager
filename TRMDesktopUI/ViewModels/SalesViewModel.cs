using Caliburn.Micro;
using System.ComponentModel;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private BindingList<ProductModel> _products;
		private int _itemQuantity;
		private BindingList<string> _cart;
		private IProductEndpoint _productEndpoint;

		public SalesViewModel(IProductEndpoint productEndpoint)
		{
			_productEndpoint = productEndpoint;
			
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			await LoadProducts();
		}

		private async Task LoadProducts()
		{
			var productsList = await _productEndpoint.GetAll();
			Products = new BindingList<ProductModel>(productsList);
		}

		public BindingList<ProductModel> Products
		{
			get { return _products; }
			set
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		public int ItemQuantity
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
		public bool CanAddToCart => false;
		public bool CanRemoveFromCart => false;
		public bool CanCheckOut => false;

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
