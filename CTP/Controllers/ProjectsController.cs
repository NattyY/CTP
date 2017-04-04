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
        private IUserService _userService = new UserService();
        private IProjectService _projectService = new ProjectService();
        private IContentService _contentService = new ContentService();

        public ProjectsController()
        {
            if (_userService.IsLoggedIn())
            {
                var user = _userService.GetUser(_userService.GetLoggedInUserId());
                ViewData["usernameUrl"] = user.UrlName;
            }
        }

        #region GET
        public ActionResult Index()
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            var projects = _projectService.GetProjects(_userService.GetLoggedInUserId());
            var model = new ProjectHubViewModel
            {
                Projects = projects.Select(ProjectMaps.MapToCard).ToList()
            };

            return View(model);
        }

        public ActionResult Categories(string projectName)
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            var project = _projectService.GetUsersProjectByUrlName(_userService.GetLoggedInUserId(), projectName);
            var categories = _projectService.GetCategories(project.Id);

            var model = new ProjectViewModel
            {
                Project = ProjectMaps.MapToCard(project),
                Categories = categories.Select(ProjectCategoryMaps.MapToCard).ToList()
            };

            return View(model);
        }

        public ActionResult Category(string projectName, string categoryName)
        {
            if (!_userService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

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
            foreach (var name in contentNames.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var item = _contentService.GetContentItemByUrlName(category.Id, name, parentContentItemId);
                ancestors.Add(item);
                parentContentItemId = item.Id;
            }

            var currentContentItem = ancestors.Last();
            ancestors.Remove(currentContentItem);

            var model = ContentItemMaps.MapToPageModel(currentContentItem, false);
            model.Ancestors = ancestors.Select(ContentItemMaps.MapToCard).ToList();

            return View(model);
        }

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
        #endregion GET

        #region Forms
        #region Create Project
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
                _projectService.InsertProject(model.Title, model.Description, model.ImageUrl, userId, urlName);
                var project = _projectService.GetUsersProjectByUrlName(userId, urlName);
                _projectService.InsertCategory("World", "", project.Id, "world");
                _projectService.InsertCategory("Characters", "", project.Id, "characters");
                _projectService.InsertCategory("Timeline", "", project.Id, "timeline");
                _projectService.InsertCategory("Plot", "", project.Id, "plot");
            }

            return RedirectToAction("Index");
        }
        #endregion Create Project

        #region Create Category
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
                _projectService.InsertCategory(model.Title, model.ImageUrl, model.ProjectId, urlName);
            }

            var url = ProjectMaps.GetUrl(project);
            return Redirect(url);
        }
        #endregion Create Category

        #region Create Content Item
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
            }

            if (!ModelState.IsValid)
            {
                // Store the invalid form submission for when the page is refreshed
                TempData["ContentItemCreationFormModel"] = model;
                TempData["ContentItemCreationFormModelState"] = ModelState;
            }
            else
            {
                _contentService.InsertContentItem(model.Title, model.CategoryId, model.ContentItemTypeId, model.ParentContentItemId, urlName, model.Text, model.ImageUrl, model.VideoUrl);
            }

            var url = parentContentItem != null ? ContentItemMaps.GetUrl(parentContentItem) : ProjectCategoryMaps.GetUrl(category);
            return Redirect(url);
        }
        #endregion Create Content Item
        #endregion Forms
    }
}