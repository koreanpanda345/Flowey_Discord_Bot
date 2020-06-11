using System;
using System.Collections.Generic;
using System.Text;

namespace Flowey.Airtable.Objects
{
    public class AfkObject
    {
        public ulong Id { get; set; }
        public bool IsAfk { get; set; } = false;
        public string Message { get; set; } = "AFK";
        public string RecordId { get; set; }
    }
}
