using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;
using Flowey.Airtable.Objects;
namespace Flowey.Airtable
{
    public class UserProfile
    {
        private AirtableBase Base;
        private string table = "Users";
        public UserProfile(string ApiKey, string ApiBase)
        {
            Base = new AirtableBase(ApiKey, ApiBase);
        }

        public async Task<bool> CheckIfRecordExist(ulong id)
        {
            var records = await Base.ListRecords(table, filterByFormula: "{ID} = " + id);
            if (records.Records.Count() != 0) return true;
            return false;
        }

        public async Task<UserObject> GetUserProfile(ulong id)
        {
            UserObject user = new UserObject();
            var records = await Base.ListRecords(table, filterByFormula: "{ID} = " + id);
            foreach(var record in records.Records)
            {
                user.Id = id;
                user.Disable = Convert.ToBoolean(record.GetField("Disable"));
                user.Balance = Convert.ToInt32(record.GetField("balance"));
                user.Xp = Convert.ToInt32(record.GetField("xp"));
                user.Level = Convert.ToInt32(record.GetField("level"));
                user.Votes = Convert.ToInt32(record.GetField("votes"));
                user.Description = Convert.ToString(record.GetField("description"));
                user.Background = Convert.ToString(record.GetField("background"));
                user.Birthday = Convert.ToString(record.GetField("birthday"));
                user.Marry = Convert.ToString(record.GetField("marry"));
                user.OwnBgNames = Convert.ToString(record.GetField("ownBgName"));
                user.OwnBgUrl = Convert.ToString(record.GetField("ownBgUrl"));
                user.Weekly = Convert.ToInt32(record.GetField("weekly"));
                user.RecordId = record.Id;
            }
            Console.WriteLine(user.Balance);
            return user;
        }

        public async Task UpdateUserProfile(UserObject data)
        {
            Fields field = new Fields();
            field.AddField("Disable", data.Disable);
            field.AddField("balance", data.Balance);
            field.AddField("xp", data.Xp);
            field.AddField("level", data.Level);
            field.AddField("votes", data.Votes);
            field.AddField("description", data.Description);
            field.AddField("background", data.Background);
            field.AddField("birthday", data.Birthday);
            field.AddField("marry", data.Marry);
            field.AddField("ownBgName", data.OwnBgNames);
            field.AddField("ownBgUrl", data.OwnBgUrl);
            field.AddField("weekly", data.Weekly);

            await Base.UpdateRecord(table, field, data.RecordId);
        }

        public async Task CreateUserProfile(UserObject data)
        {
            Fields field = new Fields();
            field.AddField("ID", data.Id);
            field.AddField("Disable", data.Disable);
            field.AddField("balance", data.Balance);
            field.AddField("xp", data.Xp);
            field.AddField("level", data.Level);
            field.AddField("votes", data.Votes);
            field.AddField("description", data.Description);
            field.AddField("background", data.Background);
            field.AddField("birthday", data.Birthday);
            field.AddField("marry", data.Marry);
            field.AddField("ownBgName", data.OwnBgNames);
            field.AddField("ownBgUrl", data.OwnBgUrl);
            field.AddField("weekly", data.OwnBgNames);

            await Base.CreateRecord(table, field);
        }
    }
}
