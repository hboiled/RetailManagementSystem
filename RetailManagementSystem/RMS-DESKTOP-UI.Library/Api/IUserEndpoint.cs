using RMS_DESKTOP_UI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMS_DESKTOP_UI.Library.Api
{
    public interface IUserEndpoint
    {
        Task<List<UserModel>> GetAll();
    }
}