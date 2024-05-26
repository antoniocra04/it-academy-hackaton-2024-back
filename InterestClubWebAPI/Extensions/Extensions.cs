﻿using InterestClubWebAPI.Models.InterestClubWebAPI.DTOs;
using InterestClubWebAPI.Models;
using System;
using Microsoft.Extensions.Logging;


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
                CountMembers = club.Users.Count(),
                Users = club.Users.Select(u => u.ToDTO()).ToList(),
                Events = club.Events.Select(e => e.ToDTO()).ToList(),                
            };
        }
    }

    public static class UserExtensions
    {
        public static UserDTO ToDTO(this User user)
        {
            return  new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Login = user.Login,
                Password = user.Password,
                Fatherland = user.Fatherland,
                Role = user.Role,
                ClubsId = user.Clubs.Select(c => c.Id.ToString()).ToList(),
                EventsId = user.Events.Select(e => e.Id.ToString()).ToList(),
            };
            
            
            //foreach (var club in user.Clubs)
            //{
            //    userDTO.ClubsId.Add(club.Id.ToString());
            //}
            //foreach (var ev in user.Events)
            //{
            //    userDTO.EventsId.Add(ev.Id.ToString());
            //}
            //return userDTO;
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
                MembersCount = ev.Members.Count(),
                Members = ev.Members.Select(m => m.ToDTO()).ToList(),
                
            };
        }
    }
}
