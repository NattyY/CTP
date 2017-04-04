using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class ViewProfileViewModel
    {
        public User User { get; set; }
        public int LoggedInUserId { get; set; }
        public List<Writing> PublicWriting { get; set; }
    }
}