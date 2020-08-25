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
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess sqlDataAccess;

        public UserData(ISqlDataAccess sqlDataAccess)
        {
            this.sqlDataAccess = sqlDataAccess;
        }

        // get info from user table

        public List<UserModel> GetUserById(string Id)
        {
            var p = new { Id = Id };

            return sqlDataAccess.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "RMSData");
        }
    }
}
