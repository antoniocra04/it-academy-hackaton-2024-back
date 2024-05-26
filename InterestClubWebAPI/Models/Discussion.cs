namespace InterestClubWebAPI.Models
{
    public class Discussion
    {
        public Guid Id { get; set; }

        public Guid ClubId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string FullDescription { get; set; }

        public ICollection<Comment> comments { get; set; }

        public Discussion() {
            comments = new List<Comment>();
        }

    }


}
