using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class CreateWritingFormViewModel
    {
        // Ensure that the title field must be filled out
        [Required]
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPublic { get; set; }
        public int ProjectId { get; set; }

        public List<Project> Projects { get; set; }
    }
}