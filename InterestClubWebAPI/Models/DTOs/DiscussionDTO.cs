namespace InterestClubWebAPI.Models.DTOs
{
    public class DiscussionDTO
    {
        public Guid Id { get; set; }

        public Guid ClubId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string FullDescription { get; set; }
       
    }
}
