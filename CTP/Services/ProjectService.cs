using CTP.Models.Entities;
using CTP.Models.Maps;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CTP.Services.Abstract
{
    public class ProjectService : IProjectService
    {
        private static IUserService _userService = new UserService();

        public IEnumerable<Project> GetProjects(int userId)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM Projects WHERE UserId = " + userId;
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();
                    var output = new List<Project>();

                    while (reader.Read())
                    {
                        output.Add(ProjectMaps.MapDbToEntity(reader));
                    }

                    return output;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public void InsertProject(string title, string description, string imageUrl, int userId, string urlName)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "INSERT INTO Projects (Title, Description, ImageUrl, UserId, UrlName) VALUES('" + title.Replace("'", "''") + "', '" + (string.IsNullOrWhiteSpace(description) ? "" : description.Replace("'", "''")) + "', '" + imageUrl + "', " + userId + ", '" + urlName + "');";
                    var command = new SqlCommand(query, sqlConn);
                    command.ExecuteNonQuery();
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public Project GetProject(int projectId)
        {
            Project output;
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM Projects WHERE Id = " + projectId;
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    if (!reader.Read()) { return null; }

                    output = ProjectMaps.MapDbToEntity(reader);
                }
                finally
                {
                    sqlConn.Close();
                }
            }

            output.User = _userService.GetUser(output.UserId);
            return output;
        }

        public IEnumerable<ProjectCategory> GetCategories(int projectId)
        {
            var output = new List<ProjectCategory>();

            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM ProjectCategories WHERE ProjectId = " + projectId;
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        output.Add(ProjectCategoryMaps.MapDbToEntity(reader));
                    }

                }
                finally
                {
                    sqlConn.Close();
                }
            }

            foreach (var category in output)
            {
                category.Project = GetProject(category.ProjectId);
            }

            return output;
        }

        public Project GetUsersProjectByUrlName(int userId, string projectUrlName)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM Projects WHERE UserId = " + userId + " AND UrlName = '" + projectUrlName + "'";
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    if (!reader.Read()) { return null; }
                    return ProjectMaps.MapDbToEntity(reader);
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public void InsertCategory(string title, string imageUrl, int projectId, string urlName)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "INSERT INTO ProjectCategories (Title, ImageUrl, ProjectId, UrlName) VALUES('" + title.Replace("'", "''") + "', '" + imageUrl + "', " + projectId + ", '" + urlName + "');";
                    var command = new SqlCommand(query, sqlConn);
                    command.ExecuteNonQuery();
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public ProjectCategory GetCategory(long categoryId)
        {
            ProjectCategory output;
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM ProjectCategories WHERE Id = " + categoryId;
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    if (!reader.Read()) { return null; }

                    output = ProjectCategoryMaps.MapDbToEntity(reader);
                }
                finally
                {
                    sqlConn.Close();
                }
            }

            output.Project = GetProject(output.ProjectId);
            return output;
        }
    }
}