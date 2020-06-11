using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;
using Flowey.Airtable.Objects;
namespace Flowey.Airtable
{
    public class Shop
    {
        private AirtableBase Base;
        public Shop(string ApiKey, string ApiBase)
        {
            Base = new AirtableBase(ApiKey, ApiBase);
        }
        public async Task<List<BackgroundObject>> GetBackgrounds()
        {
            List<BackgroundObject> backgrounds = new List<BackgroundObject>();
            var records = await Base.ListRecords("Backgrounds");
            foreach(var record in records.Records)
            {
                backgrounds.Add(new BackgroundObject()
                {
                    Name = Convert.ToString(record.GetField("Name")),
                    Url = Convert.ToString(record.GetField("url")),
                    Price = Convert.ToInt32(record.GetField("price")),
                    Nitro = Convert.ToBoolean(record.GetField("Nitro")),
                    Dev = Convert.ToBoolean(record.GetField("Dev")) 
                });
            }

            return backgrounds;
        }

        public async Task<List<ShopObject>> GetShop()
        {
            List<ShopObject> shop = new List<ShopObject>();
            var records = await Base.ListRecords("Shop");
            foreach(var record in records.Records)
            {
                shop.Add(new ShopObject()
                {
                    Name = Convert.ToString(record.GetField("Name")),
                    Description = Convert.ToString(record.GetField("Description")),
                    Price = Convert.ToInt32(record.GetField("Price")),
                    Id = Convert.ToInt32(record.GetField("id")),
                    Category = Convert.ToString(record.GetField("Category"))
                });
            }

            return shop;
        }
    }
}
