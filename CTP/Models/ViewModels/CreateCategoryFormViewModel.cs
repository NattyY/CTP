using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class CreateCategoryFormViewModel
    {
        // Ensure that the title field must be filled out
        [Required(ErrorMessage = "Please input a Title for the category")]
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public int ProjectId { get; set; }
    }
}