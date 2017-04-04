using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTP.Models.ViewModels
{
    public class CreateContentItemFormViewModel
    {
        [Required(ErrorMessage = "Please input a Title for the content item")]
        public string Title { get; set; }
        public long CategoryId { get; set; }
        public short ContentItemTypeId { get; set; }
        public long? ParentContentItemId { get; set; }

        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }

        public List<SelectListItem> ContentItemTypes = new List<SelectListItem>
        {
             new SelectListItem { Text = "Text", Value = 1.ToString() },
             new SelectListItem { Text = "Image", Value = 2.ToString() },
             //new SelectListItem { Text = "Video", Value = 3.ToString() },
             new SelectListItem { Text = "Folder", Value = 4.ToString() }
        };
    }
}