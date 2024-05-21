using Microsoft.EntityFrameworkCore;
using InterestClubWebAPI.Models;


namespace InterestClubWebAPI.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Event> Events { get; set; } = null !;

        public DbSet<Club> Clubs { get; set; } = null!;

        public ApplicationContext()
        {
            
        }        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.ljhexoykjtmlvbjpriks;Password=Hackaton20052024");

        }
    }
    
}
