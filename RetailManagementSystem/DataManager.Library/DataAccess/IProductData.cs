using DataManager.Library.Models;
using System.Collections.Generic;

namespace DataManager.Library.DataAccess
{
    public interface IProductData
    {
        ProductModel GetProductById(int id);
        List<ProductModel> GetProducts();
    }
}