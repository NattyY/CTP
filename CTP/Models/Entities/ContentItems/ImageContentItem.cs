using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.Entities.ContentItems
{
    public class ImageContentItem : ContentItem
    {
        public long ContentItemId { get; set; }
        public string ImageUrl { get; set; }
    }
}