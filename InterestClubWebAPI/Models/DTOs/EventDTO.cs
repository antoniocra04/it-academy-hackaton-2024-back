namespace InterestClubWebAPI.Models.DTOs
{
    public class EventDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? FullDescription { get; set; }
        public Guid CreatorEventID { get; set; }
        public Guid ClubID { get; set; }
        public string EventDate { get; set; }

        public string? ImagePath { get; set; }
        public int MembersCount { get; set; }

        //public List<UserDTO> Members { get; set; }

        public EventDTO()
        {
            //Members = new List<UserDTO>();
        }
    }
}
