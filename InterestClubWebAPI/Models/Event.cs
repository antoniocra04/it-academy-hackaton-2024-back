using InterestClubWebAPI.Models;

namespace InterestClubWebAPI.Models
{
    public class Event
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        //public string? Type { get; set; }

        public string EventDate { get; set; }

        //public virtual ICollection<EventMember> EventMembers { get; set; }
        

        //public Event()
        //{
        //    EventMembers = new List<EventMember>();            
        //}        
    }  
}
