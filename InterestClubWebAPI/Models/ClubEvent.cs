namespace InterestClubWebAPI.Models
{
    public class ClubEvent
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public Guid ClubId { get; set; }
        public Club Club { get; set; }
    }
}
