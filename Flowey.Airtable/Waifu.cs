using System;
using System.Collections.Generic;
using System.Text;
using AirtableApiClient;
using System.Threading.Tasks;
using Flowey.Airtable.Objects;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

namespace Flowey.Airtable
{
    public class Waifu
    {
        private AirtableBase Base;
        private string table = "Waifu";
        public Waifu(string ApiKey, string ApiBase)
        {
            Base = new AirtableBase(ApiKey, ApiBase);
        }

        public async Task<WaifuObject> GetWaifus(ulong id)
        {
            WaifuObject waifu = new WaifuObject();
            var records = await Base.ListRecords(table, filterByFormula: "{userId} = " + id);
            foreach(var record in records.Records)
            {
                waifu.Id = id;
                waifu.RecordId = record.Id;
                waifu.Feeds = Convert.ToString(record.GetField("Feeds"));
                waifu.Levels = Convert.ToString(record.GetField("Levels"));
                waifu.Waifus = Convert.ToString(record.GetField("Waifus"));
                waifu.Lewds = Convert.ToString(record.GetField("Lewds"));
                waifu.LastModifiedHour = Convert.ToString(record.GetField("LastModifedHour"));
            }
            return waifu;
        }

        public async Task <bool> CheckIfRecordExist(ulong id)
        {
            var records = await Base.ListRecords(table, filterByFormula: "{userId} = " + id);
            if (records.Records.Count() != 0) return true;
            return false;
        }

        public async Task CreateWaifuRecord(WaifuObject data)
        {
            Fields field = new Fields();
            field.AddField("userId", data.Id);
            field.AddField("Waifus", data.Waifus);
            field.AddField("Levels", data.Levels);
            field.AddField("Lewds", data.Lewds);
            field.AddField("Feeds", data.Feeds);
            field.AddField("LastModifiedHour", data.LastModifiedHour);

            await Base.CreateRecord(table, field);
        }

        public async Task UpdateWaifus(WaifuObject data)
        {
            Fields field = new Fields();
            field.AddField("Waifus", data.Waifus);
            field.AddField("Feeds", data.Feeds);
            field.AddField("Levels", data.Levels);
            field.AddField("Lewds", data.Lewds);
            field.AddField("LastModifiedHour", data.LastModifiedHour);
            await Base.UpdateRecord(table, field, data.RecordId);
        }
    }
}
