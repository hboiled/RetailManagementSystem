using RMS_DESKTOP_UI.Library.Models;
using System.Threading.Tasks;

namespace RMS_DESKTOP_UI.Library.Api
{
    public interface ISaleEndpoint
    {
        Task PostSale(SaleModel sale);
    }
}