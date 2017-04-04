using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class CreateWritingFormViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPublic { get; set; }
        public int ProjectId { get; set; }

        public List<Project> Projects { get; set; }
    }
}