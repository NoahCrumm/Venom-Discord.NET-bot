using System;
using System.Collections.Generic;
using System.Text;

namespace VenomBot.Services
{
    public class Users
    {
        public ulong GuildId { get; set; }
        public ulong Id { get; set; }
        public string Nickname { get; set; }
        public string UserName { get; set; }
        public string TimeZoneId { get; set; }
        public DateTime LastSeen { get; set; }
        public ICollection<MemberStats> MemberStats { get; set; } = new HashSet<MemberStats>();
        public TimeZoneInfo TimeZone { get; set; }

    }
}
