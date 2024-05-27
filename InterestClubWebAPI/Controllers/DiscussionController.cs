using InterestClubWebAPI.Context;
using InterestClubWebAPI.Models;
using InterestClubWebAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InterestClubWebAPI.Extensions;
using InterestClubWebAPI.Models.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.Data.Entity;

namespace InterestClubWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscussionController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly IJWTAuthManager _authentication;
        public DiscussionController(IJWTAuthManager authentication, ApplicationContext context)
        {
            _authentication = authentication;
            _db = context;
        }
        [Authorize]
        [HttpPost("CreateDiscussion")]
        public IActionResult CreateDiscussion(string clubId, string title, string description, string fullDescription)
        {
            try
            {
                Club? club = _db.Clubs.Include(c => c.Discussions).FirstOrDefault(c => c.Id.ToString() == clubId);
                if (club == null)
                {
                    return BadRequest("Нет такого клуба");
                }
                if (_db.Discussions.Any(d => d.Title == title && d.ClubId.ToString() == clubId))
                {
                    return BadRequest("Обсуждение с таким название уже созданно у этого клуба");
                }
                Discussion discussion = new Discussion { ClubId = Guid.Parse(clubId), CreatorId = club.CreatorClubID, Title = title, Description = description, FullDescription = fullDescription };
                club.Discussions.Add(discussion);
                _db.Discussions.Add(discussion);
                _db.SaveChanges();
                return Ok(discussion.ToDTO());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("GetDiscussionById")]
        public IActionResult GetDiscussionById(string discussionsId)
        {
            try
            {
                List<Comment> comments = _db.Comments.Where(c => c.DiscussionId.ToString() == discussionsId).ToList();
                Discussion? discussion = _db.Discussions.Include(d => d.comments).FirstOrDefault(d => d.Id.ToString() == discussionsId);
                if (discussion == null)
                {
                    return BadRequest("Обсуждение с таким id не сущетвует");
                }
                return Ok(discussion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("GetAllDiscussion")]
        public IActionResult GetAllDiscussion(string clubId)
        {
            try
            {
                var discussion = _db.Discussions.Where(d => d.ClubId.ToString() == clubId);

                if (discussion.Any())
                {
                    List<DiscussionDTO> discussionDTOs = new List<DiscussionDTO>();
                    foreach (var d in discussion)
                    {
                        var discussionDTO = d.ToDTO();
                        discussionDTOs.Add(discussionDTO);
                    }
                    return Ok(discussionDTOs);
                }
                return BadRequest("Обсуждений у клуба таким id нет");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }

        }
        [Authorize]
        [HttpDelete("DeleteDiscussion")]
        public IActionResult DeleteDiscussion(string discussionsId)
        {
            try
            {
                Discussion? discussion = _db.Discussions.FirstOrDefault(d => d.Id.ToString() == discussionsId);
                if (discussion == null)
                {
                    return BadRequest("Обсуждение с таким id не сущетвует");
                }
                _db.Discussions.Remove(discussion);
                _db.SaveChanges();
                return Ok("Обсуждение успешно удалено");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("CreateComment")]
        public IActionResult CreateComment(string discussionsId, string commentariy)
        {
            try
            {
                Discussion? discussion = _db.Discussions.Include(d => d.comments).FirstOrDefault(d => d.Id.ToString() == discussionsId);
                if (discussion == null)
                {
                    return BadRequest("Обсуждение с таким id не сущетвует");
                }
                var token = HttpContext.GetTokenAsync("access_token");
                var userCreditans = _authentication.getUserCreditansFromJWT(token.Result);
                User? user = _db.Users.FirstOrDefault(u => u.Login == userCreditans.login && u.Password == userCreditans.password);
                if (user == null)
                {
                    return BadRequest($"Нет пользователя с логином: {userCreditans.login}.Или неверный пароль");
                }

                Comment comment = new Comment { UserId = user.Id, LogginUser = user.Login, DiscussionId = Guid.Parse(discussionsId), Commentariy = commentariy };
                discussion.comments.Add(comment);
                _db.Comments.Add(comment);
                _db.SaveChanges();
                return Ok("Коментарий успешно добавлен");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("EditDiscussion")]
        public IActionResult EditUser(string discussionId, string title, string description, string fullDescription)
        {
            try
            {
                var token = HttpContext.GetTokenAsync("access_token");
                var userCreditans = _authentication.getUserCreditansFromJWT(token.Result);
                User? user = _db.Users.FirstOrDefault(u => u.Login == userCreditans.login && u.Password == userCreditans.password);
                if (user == null)
                {
                    return BadRequest("Нет такого пользователя от которого идет запрос :(");
                }
                Discussion? discussion = _db.Discussions.FirstOrDefault(d => d.Id.ToString() == discussionId);
                if (discussion == null)
                {
                    return BadRequest("Нет такого Обсуждения  :(");
                }
                if (discussion.CreatorId == user.Id || user.Role == Enums.Role.admin)
                {
                    discussion.Title = title;
                    discussion.Description = description;
                    discussion.FullDescription = fullDescription;
                    _db.SaveChanges();
                    return Ok("Клуб успешно изменен");
                }
                else
                {
                    return BadRequest("Нет прав для изменения клуба :(");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка при выполнении Роута", error = ex.Message });
            }
        }

    }

}
