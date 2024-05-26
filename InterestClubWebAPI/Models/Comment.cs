namespace InterestClubWebAPI.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid DiscussionId { get; set; }
        public Guid UserId { get; set; }
        public string LogginUser { get; set; }
        public string Commentariy { get; set; }

    }
}
