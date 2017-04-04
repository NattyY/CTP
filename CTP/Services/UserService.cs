using CTP.Models.Entities;
using CTP.Models.Maps;
using CTP.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CTP.Services
{
    public class UserService : IUserService
    {
        public int GetLoggedInUserId()
        {
            if (!HttpContext.Current.Request.Cookies.AllKeys.Contains("Email")) { return -1; }
            var email = HttpContext.Current.Request.Cookies.Get("Email").Value;
            if (string.IsNullOrEmpty(email)) { return -1; }

            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT Id FROM Users WHERE UPPER(EmailAddress) = '" + email.ToUpperInvariant() + "'";
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return Convert.ToInt32(reader["Id"]);
                        }
                    }
                }
                finally
                {
                    sqlConn.Close();
                }
            }

            throw new Exception("Couldn't get ID for user: " + email);
        }

        public bool IsLoggedIn()
        {
            return HttpContext.Current.Request.Cookies.AllKeys.Contains("Email");
        }

        public bool Login(string emailAddress, string password)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM Users WHERE UPPER(EmailAddress) = '" + emailAddress.ToUpperInvariant() + "' AND Password = '" + password + "'";
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        HttpContext.Current.Response.SetCookie(new HttpCookie("Email", emailAddress)
                        {
                            Expires = DateTime.Now.AddDays(30)
                        });
                        return true;
                    }
                }
                finally
                {
                    sqlConn.Close();
                }
            }

            return false;
        }

        public void Logout()
        {
            // Force cookie to expire (yesterday)
            HttpContext.Current.Response.SetCookie(new HttpCookie("Email", "")
            {
                Expires = DateTime.Now.AddDays(-1)
            });
        }

        public User GetUser(int userId)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM Users WHERE Id = " + userId;
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    if (!reader.Read()) { return null; }

                    return UserMaps.MapDbToEntity(reader);
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public User GetUserByUsername(string username)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM Users WHERE UrlName = '" + username + "'";
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    if (!reader.Read()) { return null; }

                    return UserMaps.MapDbToEntity(reader);
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }
    }
}