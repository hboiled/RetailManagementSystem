using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataManager.Library.DataAccess;
using DataManager.Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace RMSApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductData productData;

        public ProductController(IProductData productData)
        {
            this.productData = productData;
        }

        // additive roles are marked by 2 separate authorize tags, users must have both roles to access them        
        [Authorize(Roles = "Cashier")]
        [HttpGet]
        public List<ProductModel> Get()
        {
            return productData.GetProducts();
        }
    }
}