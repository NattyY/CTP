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
        // Services used for talking to db
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

        // Get the list of writing items inside of a project
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

        // The page to view a logged in users writing item
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

        // Edit a writing item
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

        // Create a new writing page (including an optional project by default)
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

        // Returns the partial view for the create writing form
        // Project ID is optional but can't set int to null, so set to -1 instead (no project should ever have an ID of -1)
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

        // Returns the partial view for the project drawer
        public ActionResult ProjectDrawer(int projectId)
        {
            var project = _projectService.GetProject(projectId);
            var categories = _projectService.GetCategories(projectId);

            // Create a model containing all of the categories in the project
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
                        // Get all of the content items in this category
                        ContentItems = _contentService.GetContentItems(c.Id)
                                                      .Select(ci => ContentItemMaps.MapToPageModel(ci, true))
                                                      .ToList()
                    }).ToList()
                }
            };

            return PartialView("Partials/ProjectDrawer", model);
        }

        // The writing page used when viewing a 'public' writing
        public ActionResult ViewPublicWriting(string username, string projectName, string writingName)
        {
            var user = _userService.GetUserByUsername(username);
            var project = _projectService.GetUsersProjectByUrlName(user.Id, projectName);
            var model = _writingService.GetWritingByProjectId(project.Id).FirstOrDefault(w => w.UrlName == writingName);

            // If the writing item doesn't exist, or if it's not public and the currently logged in user isn't the owner of the writing
            // then we shouldn't allow access to the story
            if (model == null || (!model.IsPublic && _userService.GetLoggedInUserId() != user.Id))
            {
                return Redirect("/");
            }

            return View(model);
        }

        // The form submission for creating a new writing
        // Validate input allows HTML to be POST'ed from the form: http://stackoverflow.com/a/4759693
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
                    
                    // Get the newly created project so we can redirect to it
                    project = _projectService.GetUsersProjectByUrlName(_userService.GetLoggedInUserId(), urlName);
                }
                else
                {
                    // Get the project to redirect to
                    project = _projectService.GetProject(model.ProjectId);
                }

                // Save the writing to the database
                _writingService.InsertWriting(model.Title, model.Content, model.IsPublic, project.Id, urlName);
                
                // Create the url to redirect to
                url = "/writing/" + project.UrlName;
            }

            return Redirect(url);
        }

        // The form submission for editing a writing
        [HttpPost, ValidateInput(false)]
        public ActionResult EditWriting(EditWritingFormViewModel model)
        {
            var urlName = model.Title.ToUrlName();
            // Check if a writing already exists with that name
            if (ModelState.IsValid)
            {

            }

            // Get the url to redirect to 
            var writing = _writingService.GetWriting(model.Id);
            string url = "/writing/" + writing.Project.UrlName + "/" + writing.UrlName;

            if (!ModelState.IsValid)
            {
                // Store the invalid form submission for when the page is refreshed
                TempData["WritingEditFormModel"] = model;
                TempData["WritingEditFormModelState"] = ModelState;

                // If the form submission is invalid, then go back to the edit page
                url += "/edit";
            }
            else
            {
                // Save the writing to the database
                _writingService.UpdateWriting(model.Id, model.Title, model.Content, model.IsPublic, DateTime.Now);
            }

            return Redirect(url);
        }
    }
}