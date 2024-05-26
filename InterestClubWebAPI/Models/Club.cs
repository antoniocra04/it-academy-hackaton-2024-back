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

        public ICollection<Discussion> Discussions { get; set; }

        public Club()
        {
            Events = new List<Event>();
            Users = new List<User>();
            Discussions = new List<Discussion>();
        }
    }

    

}
