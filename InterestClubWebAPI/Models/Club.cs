using static InterestClubWebAPI.Models.User;

namespace InterestClubWebAPI.Models
{
    public class Club
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? FullDescription { get; set; }

        public Guid CreatorClubID { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Event> Events { get; set; }

        public Club()
        {
            Events = new List<Event>();
            Users = new List<User>();
        }
    }

    namespace InterestClubWebAPI.DTOs
    {
        public class ClubDTO
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string? Description { get; set; }
            public string? FullDescription { get; set; }
            public Guid CreatorClubID { get; set; }
            public int CountMembers { get; set; }
            public List<UserDTO> Users { get; set; }
            public List<EventDTO> Events { get; set; }           

            public ClubDTO()
            {
                Users = new List<UserDTO>();
                Events = new List<EventDTO>();
            }
        }
    }

}
