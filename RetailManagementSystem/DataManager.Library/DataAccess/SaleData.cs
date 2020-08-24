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
    public class SaleData
    {
        private readonly IConfiguration config;

        public SaleData(IConfiguration config)
        {
            this.config = config;
        }

        // Transactions in MSSQL should be done in one big chunk, otherwise there is a possibility
        // that corrupt or incomplete data will be added to the db
        // Better for it to fail completely than to have misleading data in the db
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // Refactor for SOLID

            // Sale details models to be saved to db
            List<SaleDetailDBModel> saleDetails = new List<SaleDetailDBModel>();
            ProductData products = new ProductData(config); // temporary instantiation
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

            // opening these queries as transactions allows us to perform several different
            // operations on a set of data, act on it as though the database has consumed the changes
            // then submit all the changes as a batch to the database at once to ensure the transaction
            // is completed fully before being saved into the db
            using(SqlDataAccess sql = new SqlDataAccess(config))
            {
                try
                {
                    sql.StartTransaction("RMSData");

                    sql.SaveDataInTransaction<SaleDBModel>("dbo.spSale_Insert", sale);

                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new
                    { sale.CashierId, sale.SaleDate })
                        .FirstOrDefault();

                    // fill in sale detail models
                    foreach (var item in saleDetails)
                    {
                        // all items belong to this sale id
                        item.SaleId = sale.Id;

                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                    }

                    sql.CommitTransaction();
                }
                catch
                {
                    sql.RollbackTransaction();
                    throw;
                }

                
            }
        }

        public List<SaleReportModel> GetSaleReport()
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { },
                "RMSData");

            return output;
        }
    }
}
