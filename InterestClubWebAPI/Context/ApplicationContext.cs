using Microsoft.EntityFrameworkCore;
using InterestClubWebAPI.Models;
using System.IO;



namespace InterestClubWebAPI.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Club> Clubs { get; set; }

        public DbSet<Discussion> Discussions { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Image> Images { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //    optionsBuilder.UseNpgsql("Host=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.ljhexoykjtmlvbjpriks;Password=Hackaton20052024");
        //    //optionsBuilder.EnableSensitiveDataLogging();
        //    //optionsBuilder.UseLazyLoadingProxies();
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<User>()
                .HasMany(u => u.Clubs)
                .WithMany(c => c.Users);                


            modelBuilder.Entity<User>()
                .HasMany(u => u.Events)
                .WithMany(e => e.Members);

            //modelBuilder.Entity<Discussion>()
            //    .HasMany(d => d.comments)
            //    .WithOne()
            //    .HasForeignKey(c => c.DiscussionId);

            base.OnModelCreating(modelBuilder);
        }

    }

}
