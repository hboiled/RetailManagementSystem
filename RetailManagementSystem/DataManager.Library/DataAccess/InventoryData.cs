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
    public class InventoryData : IInventoryData
    {
        // IConfiguration is provided by net core by default for DI
        //private readonly IConfiguration config;
        private readonly ISqlDataAccess sqlDataAccess;

        public InventoryData(IConfiguration config, ISqlDataAccess sqlDataAccess)
        {
            //this.config = config;
            this.sqlDataAccess = sqlDataAccess;
        }

        public List<InventoryModel> GetInventory()
        {
            return sqlDataAccess.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { },
                "RMSData");
        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            sqlDataAccess.SaveData<InventoryModel>("dbo.spInventory_Insert", item, "RMSData");
        }
    }
}
