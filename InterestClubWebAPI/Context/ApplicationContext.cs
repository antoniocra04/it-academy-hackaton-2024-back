using Microsoft.EntityFrameworkCore;
using InterestClubWebAPI.Models;
using static InterestClubWebAPI.Models.User;


namespace InterestClubWebAPI.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } 

        public DbSet<Event> Events { get; set; } 

        public DbSet<Club> Clubs { get; set; }

        public DbSet<UserClub> UserClubs { get; set; }
        public DbSet<ClubEvent> ClubEvents { get; set; }
        public DbSet<EventMember> EventMembers { get; set; }

        public ApplicationContext()
        {
            
        }        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.ljhexoykjtmlvbjpriks;Password=Hackaton20052024");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserClub>()
                .HasKey(uc => new { uc.UserId, uc.ClubId });

            modelBuilder.Entity<UserClub>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserClubs)
                .HasForeignKey(uc => uc.UserId);

            //modelBuilder.Entity<UserClub>()
            //    .HasOne(uc => uc.Club)
            //    .WithMany(c => c.UserClubs)
            //    .HasForeignKey(uc => uc.ClubId);

            modelBuilder.Entity<ClubEvent>()
                .HasKey(ce => new { ce.ClubId, ce.EventId });

            modelBuilder.Entity<ClubEvent>()
                .HasOne(ce => ce.Club)
                .WithMany(c => c.ClubEvents)
                .HasForeignKey(ce => ce.ClubId);

            //modelBuilder.Entity<ClubEvent>()
            //    .HasOne(ce => ce.Event)
            //    .WithMany(e => e.ClubEvents)
            //    .HasForeignKey(ce => ce.EventId);

            modelBuilder.Entity<EventMember>()
                .HasKey(em => new { em.EventId, em.UserId });

            //modelBuilder.Entity<EventMember>()
            //    .HasOne(em => em.Event)
            //    .WithMany(e => e.EventMembers)
            //    .HasForeignKey(em => em.EventId);

            modelBuilder.Entity<EventMember>()
                .HasOne(em => em.User)
                .WithMany(u => u.EventMembers)
                .HasForeignKey(em => em.UserId);
        }
    }
    
}
