namespace InterestClubWebAPI.Models
{
    public class Club
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }        

        public List<Event> Events { get; set; }

        public List<User> Members { get; set; }


    }
}
