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

        public  List<Club> Clubs { get; set; }

        public List<Event> Events { get; set; }

        public User()
        {
            Clubs = new List<Club>();
            Events = new List<Event>();
        }       


    }
    namespace InterestClubWebAPI.DTOs
    {
        public class UserDTO
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? Surname { get; set; }
            public string Login { get; set; }           
            public string? Fatherland { get; set; }
            public Role Role { get; set; }            
        }
    }
}
