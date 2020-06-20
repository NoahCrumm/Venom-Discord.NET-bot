using Microsoft.EntityFrameworkCore;

namespace VenomBot.Services
{
    public class BotContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<MemberStats> MemberStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(c => new { c.Id, c.GuildId });

                entity.HasMany(e => e.MemberStats)
                    .WithOne(e => e.User)
                    .HasPrincipalKey(e => e.Id);

            });


            modelBuilder.Entity<MemberStats>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.MemberStats)
                    .HasForeignKey(e => e.UserId);
            });
        }
    }
}
