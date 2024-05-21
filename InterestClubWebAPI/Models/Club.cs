namespace InterestClubWebAPI.Models
{
    public class Club
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }        

        public virtual List<Event> Events { get; set; }

        public virtual List<User> Members { get; set; }


    }
}
