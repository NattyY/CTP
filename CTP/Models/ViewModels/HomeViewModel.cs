using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class HomeViewModel
    {
        public bool IsLoggedIn { get; set; }
        public User LoggedInUser { get; set; }
        public List<Writing> PublicWriting { get; set; }
    }
}