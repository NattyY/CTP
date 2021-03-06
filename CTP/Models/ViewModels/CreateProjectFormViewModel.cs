﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class CreateProjectFormViewModel
    {
        // Ensure that the title field must be filled out for a project
        [Required(ErrorMessage = "Please input a Title for the project")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}