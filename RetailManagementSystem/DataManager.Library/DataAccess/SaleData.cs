using DataManager.Library.Internal.DataAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // Refactor for SOLID

            // Sale details models to be saved to db
            List<SaleDetailDBModel> saleDetails = new List<SaleDetailDBModel>();
            ProductData products = new ProductData(); // temporary instantiation
            var taxRate = ConfigHelper.GetTaxRate();

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                var productInfo = products.GetProductById(item.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in the database.");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }

                saleDetails.Add(detail);
            }

            // Sale db model
            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = saleDetails.Sum(x => x.PurchasePrice),
                Tax = saleDetails.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.SubTotal + sale.Tax;

            SqlDataAccess sql = new SqlDataAccess();
            sql.SaveData<SaleDBModel>("dbo.spSale_Insert", sale, "RMSData");

            sale.Id = sql.LoadData<int, dynamic>("spSale_Lookup", new 
            {sale.CashierId, sale.SaleDate }, "RMSData")
                .FirstOrDefault();

            // fill in sale detail models
            foreach (var item in saleDetails)
            {
                // all items belong to this sale id
                item.SaleId = sale.Id;

                sql.SaveData("dbo.spSaleDetail_Insert", item, "RMSData");
            }

            
        }
    }
}
