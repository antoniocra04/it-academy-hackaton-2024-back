﻿using InterestClubWebAPI.Models;


namespace InterestClubWebAPI.Repository
{
    public interface IJWTAuthManager
    {
        Response<string> GenerateJWT(User user);

    }
}
