using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.Entities.ContentItems
{
    // C# version of db table 'VideoContentItem'
    public class VideoContentItem : ContentItem
    {
        public long ContentItemId { get; set; }
        public string VideoUrl { get; set; }
    }
}