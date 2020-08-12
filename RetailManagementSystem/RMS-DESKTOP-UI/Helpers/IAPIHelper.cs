using RMS_DESKTOP_UI.Models;
using System.Threading.Tasks;

namespace RMS_DESKTOP_UI.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
    }
}