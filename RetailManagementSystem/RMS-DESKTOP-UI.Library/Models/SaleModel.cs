using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS_DESKTOP_UI.Library.Models
{
    public class SaleModel
    {
        // Send a list of sale details to the api
        // Then the api is responsible for retrieving all relevant information
        // based on the sale model, price/tax etc all calculated server side
        public List<SaleDetailModel> SaleDetails { get; set; } = new List<SaleDetailModel>();
    }
}
