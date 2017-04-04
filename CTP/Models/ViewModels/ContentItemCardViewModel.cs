using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.ViewModels
{
    public abstract class ContentItemCardViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string UrlName { get; set; }
        public string Url { get; set; }
        public abstract string View { get; }
    }

    public class TextContentItemCardViewModel : ContentItemCardViewModel
    {
        public string Text { get; set; }
        public override string View
        {
            get
            {
                return "/Views/ContentItems/Partials/TextCard.cshtml";
            }
        }
    }

    public class ImageContentItemCardViewModel : ContentItemCardViewModel
    {
        public string ImageUrl { get; set; }
        public override string View
        {
            get
            {
                return "/Views/ContentItems/Partials/ImageCard.cshtml";
            }
        }
    }

    public class VideoContentItemCardViewModel : ContentItemCardViewModel
    {
        public string VideoUrl { get; set; }
        public override string View
        {
            get
            {
                return "/Views/ContentItems/Partials/VideoCard.cshtml";
            }
        }
    }

    public class FolderContentItemCardViewModel : ContentItemCardViewModel
    {
        public override string View
        {
            get
            {
                return "/Views/ContentItems/Partials/FolderCard.cshtml";
            }
        }
    }
}