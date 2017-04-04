using CTP.Helpers;
using CTP.Models.Entities;
using CTP.Models.ViewModels;
using CTP.Services;
using CTP.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CTP.Models.Maps
{
    public static class ProjectMaps
    {
        public static Project MapDbToEntity(SqlDataReader reader)
        {
            var id = Convert.ToInt32(reader["Id"]);
            var title = reader["Title"].ToString();
            var description = reader["Description"].ToString();
            var imageUrl = reader["ImageUrl"].ToString();
            var userID = Convert.ToInt32(reader["UserId"]);
            var urlName = reader["UrlName"].ToString();

            return new Project
            {
                Id = id,
                Title = title,
                Description = description,
                ImageUrl = imageUrl,
                UserId = userID,
                UrlName = urlName
            };
        }

        public static ProjectCardViewModel MapToCard(Project project)
        {
            return new ProjectCardViewModel
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                ImageUrl = project.ImageUrl,
                Url = GetUrl(project),
                WritingUrl = "/writing/" + project.UrlName
            };
        }

        public static string GetUrl(Project project)
        {
            return "/projects/" + project.UrlName;
        }
    }
}