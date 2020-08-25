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
    public class ProductData : IProductData
    {
        private readonly ISqlDataAccess sqlDataAccess;

        public ProductData(ISqlDataAccess sqlDataAccess)
        {            
            this.sqlDataAccess = sqlDataAccess;
        }

        public List<ProductModel> GetProducts()
        {
            return sqlDataAccess.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "RMSData");
            
        }

        public ProductModel GetProductById(int id)
        {
            return sqlDataAccess.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = id }, "RMSData")
                .FirstOrDefault();            
        }
    }
}
