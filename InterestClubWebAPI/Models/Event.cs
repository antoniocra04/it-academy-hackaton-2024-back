namespace InterestClubWebAPI.Models
{
    public class Event
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        //public string? Type { get; set; }

        public DateTime EventDate { get; set; }
        
        public List<User> Participants { get; set; }//Участники ивента



    }
    
}
