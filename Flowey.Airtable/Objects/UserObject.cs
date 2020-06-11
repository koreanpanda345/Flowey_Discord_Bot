using System;
using System.Collections.Generic;
using System.Text;

namespace Flowey.Airtable.Objects
{
    public class UserObject
    {
        public ulong Id { get; set; }
        public bool Disable { get; set; } = false;
        public int Balance { get; set; } = 0;
        public int Xp { get; set; } = 0;
        public int Level { get; set; } = 1;
        public int Votes { get; set; } = 0;
        public string Description { get; set; } = "No Description";
        public string Background { get; set; } = "https://i.imgur.com/VI1bAUr.jpg";
        public string Birthday { get; set; } = "January 1, 2020";
        public string Marry { get; set; } = "No one";
        public string OwnBgNames { get; set; } = "Default-Pink";
        public string OwnBgUrl { get; set; } = "https://i.imgur.com/VI1bAUr.jpg";
        public int Weekly { get; set; } = 0;
        public string RecordId { get; set; } 
    }
}
