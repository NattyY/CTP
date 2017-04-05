using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.Entities
{
    // C# version of db table 'ProjectCategory'
    public class ProjectCategory
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public int ProjectId { get; set; }
        public string UrlName { get; set; }

        public Project Project { get; set; }
    }
}