﻿using InterestClubWebAPI.Context;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models;
using InterestClubWebAPI.Models.DTOs;
using InterestClubWebAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.IO;
using System.Threading;

namespace InterestClubWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly IJWTAuthManager _authentication;
        IWebHostEnvironment _appEnvironment;
        public ClubController(IJWTAuthManager authentication, ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            _authentication = authentication;
            _db = context;
            _appEnvironment = appEnvironment;
        }
        [Authorize]
        [HttpPost("CreateClub")]
        public IActionResult CreateClub(string title, string description, string fullDescription, string userId)
        {
            User? user = _db.Users.FirstOrDefault(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (_db.Clubs.Any(c => c.Title == title))
            {
                return BadRequest("Club with the same title already exists.");
            }
            Club club = new Club { Title = title, Description = description, FullDescription = fullDescription, CreatorClubID = user.Id };
            club.CreatorClubID = user.Id;
            club.Users.Add(user);
            _db.Clubs.Add(club);
            _db.SaveChanges();
            var clubDTO = club.ToDTO();
            return Ok(clubDTO);
        }
        [Authorize]
        [HttpDelete("DeleteClub")]
        public IActionResult DeleteClub(string title)
        {
            Club? club = _db.Clubs.FirstOrDefault(club => club.Title == title);
            if (club != null)
            {
                // Путь к папке клуба
                string clubDirectoryPath = Path.Combine(_appEnvironment.ContentRootPath, "Images", club.Title);

                // Проверка, существует ли папка
                if (Directory.Exists(clubDirectoryPath))
                {
                    // Удаление папки и ее содержимого
                    Directory.Delete(clubDirectoryPath, true);
                }

                // Удаление записи клуба из базы данных
                _db.Clubs.Remove(club);
                _db.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("Нет клуба с таким названием");
            }
        }


        [AllowAnonymous]
        [HttpGet("GetClub")]
        public IActionResult GetClub(string id)
        {

            Club? club = _db.Clubs.Include(c => c.Users).Include(c => c.Events).FirstOrDefault(club => club.Id.ToString() == id);
            if (club == null)
            {
                return BadRequest("Такого Клуба нет :(");
            }
            else
            {
                var clubDTO = club.ToDTO();
                return Ok(clubDTO);
            }
        }
        [AllowAnonymous]
        [HttpGet("GetAllClubs")]
        public IActionResult GetAllClubs()
        {
            var clubs = _db.Clubs.Include(c => c.Users).Include(c => c.Events).ToList();

            if (clubs.Any())
            {
                List<ClubDTO> clubDTOs = new List<ClubDTO>();

                foreach (var club in clubs)
                {
                    var clubDTO = club.ToDTO();
                    clubDTOs.Add(clubDTO);
                }

                return Ok(clubDTOs);
            }
            else
            {
                return BadRequest("Такого Клуба нет :(");
            }
        }


        [Authorize]
        [HttpPost("AddImageInClub")]
        public async Task<IActionResult> AddImageInClub(string ClubId)
        {
            var uploadedFile = Request.Form.Files.FirstOrDefault();
            Club? club = _db.Clubs.FirstOrDefault(club => club.Id.ToString() == ClubId);
            if (club == null)
            {
                return BadRequest("Такого Клуба нет :(");
            }
            if (uploadedFile != null)
            {
                // Проверка, является ли файл изображением
                var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(uploadedFile.FileName).ToLowerInvariant();
                if (!permittedExtensions.Contains(ext))
                {
                    return BadRequest("Файл не является изображением");
                }

                // Проверка типа содержимого
                var permittedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!permittedContentTypes.Contains(uploadedFile.ContentType))
                {
                    return BadRequest("Файл не является изображением");
                }
                //// путь к папке Files
                //string folderPath = Path.Combine(_appEnvironment.WebRootPath, "Files", club.Title);
                //string filePath = Path.Combine(folderPath, uploadedFile.FileName);
                //// Создание папки, если она не существует
                //if (!Directory.Exists(folderPath))
                //{
                //    Directory.CreateDirectory(folderPath);
                //}

                // Удаление старого изображения, если оно существует
                if (club.ClubImage != null)
                {
                    string oldImagePath = _appEnvironment.ContentRootPath + club.ClubImage.Path;
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    _db.Images.Remove(club.ClubImage);
                }
                // путь к папке Files
                string path = $"/Images/{club.Title}/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.ContentRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                Image image = new Image { ImageName = uploadedFile.FileName, Path = path };
                club.ClubImage = image;
                _db.Images.Add(image);
                _db.SaveChanges();
            }

            return Ok("Изображение успешно добавлено");
        }
    }
}
