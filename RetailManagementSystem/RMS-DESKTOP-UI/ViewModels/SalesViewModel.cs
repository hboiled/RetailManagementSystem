using Caliburn.Micro;
using RMS_DESKTOP_UI.Library.Api;
using RMS_DESKTOP_UI.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS_DESKTOP_UI.ViewModels
{
    public class SalesViewModel: Screen
    {
		private BindingList<ProductModel> _products;
		private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();
		private IProductEndpoint _productEndpoint;

		public SalesViewModel(IProductEndpoint productEndpoint)
		{
			_productEndpoint = productEndpoint;			
		}

		protected override async void OnViewLoaded(object view)
		{
			// when view is loaded, asynchronously load items from api
			base.OnViewLoaded(view);
			await LoadItems();
		}

		private async Task LoadItems()
		{
			var items = await _productEndpoint.GetAll();
			Products = new BindingList<ProductModel>(items);
		}

		private ProductModel _selectedProduct;

		public ProductModel SelectedProduct
		{
			get { return _selectedProduct; }
			set {
				_selectedProduct = value;
				NotifyOfPropertyChange(() => SelectedProduct);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}


		public BindingList<ProductModel> Products
		{
			get { return _products; }
			set { 
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		// Caliburn micro validates input for an int
		private int _itemQuantity;

		public int ItemQuantity
		{
			get { return _itemQuantity; }
			set { 
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

		public string SubTotal
		{
			get { 
				return "$00.00";
			}			
		}

		public string Tax
		{
			get
			{
				return "$00.00";
			}
		}

		public string Total
		{
			get
			{
				return "$00.00";
			}
		}


		public bool CanAddToCart
		{
			get
			{
				bool output = false;

				if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
				{
					output = true;
				}

				return output;
			}
		}

		public bool CanRemoveFromCart
		{
			get
			{
				bool output = false;

				return output;
			}
		}

		public bool CanCheckOut
		{
			get
			{
				bool output = false;

				return output;
			}
		}

		public void AddToCart()
		{
			CartItemModel item = new CartItemModel
			{
				Product = SelectedProduct,
				QuantityInCart = ItemQuantity
			};

			Cart.Add(item);
		}

		public void RemoveFromCart()
		{

		}

		public void CheckOut()
		{

		}


	}
}
