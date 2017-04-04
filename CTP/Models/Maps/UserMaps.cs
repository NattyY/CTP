using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CTP.Models.Maps
{
    public static class UserMaps
    {
        public static User MapDbToEntity(SqlDataReader reader)
        {
            var id = Convert.ToInt32(reader["Id"]);
            var username = reader["Username"].ToString();
            var email = reader["EmailAddress"].ToString();
            var urlName = reader["UrlName"].ToString();

            return new User
            {
                Id = id,
                EmailAddress = email,
                Username = username,
                UrlName = urlName
            };
        }

    }
}