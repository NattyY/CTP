using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class ProjectCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Url { get; set; }
        public string WritingUrl { get; set; }
    }
}