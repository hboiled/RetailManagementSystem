using DataManager.Library.Internal.DataAccess;
using DataManager.Library.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class ProductData
    {
        private readonly IConfiguration config;

        public ProductData(IConfiguration config)
        {
            this.config = config;
        }

        public List<ProductModel> GetProducts()
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "RMSData");
            return output;
        }

        public ProductModel GetProductById(int id)
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = id }, "RMSData")
                .FirstOrDefault();
            return output;
        }
    }
}
