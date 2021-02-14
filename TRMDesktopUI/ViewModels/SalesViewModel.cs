using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private BindingList<ProductDisplayModel> _products;
		private ProductDisplayModel _selectedProduct;
		private CartItemDisplayModel _selectedCartItem;
		private int _itemQuantity = 1;
		private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
		private IProductEndpoint _productEndpoint;
		private IConfigHelper _configHelper;
		private ISaleEndpoint _saleEndpoint;
		private IMapper _mapper;
		private StatusInfoViewModel _status;
		private readonly IWindowManager _window;

		public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper,
			ISaleEndpoint saleEndpoint, IMapper mapper, StatusInfoViewModel status, IWindowManager window)
		{
			_productEndpoint = productEndpoint;
			_configHelper = configHelper;
			_saleEndpoint = saleEndpoint;
			_mapper = mapper;
			_status = status;
			_window = window;
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			try
			{
				await LoadProducts();
			}
			catch (Exception ex)
			{
				dynamic settings = new ExpandoObject();
				settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				settings.ResizeMode = ResizeMode.NoResize;
				settings.Title = "System Error";

				if (ex.Message == "Unauthorized")
				{
					_status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form.");
					_window.ShowDialog(_status, null, settings);
				}
				else
				{
					_status.UpdateMessage("Fatal Exception", ex.Message);
					_window.ShowDialog(_status, null, settings);
				}

				TryClose();
			}
		}

		private async Task LoadProducts()
		{
			var productsList = await _productEndpoint.GetAll();
			var products = _mapper.Map<List<ProductDisplayModel>>(productsList);
			Products = new BindingList<ProductDisplayModel>(products);
		}

		public BindingList<ProductDisplayModel> Products
		{
			get { return _products; }
			set
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		public ProductDisplayModel SelectedProduct
		{
			get { return _selectedProduct; }
			set
			{
				_selectedProduct = value;
				NotifyOfPropertyChange(() => SelectedProduct);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		public CartItemDisplayModel SelectedCartItem
		{
			get { return _selectedCartItem; }
			set
			{
				_selectedCartItem = value;
				NotifyOfPropertyChange(() => SelectedCartItem);
				NotifyOfPropertyChange(() => CanRemoveFromCart);
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

		public BindingList<CartItemDisplayModel> Cart
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

			taxAmount = Cart
				.Where(x => x.Product.IsTaxable)
				.Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

			//foreach (var item in Cart)
			//{
			//	if (item.Product.IsTaxable)
			//	{
			//		taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxRate);
			//	}
			//}

			return taxAmount;
		}
		private async Task ResetSalesViewModel()
		{
			Cart = new BindingList<CartItemDisplayModel>();
			await LoadProducts();

			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
			NotifyOfPropertyChange(() => CanCheckOut);
		}

		// TODO - Make sure something is selected...
		public bool CanAddToCart => ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity;
		public bool CanRemoveFromCart => SelectedCartItem != null && SelectedCartItem?.QuantityInCart > 0;
		public bool CanCheckOut => Cart.Count > 0;

		public void AddToCart()
		{
			CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

			if (existingItem != null)
			{
				existingItem.QuantityInCart += ItemQuantity;
			}
			else
			{
				CartItemDisplayModel item = new CartItemDisplayModel
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
			NotifyOfPropertyChange(() => CanCheckOut);
		}

		public void RemoveFromCart()
		{
			SelectedCartItem.Product.QuantityInStock += 1;
			if (SelectedCartItem.QuantityInCart > 1)
			{
				SelectedCartItem.QuantityInCart -= 1;
			}
			else
			{
				Cart.Remove(SelectedCartItem);
			}

			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
			NotifyOfPropertyChange(() => CanAddToCart);
			NotifyOfPropertyChange(() => CanCheckOut);
		}

		public async Task CheckOut()
		{
			SaleModel sale = new SaleModel();

			foreach (var item in Cart)
			{
				sale.saleDetails.Add(new SaleDetailModel
				{
					ProductId = item.Product.Id,
					Quantity = item.QuantityInCart
				});
			}

			await _saleEndpoint.PostSale(sale);

			await ResetSalesViewModel();
		}

	}
}
