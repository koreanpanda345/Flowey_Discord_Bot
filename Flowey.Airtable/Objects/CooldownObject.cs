using System;
using System.Collections.Generic;
using System.Text;

namespace Flowey.Airtable.Objects
{
    public class CooldownObject
    {
        public ulong Id { get; set; }
        public string RecordId { get; set; }
        public long Daily { get; set; } = 0;
        public long Vote { get; set; } = 0;
        public long Pluck { get; set; } = 0;
        public long Bless { get; set; } = 0;
        public long Work { get; set; } = 0;
        public long Thievery { get; set; } = 0;
    }
}
