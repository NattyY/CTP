using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class CategoryPdViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string UrlName { get; set; }

        public List<ContentItemPageViewModel> ContentItems { get; set; }
    }
}