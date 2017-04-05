using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.Entities.ContentItems
{
    // C# version of db table 'FolderContentItem'
    public class FolderContentItem : ContentItem
    {
        public long ContentItemId { get; set; }
    }
}