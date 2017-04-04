using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public class WritingDashboardViewModel
    {
        public Project Project { get; set; }
        public List<Writing> Writings { get; set; }
    }
}