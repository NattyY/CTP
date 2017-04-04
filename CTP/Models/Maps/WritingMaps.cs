using CTP.Helpers;
using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CTP.Models.Maps
{
    public static class WritingMaps
    {
        public static Writing MapDbToEntity(SqlDataReader reader)
        {
            var id = Convert.ToInt32(reader["Id"]);
            var title = reader["Title"].ToString();
            var content = reader["Content"].ToString();
            var isPublic = reader["IsPublic"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsPublic"]);
            var urlName = reader["UrlName"].ToString();
            var dateCreated = Convert.ToDateTime(reader["DateCreated"]);
            var lastModified = Convert.ToDateTime(reader["LastModified"]);
            var projectId = Convert.ToInt32(reader["ProjectId"]);

            return new Writing
            {
                Id = id,
                Title = title,
                Content = content,
                IsPublic = isPublic,
                UrlName = urlName,
                DateCreated = dateCreated,
                LastModified = lastModified,
                ProjectId = projectId
            };
        }

        public static string GetPublicUrl(Writing writing)
        {
            return "/profile/" + writing.Project.User.Username.ToUrlName() + "/" + writing.Project.UrlName + "/" + writing.UrlName;
        }
    }
}