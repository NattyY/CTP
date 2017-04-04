using CTP.Models.Entities;
using CTP.Models.Entities.ContentItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class ProjectCategoryViewModel
    {
        public List<ContentItemCardViewModel> ContentItems { get; set; }

        public ProjectCategory Category { get; set; }
    }
}