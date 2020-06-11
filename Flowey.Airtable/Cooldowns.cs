using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;
using Flowey.Airtable.Objects;
namespace Flowey.Airtable
{
    public class Cooldowns
    {
        private string table = "Cooldowns";
        private AirtableBase Base;
        public Cooldowns(string ApiKey, string ApiBase)
        {
            Base = new AirtableBase(ApiKey, ApiBase);
        }

        public async Task<bool> CheckIfRecordExist(ulong id)
        {
            var records = await Base.ListRecords(table, filterByFormula: "{userId} = " + id);
            if (records.Records.Count() != 0) return true;
            return false;
        }

        public async Task<CooldownObject> GetCooldowns(ulong id)
        {
            CooldownObject cooldowns = new CooldownObject();

            var records = await Base.ListRecords(table, filterByFormula: "{userId} = " + id);
            foreach(var record in records.Records)
            {

                cooldowns.Bless = Convert.ToInt64(record.GetField("Bless"));
                cooldowns.Daily = Convert.ToInt64(record.GetField("Daily"));
                cooldowns.Vote = Convert.ToInt64(record.GetField("Vote"));
                cooldowns.RecordId = record.Id;
                cooldowns.Id = id;
            }
            Console.WriteLine(cooldowns.Daily);
            return cooldowns;
        }

        public async Task CreateCooldowns(CooldownObject data)
        {
            Fields field = new Fields();
            field.AddField("Daily", data.Daily);
            field.AddField("Bless", data.Bless);
            field.AddField("Pluck", data.Pluck);
            field.AddField("Vote", data.Vote);
            field.AddField("Work", data.Work);
            field.AddField("Thievery", data.Thievery);
            field.AddField("userId", data.Id);

            await Base.CreateRecord(table, field);
        }

        public async Task UpdateCooldowns(CooldownObject data)
        {
            Fields field = new Fields();
            field.AddField("Daily", data.Daily);
            field.AddField("Bless", data.Bless);
            field.AddField("Pluck", data.Pluck);
            field.AddField("Vote", data.Vote);
            field.AddField("Work", data.Work);
            field.AddField("Thievery", data.Thievery);

            await Base.UpdateRecord(table, field, data.RecordId);
        }
    }
}
