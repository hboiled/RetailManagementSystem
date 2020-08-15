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
		private BindingList<ProductModel> _cart;
		private IProductEndpoint _productEndpoint;

		public SalesViewModel(IProductEndpoint productEndpoint)
		{
			_productEndpoint = productEndpoint;
			_products = new BindingList<ProductModel>();
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			await LoadItems();
		}

		private async Task LoadItems()
		{
			var items = await _productEndpoint.GetAll();
			Products = new BindingList<ProductModel>(items);
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
			}
		}
		

		public BindingList<ProductModel> Cart
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

		}

		public void RemoveFromCart()
		{

		}

		public void CheckOut()
		{

		}


	}
}
