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
        // Get the id of the logged in user
        public int GetLoggedInUserId()
        {
            // Check the cookies to see if they contain an email address, and if not then return -1 (no user should have that ID)
            if (!HttpContext.Current.Request.Cookies.AllKeys.Contains("Email")) { return -1; }

            // The cookies might have an empty cookie called Email
            var email = HttpContext.Current.Request.Cookies.Get("Email").Value;
            if (string.IsNullOrEmpty(email)) { return -1; }

            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    // Get the user ID by their email address
                    var query = "SELECT Id FROM Users WHERE UPPER(EmailAddress) = '" + email.ToUpperInvariant() + "'";
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // Must be converted to ID
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

        // Returns true if someone is logged in
        public bool IsLoggedIn()
        {
            return HttpContext.Current.Request.Cookies.AllKeys.Contains("Email");
        }

        // Returns true if successfully logged someone in with email and password
        public bool Login(string emailAddress, string password)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    // Check against the db
                    var query = "SELECT * FROM Users WHERE UPPER(EmailAddress) = '" + emailAddress.ToUpperInvariant() + "' AND Password = '" + password + "'";
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    // If there's any results, then we know there was a match so add the email address to cookies to show 'logged in'
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

        // Get a specific user 
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

                    // Return null if no results from db
                    if (!reader.Read()) { return null; }
                    
                    // Map the result to the c# model
                    return UserMaps.MapDbToEntity(reader);
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        // Get a specific user by their username
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

                    // Return null if no results from db
                    if (!reader.Read()) { return null; }
                    
                    // Map the result to the c# model
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