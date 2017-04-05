using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.Entities.ContentItems
{
    // C# version of db table 'ImageContentItem'
    public class ImageContentItem : ContentItem
    {
        public long ContentItemId { get; set; }
        public string ImageUrl { get; set; }
    }
}