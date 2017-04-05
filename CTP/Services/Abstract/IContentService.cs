using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Services.Abstract
{
    // Interface allows for the caller to know what functions exist on this service 
    public interface IContentService
    {
        IEnumerable<ContentItem> GetContentItems(long categoryId, long? parentContentItemId = null);
        ContentItem GetContentItem(long id);
        ContentItem GetContentItemByUrlName(long categoryId, string urlName, long? parentContentItemId);
        void InsertContentItem(string title, long categoryId, short contentItemTypeId, long? parentContentItemId, string urlName, string text = null, string imageUrl = null, string videoUrl = null);
        void Delete(long contentItemId);
    }
}