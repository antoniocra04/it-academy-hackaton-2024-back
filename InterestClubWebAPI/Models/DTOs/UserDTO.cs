using InterestClubWebAPI.Enums;

namespace InterestClubWebAPI.Models.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string? Fatherland { get; set; }
        public Role Role { get; set; }

        public List<string> ClubsId { get; set; }

        public List<string> EventsId { get; set; }

        public UserDTO()
        {

            ClubsId = new List<string>();

            EventsId = new List<string>();
        }
    }
}
