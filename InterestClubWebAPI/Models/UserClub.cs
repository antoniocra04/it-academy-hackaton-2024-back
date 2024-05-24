namespace InterestClubWebAPI.Models
{
    public class UserClub
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid ClubId { get; set; }
        public Club Club { get; set; }
    }
}
