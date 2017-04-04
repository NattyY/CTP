using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class ProjectPdViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<CategoryPdViewModel> Categories { get; set; }
    }
}