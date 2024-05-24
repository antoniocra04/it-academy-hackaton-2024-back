using InterestClubWebAPI.Enums;

namespace InterestClubWebAPI.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string? Fatherland { get; set; }

        public Role Role { get; set; }

        public virtual List<Club> UserClubs { get; set; }

        public virtual List<Event> UserEvents { get; set; }

        public User()
        {
            UserClubs = new List<Club>();
            UserEvents = new List<Event>();
        }       


    }
    
}
