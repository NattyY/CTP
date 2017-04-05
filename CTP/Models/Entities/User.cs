using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.Entities
{
    // C# version of db table 'User'
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string UrlName { get; set; }
    }
}