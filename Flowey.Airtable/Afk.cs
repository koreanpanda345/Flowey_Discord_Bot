using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;
using Flowey.Airtable.Objects;
namespace Flowey.Airtable
{
    public class Afk
    {
        private AirtableBase Base;
        private string table = "AFK";
        public Afk(string ApiKey, string ApiBase)
        {
            Base = new AirtableBase(ApiKey, ApiBase);
        }

        public async Task<bool> CheckIfRecordExist(ulong id)
        {
            var records = await Base.ListRecords(table, filterByFormula: "{id} = " + id);
            if (records.Records.Count() != 0) return true;
            return false;
        }

        public async Task<AfkObject> GetAfk(ulong id)
        {
            AfkObject afk = new AfkObject();

            var records = await Base.ListRecords(table, filterByFormula: "{id} = " + id);
            foreach(var record in records.Records)
            {
                afk.Id = id;
                afk.IsAfk = Convert.ToBoolean(record.GetField("isAfk"));
                afk.Message = Convert.ToString(record.GetField("message"));
                afk.RecordId = record.Id;
            }

            return afk;
        }

        public async Task UpdateAfk(AfkObject data)
        {
            Fields field = new Fields();
            field.AddField("isAfk", data.IsAfk);
            field.AddField("message", data.Message);

            var result = await Base.UpdateRecord(table, field, data.RecordId);
            Console.WriteLine($"Update AFK Result: {result.Success}");
            Console.WriteLine($"Error: {result.AirtableApiError}");
        }

        public async Task CreateAfk(AfkObject data)
        {
            Fields field = new Fields();
            field.AddField("id", data.Id);
            field.AddField("isAfk", data.IsAfk);
            field.AddField("message", data.Message);

            await Base.CreateRecord(table, field);
        }
    }
}
