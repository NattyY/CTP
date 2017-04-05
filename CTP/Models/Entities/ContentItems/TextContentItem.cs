using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.Entities.ContentItems
{
    // C# version of db table 'TextContentItem'
    public class TextContentItem : ContentItem
    {
        public long ContentItemId { get; set; }
        public string Text { get; set; }
    }
}