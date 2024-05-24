using static InterestClubWebAPI.Models.User;

namespace InterestClubWebAPI.Models
{
    public class Club
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<ClubEvent> ClubEvents { get; set; }
        
        
        public Club()
        {
            ClubEvents = new List<ClubEvent>();            
        }
    }
    
}
