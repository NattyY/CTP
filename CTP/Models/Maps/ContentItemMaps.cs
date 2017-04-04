using CTP.Models.Entities;
using CTP.Models.Entities.ContentItems;
using CTP.Models.ViewModels;
using CTP.Services;
using CTP.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CTP.Models.Maps
{
    public static class ContentItemMaps
    {
        private static IContentService _contentService = new ContentService();

        public static ContentItem MapDbToEntity(SqlDataReader reader)
        {
            var id = Convert.ToInt64(reader["Id"]);
            var title = reader["Title"].ToString();
            var categoryId = Convert.ToInt64(reader["CategoryId"]);
            var contentItemTypeId = Convert.ToInt16(reader["ContentItemTypeId"]);
            string parentContentItemIdStr = reader["ParentContentItemId"] == null ? null : reader["ParentContentItemId"].ToString();
            long? parentContentItemId = null;
            if (!string.IsNullOrWhiteSpace(parentContentItemIdStr)) { parentContentItemId = Convert.ToInt64(parentContentItemIdStr); }
            var urlName = reader["UrlName"].ToString();

            switch (contentItemTypeId)
            {
                case 1:
                    // Text content item
                    var text = reader["Text"].ToString();

                    return new TextContentItem
                    {
                        Text = text,
                        Id = id,
                        Title = title,
                        CategoryId = categoryId,
                        ContentItemId = id,
                        ContentItemTypeId = contentItemTypeId,
                        ParentContentItemId = parentContentItemId,
                        UrlName = urlName
                    };
                case 2:
                    // Image content item
                    var imageUrl = reader["ImageUrl"].ToString();

                    return new ImageContentItem
                    {
                        ImageUrl = imageUrl,
                        Id = id,
                        Title = title,
                        CategoryId = categoryId,
                        ContentItemId = id,
                        ContentItemTypeId = contentItemTypeId,
                        ParentContentItemId = parentContentItemId,
                        UrlName = urlName
                    };
                case 3:
                    // Video content item
                    var videoUrl = reader["VideoUrl"].ToString();

                    return new VideoContentItem
                    {
                        VideoUrl = videoUrl,
                        Id = id,
                        Title = title,
                        CategoryId = categoryId,
                        ContentItemId = id,
                        ContentItemTypeId = contentItemTypeId,
                        ParentContentItemId = parentContentItemId,
                        UrlName = urlName
                    };
                case 4:
                    // Folder content item
                    return new FolderContentItem
                    {
                        Id = id,
                        Title = title,
                        CategoryId = categoryId,
                        ContentItemId = id,
                        ContentItemTypeId = contentItemTypeId,
                        ParentContentItemId = parentContentItemId,
                        UrlName = urlName
                    };
                default:
                    throw new NotImplementedException("Didn't recognise content item type id");
            }
        }

        public static ContentItemCardViewModel MapToCard(ContentItem contentItem)
        {
            switch (contentItem.ContentItemTypeId)
            {
                case 1:
                    // Text content item
                    return new TextContentItemCardViewModel
                    {
                        Id = contentItem.Id,
                        Title = contentItem.Title,
                        UrlName = contentItem.UrlName,
                        Url = GetUrl(contentItem),
                        Text = (contentItem as TextContentItem).Text
                    };
                case 2:
                    // Image content item
                    return new ImageContentItemCardViewModel
                    {
                        Id = contentItem.Id,
                        Title = contentItem.Title,
                        UrlName = contentItem.UrlName,
                        Url = GetUrl(contentItem),
                        ImageUrl = (contentItem as ImageContentItem).ImageUrl
                    };
                case 3:
                    // Video content item
                    return new VideoContentItemCardViewModel
                    {
                        Id = contentItem.Id,
                        Title = contentItem.Title,
                        UrlName = contentItem.UrlName,
                        Url = GetUrl(contentItem),
                        VideoUrl = (contentItem as VideoContentItem).VideoUrl
                    };
                case 4:
                    // Folder content item
                    return new FolderContentItemCardViewModel
                    {
                        Id = contentItem.Id,
                        Title = contentItem.Title,
                        UrlName = contentItem.UrlName,
                        Url = GetUrl(contentItem)
                    };
                default:
                    throw new NotImplementedException("Didn't recognise content item type id");
            }
        }

        public static ContentItemPageViewModel MapToPageModel(ContentItem contentItem, bool recursiveFolders)
        {
            switch (contentItem.ContentItemTypeId)
            {
                case 1:
                    // Text content item
                    return new TextContentItemPageViewModel
                    {
                        Id = contentItem.Id,
                        Title = contentItem.Title,
                        UrlName = contentItem.UrlName,
                        Url = GetUrl(contentItem),
                        Text = (contentItem as TextContentItem).Text,
                        Category = contentItem.Category
                    };
                case 2:
                    // Image content item
                    return new ImageContentItemPageViewModel
                    {
                        Id = contentItem.Id,
                        Title = contentItem.Title,
                        UrlName = contentItem.UrlName,
                        Url = GetUrl(contentItem),
                        ImageUrl = (contentItem as ImageContentItem).ImageUrl,
                        Category = contentItem.Category
                    };
                case 3:
                    // Video content item
                    return new VideoContentItemPageViewModel
                    {
                        Id = contentItem.Id,
                        Title = contentItem.Title,
                        UrlName = contentItem.UrlName,
                        Url = GetUrl(contentItem),
                        VideoUrl = (contentItem as VideoContentItem).VideoUrl,
                        Category = contentItem.Category
                    };
                case 4:
                    // Folder content item
                    if (recursiveFolders)
                    {
                        // Retrieve content items for folders within this folder as well
                        return new FullFolderContentItemPageViewModel
                        {
                            Id = contentItem.Id,
                            Title = contentItem.Title,
                            UrlName = contentItem.UrlName,
                            Url = GetUrl(contentItem),
                            Category = contentItem.Category,
                            ContentItems = _contentService.GetContentItems(contentItem.CategoryId, contentItem.Id).Select(ci => MapToPageModel(ci, true)).ToList()
                        };
                    }
                    else
                    {
                        // Only retrieve top level content items in this folder
                        return new FolderContentItemPageViewModel
                        {
                            Id = contentItem.Id,
                            Title = contentItem.Title,
                            UrlName = contentItem.UrlName,
                            Url = GetUrl(contentItem),
                            Category = contentItem.Category,
                            ContentItems = _contentService.GetContentItems(contentItem.CategoryId, contentItem.Id).Select(MapToCard).ToList()
                        };
                    }
                default:
                    throw new NotImplementedException("Didn't recognise content item type id");
            }
        }


        public static string GetUrl(ContentItem contentItem)
        {
            if (contentItem.ParentContentItem == null)
            {
                return ProjectCategoryMaps.GetUrl(contentItem.Category) + "/" + contentItem.UrlName;
            }
            else
            {
                return GetUrl(contentItem.ParentContentItem) + "/" + contentItem.UrlName;
            }
        }
    }
}