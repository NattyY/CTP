using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public abstract class ContentItemPageViewModel
    {
        public ProjectCategory Category { get; set; }
        public List<ContentItemCardViewModel> Ancestors { get; set; }

        public long Id { get; set; }
        public string Title { get; set; }
        public string UrlName { get; set; }
        public string Url { get; set; }
        public abstract string View { get; }
    }

    public class TextContentItemPageViewModel : ContentItemPageViewModel
    {
        public string Text { get; set; }
        public override string View
        {
            get
            {
                return "/Views/ContentItems/Partials/TextPage.cshtml";
            }
        }
    }

    public class ImageContentItemPageViewModel : ContentItemPageViewModel
    {
        public string ImageUrl { get; set; }
        public override string View
        {
            get
            {
                return "/Views/ContentItems/Partials/ImagePage.cshtml";
            }
        }
    }

    public class VideoContentItemPageViewModel : ContentItemPageViewModel
    {
        public string VideoUrl { get; set; }
        public override string View
        {
            get
            {
                return "/Views/ContentItems/Partials/VideoPage.cshtml";
            }
        }
    }

    public class FolderContentItemPageViewModel : ContentItemPageViewModel
    {
        public List<ContentItemCardViewModel> ContentItems { get; set; }

        public override string View
        {
            get
            {
                return "/Views/ContentItems/Partials/FolderPage.cshtml";
            }
        }
    }

    public class FullFolderContentItemPageViewModel : ContentItemPageViewModel
    {
        public List<ContentItemPageViewModel> ContentItems { get; set; }

        public override string View
        {
            get
            {
                return "NOT REQUIRED";
            }
        }
    }
}