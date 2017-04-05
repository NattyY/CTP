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
        // Services used for talking to db
        private IUserService _userService = new UserService();
        private IWritingService _writingService = new WritingService();

        // On page load, check if a user is logged in and get the user url for the profile link
        public HomeController()
        {
            if (_userService.IsLoggedIn())
            {
                var user = _userService.GetUser(_userService.GetLoggedInUserId());
                ViewData["usernameUrl"] = user.UrlName;
            }
        }

        // Home page
        public ActionResult Index(UserLoginViewModel loginModel = null)
        {
            var isLoggedIn = _userService.IsLoggedIn();
            
            // Get the list of public writing
            var publicWriting = _writingService.GetPublicWriting();

            var model = new HomeViewModel
            {
                IsLoggedIn = isLoggedIn,
                // If the user is logged in then retrieve from the db, otherwise just set to null
                LoggedInUser = isLoggedIn ? _userService.GetUser(_userService.GetLoggedInUserId()) : null,

                // Order the public writing by the last modified date and then take the top 3
                PublicWriting = publicWriting.OrderByDescending(w => w.LastModified).Take(3).ToList()
            };
            return View(model);
        }

        // Profile page
        public ActionResult Profile(string username)
        {
            var user = _userService.GetUserByUsername(username);
            var loggedIn = _userService.GetLoggedInUserId();
            var model = new ViewProfileViewModel
            {
                User = user,
                LoggedInUserId = loggedIn,
                // Get the latest 3 from list of public writing for the correct user id
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

            // If there was no previous form submission then just create a new model
            if (model == null) { model = new UserLoginViewModel(); }
            return View("Partials/Login");
        }

        [HttpPost]
        public ActionResult Login(UserLoginViewModel model)
        {
            // If the form submission is valid, then try to log the user in
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