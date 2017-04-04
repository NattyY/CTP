using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Services.Abstract
{
    public interface IUserService
    {
        bool IsLoggedIn();
        int GetLoggedInUserId();
        bool Login(string emailAddress, string password);
        User GetUser(int id);
        User GetUserByUsername(string username);
        void Logout();
    }
}