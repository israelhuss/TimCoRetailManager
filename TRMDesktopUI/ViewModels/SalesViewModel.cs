using Caliburn.Micro;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private BindingList<ProductModel> _products;
		private ProductModel _selectedProduct;
		private int _itemQuantity = 1;
		private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();
		private IProductEndpoint _productEndpoint;
		private IConfigHelper _configHelper;

		public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper)
		{
			_productEndpoint = productEndpoint;
			_configHelper = configHelper;

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

		public ProductModel SelectedProduct
		{
			get { return _selectedProduct; }
			set
			{
				_selectedProduct = value;
				NotifyOfPropertyChange(() => SelectedProduct);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		public int ItemQuantity
		{
			get { return _itemQuantity; }
			set
			{
				_itemQuantity = value;
				NotifyOfPropertyChange(() => ItemQuantity);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		public BindingList<CartItemModel> Cart
		{
			get { return _cart; }
			set
			{
				_cart = value;
				NotifyOfPropertyChange(() => Cart);
			}
		}

		public string SubTotal => CalculateSubTotal().ToString("C");
		public string Tax => CalculateTax().ToString("C");
		public string Total => (CalculateSubTotal() + CalculateTax()).ToString("C");


		private decimal CalculateSubTotal()
		{
			decimal subTotal = 0;

			foreach (var item in Cart)
			{
				subTotal += (item.Product.RetailPrice * item.QuantityInCart);
			}

			return subTotal;
		}

		private decimal CalculateTax()
		{
			decimal taxAmount = 0;
			decimal taxRate = _configHelper.GetTaxRate() / 100;

			foreach (var item in Cart)
			{
				if (item.Product.IsTaxable)
				{
					taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxRate);
				}
			}

			return taxAmount;
		}

		// TODO - Make sure something is selected...
		public bool CanAddToCart => ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity;
		public bool CanRemoveFromCart => false;
		public bool CanCheckOut => false;

		public void AddToCart()
		{
			CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

			if (existingItem != null)
			{
				existingItem.QuantityInCart += ItemQuantity;
				Cart.Remove(existingItem);
				Cart.Add(existingItem);
			}
			else
			{
				CartItemModel item = new CartItemModel
				{
					Product = SelectedProduct,
					QuantityInCart = ItemQuantity
				};
				Cart.Add(item);
			}

			SelectedProduct.QuantityInStock -= ItemQuantity;
			ItemQuantity = 1;
			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
		}

		public void RemoveFromCart()
		{

			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
		}

		public void CheckOut()
		{

		}
	}
}
