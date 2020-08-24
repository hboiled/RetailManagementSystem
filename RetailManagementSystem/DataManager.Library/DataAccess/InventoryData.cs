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
    public class InventoryData
    {
        // IConfiguration is provided by net core by default for DI
        private readonly IConfiguration config;

        public InventoryData(IConfiguration config)
        {
            this.config = config;
        }

        public List<InventoryModel> GetInventory()
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            var output = sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { },
                "RMSData");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            SqlDataAccess sql = new SqlDataAccess(config);

            sql.SaveData<InventoryModel>("dbo.spInventory_Insert", item, "RMSData");
        }
    }
}
