﻿using InterestClubWebAPI.Models.InterestClubWebAPI.DTOs;
using InterestClubWebAPI.Models;


namespace InterestClubWebAPI.Extensions
{
    public static class ClubExtensions
    {
        public static ClubDTO ToDTO(this Club club)
        {
            return new ClubDTO
            {
                Id = club.Id,
                Title = club.Title,
                Description = club.Description,
                FullDescription = club.FullDescription,
                CreatorClubID = club.CreatorClubID,
                Users = club.Users.Select(u => u.ToDTO()).ToList(),
                Events = club.Events.Select(e => e.ToDTO()).ToList(),
                CountMembers = club.Users.Count()
            };
        }
    }

    public static class UserExtensions
    {
        public static UserDTO ToDTO(this User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Login = user.Login,                
                Fatherland = user.Fatherland,
                Role = user.Role                
            };
        }
    }

    public static class EventExtensions
    {
        public static EventDTO ToDTO(this Event ev)
        {
            return new EventDTO
            {
                Id = ev.Id,
                Name = ev.Name,
                Description = ev.Description,
                CreatorEventID = ev.CreatorEventID,
                ClubID = ev.ClubID,
                EventDate = ev.EventDate,
                Members = ev.Members.Select(m => m.ToDTO()).ToList()
            };
        }
    }
}

