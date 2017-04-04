using CTP.Models.ViewModels;
using CTP.Services;
using CTP.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTP.Controllers
{
    public class HomeController : Controller
    {
        private IUserService _userService = new UserService();
        private IWritingService _writingService = new WritingService();

        public HomeController()
        {
            if (_userService.IsLoggedIn())
            {
                var user = _userService.GetUser(_userService.GetLoggedInUserId());
                ViewData["usernameUrl"] = user.UrlName;
            }
        }

        // GET: Home
        public ActionResult Index(UserLoginViewModel loginModel = null)
        {
            var isLoggedIn = _userService.IsLoggedIn();
            var publicWriting = _writingService.GetPublicWriting();

            var model = new HomeViewModel
            {
                IsLoggedIn = isLoggedIn,
                LoggedInUser = isLoggedIn ? _userService.GetUser(_userService.GetLoggedInUserId()) : null,
                PublicWriting = publicWriting.OrderByDescending(w => w.LastModified).Take(3).ToList()
            };
            return View(model);
        }

        public ActionResult Profile(string username)
        {
            var user = _userService.GetUserByUsername(username);
            var loggedIn = _userService.GetLoggedInUserId();
            var model = new ViewProfileViewModel
            {
                User = user,
                LoggedInUserId = loggedIn,
                PublicWriting = _writingService.GetPublicWriting().Where(p => p.Project.UserId == user.Id).OrderByDescending(w => w.LastModified).Take(3).ToList()
            };
            return View(model);
        }

        public ActionResult LoginForm()
        {
            // Restore previous model from invalid form submission
            UserLoginViewModel model = null;
            if (TempData.ContainsKey("UserLoginModel"))
            {
                model = TempData["UserLoginModel"] as UserLoginViewModel;
                ModelState.Merge((ModelStateDictionary)TempData["UserLoginModelState"]);
            }

            if (model == null) { model = new UserLoginViewModel(); }
            return View("Partials/Login");
        }

        [HttpPost]
        public ActionResult Login(UserLoginViewModel model)
        {
            if (ModelState.IsValid && !_userService.Login(model.EmailAddress, model.Password))
            {
                ModelState.AddModelError("Password", "The username or password is incorrect");
            }

            if (!ModelState.IsValid)
            {
                // Store the invalid form submission for when the page is refreshed
                TempData["UserLoginModel"] = model;
                TempData["UserLoginModelState"] = ModelState;
                return Redirect("Index");
            }

            return RedirectToAction("Index", "Projects");
        }

        public ActionResult Logout()
        {
            _userService.Logout();
            return Redirect("/");
        }
    }
}