using DataManager.Library.Internal.DataAccess;
using DataManager.Library.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class SaleData : ISaleData
    {
        private readonly ISqlDataAccess sqlDataAccess;
        private readonly IProductData productData;

        public SaleData(ISqlDataAccess sqlDataAccess, IProductData productData)
        {
            this.sqlDataAccess = sqlDataAccess;
            this.productData = productData;
        }

        // Transactions in MSSQL should be done in one big chunk, otherwise there is a possibility
        // that corrupt or incomplete data will be added to the db
        // Better for it to fail completely than to have misleading data in the db
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // Refactor for SOLID

            // Sale details models to be saved to db
            List<SaleDetailDBModel> saleDetails = new List<SaleDetailDBModel>();
            //ProductData products = new ProductData(config); // temporary instantiation
            var taxRate = ConfigHelper.GetTaxRate();

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                var productInfo = productData.GetProductById(item.ProductId);

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

            // opening these queries as transactions allows us to perform several different
            // operations on a set of data, act on it as though the database has consumed the changes
            // then submit all the changes as a batch to the database at once to ensure the transaction
            // is completed fully before being saved into the db
            
            try
            {
                sqlDataAccess.StartTransaction("RMSData");

                sqlDataAccess.SaveDataInTransaction<SaleDBModel>("dbo.spSale_Insert", sale);

                sale.Id = sqlDataAccess.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new
                { sale.CashierId, sale.SaleDate })
                    .FirstOrDefault();

                // fill in sale detail models
                foreach (var item in saleDetails)
                {
                    // all items belong to this sale id
                    item.SaleId = sale.Id;

                    sqlDataAccess.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }

                sqlDataAccess.CommitTransaction();
            }
            catch
            {
                sqlDataAccess.RollbackTransaction();
                throw;
            }


            
        }

        public List<SaleReportModel> GetSaleReport()
        {
            return sqlDataAccess.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { },
                "RMSData");
        }
    }
}
