using CTP.Models.Entities;
using CTP.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CTP.Models.Maps
{
    public static class ProjectCategoryMaps
    {
        public static ProjectCategory MapDbToEntity(SqlDataReader reader)
        {
            // Get fields from the db
            var id = Convert.ToInt64(reader["Id"]);
            var title = reader["Title"].ToString();
            var imageUrl = reader["ImageUrl"].ToString();
            var projectId = Convert.ToInt32(reader["ProjectId"]);
            var urlName = reader["UrlName"].ToString();

            return new ProjectCategory
            {
                Id = id,
                Title = title,
                ImageUrl = imageUrl,
                ProjectId = projectId,
                UrlName = urlName
            };
        }

        public static ProjectCategoryCardViewModel MapToCard(ProjectCategory category)
        {
            // Create the card view model
            return new ProjectCategoryCardViewModel
            {
                Id = category.Id,
                Title = category.Title,
                ImageUrl = category.ImageUrl,
                Url = GetUrl(category)
            };
        }

        // Get the url for the project category by getting the project url and adding the url name afterwards
        public static string GetUrl(ProjectCategory category)
        {
            return ProjectMaps.GetUrl(category.Project) + "/" + category.UrlName;
        } 
    }
}