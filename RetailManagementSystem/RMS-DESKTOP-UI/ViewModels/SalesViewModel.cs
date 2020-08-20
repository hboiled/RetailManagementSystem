using AutoMapper;
using Caliburn.Micro;
using RMS_DESKTOP_UI.Library.Api;
using RMS_DESKTOP_UI.Library.Helpers;
using RMS_DESKTOP_UI.Library.Models;
using RMS_DESKTOP_UI.Models;
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
		private BindingList<ProductDisplayModel> _products;
		private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
		private IProductEndpoint _productEndpoint;
		private IConfigHelper _configHelper;
		private ISaleEndpoint _saleEndpoint;
		private IMapper _mapper;

		public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, ISaleEndpoint saleEndpoint,
			IMapper mapper)
		{
			_productEndpoint = productEndpoint;
			_configHelper = configHelper;
			_saleEndpoint = saleEndpoint;
			_mapper = mapper;
		}

		protected override async void OnViewLoaded(object view)
		{
			// when view is loaded, asynchronously load items from api
			base.OnViewLoaded(view);
			await LoadItems();
		}

		private async Task LoadItems()
		{
			var itemList = await _productEndpoint.GetAll();
			var items = _mapper.Map<List<ProductDisplayModel>>(itemList);
			Products = new BindingList<ProductDisplayModel>(items);
		}

		private async Task ResetSalesViewModel()
		{
			Cart = new BindingList<CartItemDisplayModel>();

			await LoadItems();

			NotifyPropChangeMoney();
			NotifyOfPropertyChange(() => CanCheckOut);
		}

		private ProductDisplayModel _selectedProduct;

		public ProductDisplayModel SelectedProduct
		{
			get { return _selectedProduct; }
			set {
				_selectedProduct = value;
				NotifyOfPropertyChange(() => SelectedProduct);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		private CartItemDisplayModel _selectedCartItem;

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


		public BindingList<ProductDisplayModel> Products
		{
			get { return _products; }
			set { 
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		// Caliburn micro validates input for an int
		private int _itemQuantity = 1;

		public int ItemQuantity
		{
			get { return _itemQuantity; }
			set { 
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

		public string SubTotal
		{
			get {
				return CalculateSubTotal().ToString("C");
			}			
		}

		private decimal CalculateSubTotal()
		{
			decimal subTotal = 0;

			foreach (var item in Cart)
			{
				subTotal += item.Product.RetailPrice * item.QuantityInCart;
			}

			return subTotal;
		}

		public string Tax
		{
			get
			{
				return CalculateTax().ToString("C");
			}
		}

		private decimal CalculateTax()
		{
			decimal taxAmount = 0;
			decimal taxRate = _configHelper.GetTaxRate();

			taxAmount = Cart
				.Where(x => x.Product.IsTaxable)
				.Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

			//foreach (var item in Cart)
			//{
			//	if (item.Product.IsTaxable)
			//	{
			//		taxAmount += (item.Product.RetailPrice * item.QuantityInCart) * taxRate;
			//	}
			//}

			return taxAmount;
		}

		public string Total
		{
			get
			{
				decimal total = CalculateSubTotal() + CalculateTax();
				return total.ToString("C");
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

				if (SelectedCartItem != null && SelectedCartItem?.QuantityInCart > 0)
				{
					output = true;
				}

				return output;
			}
		}

		public bool CanCheckOut
		{
			get
			{
				bool output = false;

				if (Cart.Count > 0)
				{
					output = true;
				}

				return output;
			}
		}

		public void AddToCart()
		{
			CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

			if (existingItem != null)
			{
				existingItem.QuantityInCart += ItemQuantity;
				//temp hack to refresh display
				//Cart.Remove(existingItem);
				//Cart.Add(existingItem);
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

			NotifyPropChangeMoney();
			NotifyOfPropertyChange(() => CanAddToCart);
			NotifyOfPropertyChange(() => CanCheckOut);
		}

		public async Task CheckOut()
		{
			SaleModel sale = new SaleModel();

			foreach (var item in Cart)
			{
				sale.SaleDetails.Add(new SaleDetailModel
				{
					ProductId = item.Product.Id,
					Quantity = item.QuantityInCart
				});
			}

			await _saleEndpoint.PostSale(sale);

			await ResetSalesViewModel();
		}

		public void NotifyPropChangeMoney()
		{
			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
		}


	}
}
