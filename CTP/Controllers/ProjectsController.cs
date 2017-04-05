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
    public class ProjectsController : Controller
    {
        // Services used for talking to db
        private IUserService _userService = new UserService();
        private IProjectService _projectService = new ProjectService();
        private IContentService _contentService = new ContentService();

        // On page load, check if a user is logged in and get the user url for the profile link
        public ProjectsController()
        {
            if (_userService.IsLoggedIn())
            {
                var user = _userService.GetUser(_userService.GetLoggedInUserId());
                ViewData["usernameUrl"] = user.UrlName;
            }
        }

        // Project Hub
        public ActionResult Index()
        {
            // Redirect to the homepage if not logged in
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            // Get the projects for the logged in user
            var projects = _projectService.GetProjects(_userService.GetLoggedInUserId());

            // Create the model to pass in to the view
            var model = new ProjectHubViewModel
            {
                // Map the project to a model that contains the fields for a 'card' version of the project
                Projects = projects.Select(ProjectMaps.MapToCard).ToList()
            };

            return View(model);
        }

        // Categories listing
        public ActionResult Categories(string projectName)
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            // Get project by logged in user and projectName (from the URL), then categories in the project
            var project = _projectService.GetUsersProjectByUrlName(_userService.GetLoggedInUserId(), projectName);
            var categories = _projectService.GetCategories(project.Id);

            var model = new ProjectViewModel
            {
                Project = ProjectMaps.MapToCard(project),
                Categories = categories.Select(ProjectCategoryMaps.MapToCard).ToList()
            };

            return View(model);
        }

        // Display the contents of a category
        public ActionResult Category(string projectName, string categoryName)
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            // Get the project by user and URL part, then category by project ID and URL part, then get the content items in that category
            var project = _projectService.GetUsersProjectByUrlName(_userService.GetLoggedInUserId(), projectName);
            var categories = _projectService.GetCategories(project.Id);
            var category = categories.First(c => c.UrlName == categoryName);
            var contentItems = _contentService.GetContentItems(category.Id);

            var model = new ProjectCategoryViewModel
            {
                ContentItems = contentItems.Select(ContentItemMaps.MapToCard).ToList(),
                Category = category
            };

            return View(model);
        }

        // Display the contents of a content item
        public ActionResult ContentItem(string projectName, string categoryName, string contentNames)
        {
            if (string.IsNullOrWhiteSpace(contentNames)) { contentNames = string.Empty; }

            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            var project = _projectService.GetUsersProjectByUrlName(_userService.GetLoggedInUserId(), projectName);
            var categories = _projectService.GetCategories(project.Id);
            var category = categories.First(c => c.UrlName == categoryName);

            long? parentContentItemId = null;
            var ancestors = new List<ContentItem>();

            // Loop through the list of content names from the URL and find them all
            foreach (var name in contentNames.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var item = _contentService.GetContentItemByUrlName(category.Id, name, parentContentItemId);
                ancestors.Add(item);
                parentContentItemId = item.Id;
            }

            // The last content item is the one we are currently 'in'
            var currentContentItem = ancestors.Last();
            ancestors.Remove(currentContentItem);

            // Map the current content item to a page model so it can be displayed in the page
            var model = ContentItemMaps.MapToPageModel(currentContentItem, false);

            // Map the others to card models so we can use them in the breadcrumbs
            model.Ancestors = ancestors.Select(ContentItemMaps.MapToCard).ToList();

            return View(model);
        }

        // Delete a content item by id
        public ActionResult DeleteContentItem(long id)
        {
            var contentItem = _contentService.GetContentItem(id);

            var url = "/projects";

            // Can't do anything if the content item doesn't exist 
            if (contentItem != null)
            {
                // Check if the content item is allowed to be deleted by this user
                if (contentItem.Category.Project.UserId == _userService.GetLoggedInUserId())
                {
                    _contentService.Delete(id);
                }

                // Redirect to the parent content item or parent category
                url = contentItem.ParentContentItemId.HasValue ? ContentItemMaps.GetUrl(contentItem.ParentContentItem) : ProjectCategoryMaps.GetUrl(contentItem.Category);
            }

            return Redirect(url);
        }

        public ActionResult CreateProjectForm()
        {
            // Restore previous model from invalid form submission
            CreateProjectFormViewModel model = null;
            if (TempData.ContainsKey("ProjectCreationFormModel"))
            {
                model = TempData["ProjectCreationFormModel"] as CreateProjectFormViewModel;
                ModelState.Merge((ModelStateDictionary)TempData["ProjectCreationFormModelState"]);
            }

            if (model == null) { model = new CreateProjectFormViewModel(); }
            return PartialView("Partials/CreateProjectForm", model);
        }

        [HttpPost]
        public ActionResult CreateProject(CreateProjectFormViewModel model)
        {
            var userId = _userService.GetLoggedInUserId();
            var urlName = model.Title.ToUrlName();

            // Check if a project already exists with that name
            if (ModelState.IsValid)
            {
                var existingProject = _projectService.GetUsersProjectByUrlName(userId, urlName);
                if (existingProject != null)
                {
                    ModelState.AddModelError("Title", "The name '" + urlName + "' already exists - please try another Project Title");
                }
            }

            if (!ModelState.IsValid)
            {
                // Store the invalid form submission for when the page is refreshed
                TempData["ProjectCreationFormModel"] = model;
                TempData["ProjectCreationFormModelState"] = ModelState;
            }
            else
            {
                // Insert a new project with the specified values
                _projectService.InsertProject(model.Title, model.Description, model.ImageUrl, userId, urlName);
                
                // Find the newly created project
                var project = _projectService.GetUsersProjectByUrlName(userId, urlName);
                
                // Create some default categories in the new project
                _projectService.InsertCategory("World", "", project.Id, "world");
                _projectService.InsertCategory("Characters", "", project.Id, "characters");
                _projectService.InsertCategory("Timeline", "", project.Id, "timeline");
                _projectService.InsertCategory("Plot", "", project.Id, "plot");
            }

            return RedirectToAction("Index");
        }

        public ActionResult CreateCategoryForm(int projectId)
        {
            // Restore previous model from invalid form submission
            CreateCategoryFormViewModel model = null;
            if (TempData.ContainsKey("CategoryCreationFormModel"))
            {
                model = TempData["CategoryCreationFormModel"] as CreateCategoryFormViewModel;
                ModelState.Merge((ModelStateDictionary)TempData["CategoryCreationFormModelState"]);
            }

            if (model == null)
            {
                model = new CreateCategoryFormViewModel()
                {
                    ProjectId = projectId
                };
            }
            return PartialView("Partials/CreateCategoryForm", model);
        }

        [HttpPost]
        public ActionResult CreateCategory(CreateCategoryFormViewModel model)
        {
            var urlName = model.Title.ToUrlName();
            // Find the project to insert the category into
            var project = _projectService.GetProject(model.ProjectId);

            // Check if a category already exists in this project with that name
            if (ModelState.IsValid)
            {
                var existingCategory = _projectService.GetCategories(project.Id).FirstOrDefault(c => c.UrlName == urlName);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("Title", "The name '" + urlName + "' already exists - please try another Category Title");
                }
            }

            if (!ModelState.IsValid)
            {
                // Store the invalid form submission for when the page is refreshed
                TempData["CategoryCreationFormModel"] = model;
                TempData["CategoryCreationFormModelState"] = ModelState;
            }
            else
            {
                // Create the category
                _projectService.InsertCategory(model.Title, model.ImageUrl, model.ProjectId, urlName);
            }

            var url = ProjectMaps.GetUrl(project);
            return Redirect(url);
        }

        public ActionResult CreateContentItemForm(long categoryId, long? parentContentItemId = null)
        {
            // Restore previous model from invalid form submission
            CreateContentItemFormViewModel model = null;
            if (TempData.ContainsKey("ContentItemCreationFormModel"))
            {
                model = TempData["ContentItemCreationFormModel"] as CreateContentItemFormViewModel;
                ModelState.Merge((ModelStateDictionary)TempData["ContentItemCreationFormModelState"]);
            }

            if (model == null)
            {
                model = new CreateContentItemFormViewModel()
                {
                    CategoryId = categoryId,
                    ParentContentItemId = parentContentItemId
                };
            }
            return PartialView("Partials/CreateContentItemForm", model);
        }

        [HttpPost]
        public ActionResult CreateContentItem(CreateContentItemFormViewModel model)
        {
            var urlName = model.Title.ToUrlName();

            var category = _projectService.GetCategory(model.CategoryId);
            category.Project = _projectService.GetProject(category.ProjectId);
            var parentContentItem = model.ParentContentItemId.HasValue ? _contentService.GetContentItem(model.ParentContentItemId.Value) : null;

            // Check if a content item already exists in this category with that name
            if (ModelState.IsValid)
            {
                var existingContentItem = _contentService.GetContentItemByUrlName(category.Id, urlName, model.ParentContentItemId);
                if (existingContentItem != null)
                {
                    ModelState.AddModelError("Title", "The name '" + urlName + "' already exists - please try another Content Item Title");
                }

                // Check if the 'text' type was selected and the 'text' field wasn't set
                if (model.ContentItemTypeId == 1 && string.IsNullOrWhiteSpace(model.Text))
                {
                    ModelState.AddModelError("Text", "The Text field is required");
                }

                // Check if the 'image' type was selected and the 'imageurl' field wasn't set
                if (model.ContentItemTypeId == 2 && string.IsNullOrWhiteSpace(model.ImageUrl))
                {
                    ModelState.AddModelError("ImageUrl", "The Image URL field is required");
                }
            }

            if (!ModelState.IsValid)
            {
                // Store the invalid form submission for when the page is refreshed
                TempData["ContentItemCreationFormModel"] = model;
                TempData["ContentItemCreationFormModelState"] = ModelState;
            }
            else
            {
                // Create the content item
                _contentService.InsertContentItem(model.Title, model.CategoryId, model.ContentItemTypeId, model.ParentContentItemId, urlName, model.Text, model.ImageUrl, model.VideoUrl);
            }

            // Either redirect back to the parent content item (if one exists), or the parent category
            var url = parentContentItem != null ? ContentItemMaps.GetUrl(parentContentItem) : ProjectCategoryMaps.GetUrl(category);
            return Redirect(url);
        }
    }
}