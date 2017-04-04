using CTP.Helpers;
using CTP.Models.Entities;
using CTP.Models.Maps;
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
    public class WritingController : Controller
    {
        private IUserService _userService = new UserService();
        private IProjectService _projectService = new ProjectService();
        private IWritingService _writingService = new WritingService();
        private IContentService _contentService = new ContentService();

        public WritingController()
        {
            if (_userService.IsLoggedIn())
            {
                var user = _userService.GetUser(_userService.GetLoggedInUserId());
                ViewData["usernameUrl"] = user.UrlName;
            }
        }

        public ActionResult Index(string projectName)
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            var currentUserId = _userService.GetLoggedInUserId();
            var project = _projectService.GetUsersProjectByUrlName(currentUserId, projectName);
            var writings = _writingService.GetWritingByProjectId(project.Id);
            var model = new WritingDashboardViewModel
            {
                Project = project,
                Writings = writings.ToList()
            };
            return View(model);
        }

        public ActionResult ViewWriting(string projectName, string writingName)
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            var currentUserId = _userService.GetLoggedInUserId();
            var project = _projectService.GetUsersProjectByUrlName(currentUserId, projectName);
            var writing = _writingService.GetWritingByProjectId(project.Id).First(w => w.UrlName == writingName);

            return View(writing);
        }

        public ActionResult Edit(string projectName, string writingName)
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            var currentUserId = _userService.GetLoggedInUserId();
            var project = _projectService.GetUsersProjectByUrlName(currentUserId, projectName);
            var writing = _writingService.GetWritingByProjectId(project.Id).First(w => w.UrlName == writingName);

            var model = new EditWritingFormViewModel
            {
                Id = writing.Id,
                Title = writing.Title,
                Content = writing.Content,
                IsPublic = writing.IsPublic,
                ProjectId = project.Id,
                Project = project,
                Url = "/writing/" + project.UrlName + "/" + writing.UrlName
            };
            return View(model);
        }

        public ActionResult CreateWritingPage(string projectName = "")
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            var currentUserId = _userService.GetLoggedInUserId();
            var model = new CreateWritingPageViewModel();
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                var project = _projectService.GetUsersProjectByUrlName(currentUserId, projectName);
                model.ProjectId = project.Id;
            }

            return View(model);
        }

        public ActionResult CreateWritingForm(int projectId = -1)
        {
            var currentUserId = _userService.GetLoggedInUserId();

            // Restore previous model from invalid form submission
            CreateWritingFormViewModel model = null;
            if (TempData.ContainsKey("WritingCreationFormModel"))
            {
                model = TempData["WritingCreationFormModel"] as CreateWritingFormViewModel;
                ModelState.Merge((ModelStateDictionary)TempData["WritingCreationFormModelState"]);
            }

            if (model == null) { model = new CreateWritingFormViewModel(); }
            var projects = _projectService.GetProjects(currentUserId);
            model.Projects = projects.ToList();
            model.ProjectId = projectId;
            return PartialView("Partials/CreateWriting", model);
        }

        public ActionResult ProjectDrawer(int projectId)
        {
            var project = _projectService.GetProject(projectId);
            var categories = _projectService.GetCategories(projectId);
            var model = new ProjectDrawerViewModel
            {
                Project = new ProjectPdViewModel
                {
                    Id = project.Id,
                    Title = project.Title,

                    Categories = categories.Select(c => new CategoryPdViewModel
                    {
                        Id = c.Id,
                        Title = c.Title,
                        UrlName = c.UrlName,

                        ContentItems = _contentService.GetContentItems(c.Id)
                                                      .Select(ci => ContentItemMaps.MapToPageModel(ci, true))
                                                      .ToList()
                    }).ToList()
                }
            };

            return PartialView("Partials/ProjectDrawer", model);
        }

        public ActionResult ViewPublicWriting(string username, string projectName, string writingName)
        {
            var user = _userService.GetUserByUsername(username);
            var project = _projectService.GetUsersProjectByUrlName(user.Id, projectName);
            var model = _writingService.GetWritingByProjectId(project.Id).FirstOrDefault(w => w.UrlName == writingName);

            if (model == null || (!model.IsPublic && _userService.GetLoggedInUserId() != user.Id))
            {
                return Redirect("/");
            }

            return View(model);
        }

        #region Forms
        [HttpPost, ValidateInput(false)]
        public ActionResult CreateWriting(CreateWritingFormViewModel model)
        {
            var urlName = model.Title.ToUrlName();
            // Check if a writing already exists with that name
            if (ModelState.IsValid)
            {

            }

            string url;
            if (!ModelState.IsValid)
            {
                // Store the invalid form submission for when the page is refreshed
                TempData["WritingCreationFormModel"] = model;
                TempData["WritingCreationFormModelState"] = ModelState;
                url = "/writing/create-new";
            }
            else
            {
                Project project;
                // Create a new project for the writing to go into
                if (model.ProjectId == -1)
                {
                    _projectService.InsertProject(model.Title, model.Title, string.Empty, _userService.GetLoggedInUserId(), urlName);
                    project = _projectService.GetUsersProjectByUrlName(_userService.GetLoggedInUserId(), urlName);
                }
                else
                {
                    project = _projectService.GetProject(model.ProjectId);
                }

                // Save the writing to the database
                _writingService.InsertWriting(model.Title, model.Content, model.IsPublic, project.Id, urlName);
                url = "/writing/" + project.UrlName;
            }

            return Redirect(url);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditWriting(EditWritingFormViewModel model)
        {
            var urlName = model.Title.ToUrlName();
            // Check if a writing already exists with that name
            if (ModelState.IsValid)
            {

            }

            var writing = _writingService.GetWriting(model.Id);
            string url = "/writing/" + writing.Project.UrlName + "/" + writing.UrlName;

            if (!ModelState.IsValid)
            {
                // Store the invalid form submission for when the page is refreshed
                TempData["WritingEditFormModel"] = model;
                TempData["WritingEditFormModelState"] = ModelState;
                url += "/edit";
            }
            else
            {
                // Save the writing to the database
                _writingService.UpdateWriting(model.Id, model.Title, model.Content, model.IsPublic, DateTime.Now);
            }

            return Redirect(url);
        }
        #endregion Forms
    }
}