using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.Entities
{
    // C# version of db table 'Writing'
    public class Writing
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPublic { get; set; }
        public string UrlName { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}