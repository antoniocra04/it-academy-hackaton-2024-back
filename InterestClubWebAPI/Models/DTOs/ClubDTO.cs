namespace InterestClubWebAPI.Models.DTOs
{
    public class ClubDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? FullDescription { get; set; }
        public Guid CreatorClubID { get; set; }
        public int CountMembers { get; set; }
        //public List<UserDTO> Users { get; set; }
        public string? ImagePath { get; set; }
        public List<EventDTO> Events { get; set; }

        public List<Discussion> Discussions { get; set; }

        public ClubDTO()
        {
            //Users = new List<UserDTO>();
            Events = new List<EventDTO>();
            Discussions = new List<Discussion>();
        }
    }
}
