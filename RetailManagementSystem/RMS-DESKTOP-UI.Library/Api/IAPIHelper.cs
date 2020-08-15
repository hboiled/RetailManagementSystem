using RMS_DESKTOP_UI.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace RMS_DESKTOP_UI.Library.Api
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
        Task GetLoggedInUserInfo(string token);
        HttpClient ApiClient { get; }
    }
}