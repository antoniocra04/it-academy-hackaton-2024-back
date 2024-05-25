﻿using InterestClubWebAPI.Models;

namespace InterestClubWebAPI.Models
{
    public class Event
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public Guid CreatorEventID { get; set; }

        public Guid ClubID { get; set; } 

        public string EventDate { get; set; }

        public List<User> Members { get; set; }

        public Event()
        {
            Members = new List<User>();
        }
    }

    namespace InterestClubWebAPI.DTOs
    {
        public class EventDTO
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public Guid CreatorEventID { get; set; }
            public Guid ClubID { get; set; }
            public string EventDate { get; set; }
            public List<UserDTO> Members { get; set; }

            public EventDTO()
            {
                Members = new List<UserDTO>();
            }
        }
    }
}
