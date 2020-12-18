using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWall.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TheWall.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context { get; set; }
        private PasswordHasher<User> regHasher = new PasswordHasher<User>();
        private PasswordHasher<LoginUser> logHasher = new PasswordHasher<LoginUser>();

        public HomeController(MyContext context)
        {
            _context = context; 
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User u)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.FirstOrDefault(usr => usr.Email == u.Email) !=null)
                {
                    ModelState.AddModelError("Email", "Email is already in use, try logging in!");
                    return View("Index");
                }
                string hash = regHasher.HashPassword(u, u.Password);
                u.Password = hash;
                _context.Users.Add(u);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("userId", u.UserId);
                return Redirect("/home");
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser lu)
        {
            if(ModelState.IsValid)
            {
                User userInDB = _context.Users.FirstOrDefault(u => u.Email == lu.LoginEmail);
                if(userInDB == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email or Password");
                    return View("Index");
                }
                var result = logHasher.VerifyHashedPassword(lu, userInDB.Password, lu.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Invalid Email or Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("userId", userInDB.UserId);
                return Redirect("/home");
            }
            return View("Index");
        }

        [HttpGet("home")]
        public IActionResult Home()
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserId == (int?)HttpContext.Session.GetInt32("userId")); 
            if(userId == null)
            {
                return Redirect("/");
            }
            List<Message> Messages = _context.Messages
                                    .Include(m => m.Creator)
                                    .Include(m => m.Comments)
                                    .ThenInclude(c => c.Writer)
                                    .OrderByDescending(m => m.CreatedAt)
                                    .ToList();
            ViewBag.Messages = Messages;
            List<Comment> Comments = _context.Comments
                                    .Include(m => m.Maker)
                                    .OrderBy(m => m.CreatedAt)
                                    .ToList();
            ViewBag.Comments = Comments;
            ViewBag.User = userId;
            return View("Home");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        [HttpPost("message")]
        public IActionResult Message(Message newMessage, int messageId)
        {
            //put the user into session/store userid
            int? userId = HttpContext.Session.GetInt32("userId");
            newMessage.UserId = (int)userId;
            _context.Messages.Add(newMessage);
            _context.SaveChanges();
            List<Message> Messages = _context.Messages
                            .Include(m => m.Creator)
                            .Include(m => m.Comments)
                            .ThenInclude(c => c.Writer)
                            .OrderBy(m => m.CreatedAt)
                            .ToList();
            ViewBag.Messages = Messages;
            User userInDB = _context.Users
                            .FirstOrDefault(i => i.UserId == userId);
            ViewBag.User = userInDB;
            return Redirect("/home");
        }

        [HttpGet("/delete/message/{messageId}")]
        public IActionResult DeleteMessage(int messageId)
        {
            Message todelete = _context.Messages
                                .FirstOrDefault(m => m.MessageId == messageId);
            _context.Messages.Remove(todelete);
            //put the user into session/store userid
            int? userId = HttpContext.Session.GetInt32("userId");
            _context.SaveChanges();
            return Redirect("/home");
        }

        [HttpPost("/comment")]
        public IActionResult Comment(Comment newComment, int commentId, int messageId)
        {
            //put the user into session/store userid
            int? userId = HttpContext.Session.GetInt32("userId");
            newComment.UserId = (int)userId;
            _context.Comments.Add(newComment);
            _context.SaveChanges();
            List<Comment> Comments = _context.Comments
                                    .Include(m => m.Maker)
                                    .OrderBy(m => m.CreatedAt)
                                    .ToList();
            ViewBag.Comments = Comments;
            User userInDB = _context.Users
                            .FirstOrDefault(i => i.UserId == userId);
            ViewBag.User = userInDB;
            return Redirect("/home");
        }

        [HttpGet("/delete/comment/{commentId}")]
        public IActionResult DeleteComment(int commentId)
        {
            Comment todelete = _context.Comments
                                .FirstOrDefault(m => m.CommentId == commentId);
            _context.Comments.Remove(todelete);
            //put the user into session/store userid
            int? userId = HttpContext.Session.GetInt32("userId");
            _context.SaveChanges();
            return Redirect("/home");
        }

    }
}
