using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Models.Entities
{
    public abstract class ContentItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long CategoryId { get; set; }
        public short ContentItemTypeId { get; set; }
        public long? ParentContentItemId { get; set; }
        public string UrlName { get; set; }

        public ProjectCategory Category { get; set; }
        public ContentItem ParentContentItem { get; set; }
    }
}