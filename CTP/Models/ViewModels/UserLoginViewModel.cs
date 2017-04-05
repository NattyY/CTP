using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class UserLoginViewModel
    {
        // Ensure that an email must be filled out
        [Required]
        public string EmailAddress { get; set; }
        
        // Ensure that a password must be filled out
        [Required]
        public string Password { get; set; }
    }
}