﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class ProjectViewModel
    {
        public ProjectCardViewModel Project { get; set; }
        public List<ProjectCategoryCardViewModel> Categories { get; set; }
    }
}