using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Services.Abstract
{
    // Interface allows for the caller to know what functions exist on this service
    public interface IProjectService
    {
        IEnumerable<Project> GetProjects(int userId);

        Project GetProject(int projectId);

        void InsertProject(string title, string description, string imageUrl, int userId, string urlName);
        void InsertCategory(string title, string imageUrl, int projectId, string urlName);

        IEnumerable<ProjectCategory> GetCategories(int projectId);
        Project GetUsersProjectByUrlName(int userId, string projectName);
        ProjectCategory GetCategory(long categoryId);
    }
}