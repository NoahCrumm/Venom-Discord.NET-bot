using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VenomBot.Services
{
    public class MemberStats
    {
        public Users User { get; set; }

        [Key]
        public ulong Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public string Nickname { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
