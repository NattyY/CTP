using CTP.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CTP.Models.Entities;
using System.Web.Configuration;
using System.Data.SqlClient;
using CTP.Models.Maps;

namespace CTP.Services
{
    // Inherits from icontentservice so must implement functions from it
    public class ContentService : IContentService
    {
        private IProjectService _projectService = new ProjectService();

        // Get all content items for a given category and optional parent content item 
        public IEnumerable<ContentItem> GetContentItems(long categoryId, long? parentContentItemId = null)
        {
            var output = new List<ContentItem>();

            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    // Do a left join on content item tables to find matching rows
                    var query = "SELECT * FROM ContentItems "
                              + "LEFT JOIN TextContentItems ON ContentItems.Id = TextContentItems.ContentItemId "
                              + "LEFT JOIN ImageContentItems ON ContentItems.Id = ImageContentItems.ContentItemId "
                              + "LEFT JOIN VideoContentItems ON ContentItems.Id = VideoContentItems.ContentItemId "
                              + "LEFT JOIN FolderContentItems ON ContentItems.Id = FolderContentItems.ContentItemId "
                              + "WHERE CategoryId = " + categoryId
                              // Search by the parent content item id if it has been included
                              + (parentContentItemId.HasValue ? " AND ParentContentItemId = " + parentContentItemId.Value.ToString() : " AND ParentContentItemId IS NULL");

                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    // Map the results to a c# model 
                    while (reader.Read())
                    {
                        output.Add(ContentItemMaps.MapDbToEntity(reader));
                    }

                }
                finally
                {
                    sqlConn.Close();
                }
            }

            // Loop through the results and get the category and parent content items
            foreach (var contentItem in output)
            {
                contentItem.Category = _projectService.GetCategory(contentItem.CategoryId);
                if (contentItem.ParentContentItemId.HasValue) { contentItem.ParentContentItem = GetContentItem(contentItem.ParentContentItemId.Value); }
            }

            return output;
        }

        // Get a specific content item by its id
        public ContentItem GetContentItem(long id)
        {
            ContentItem output;

            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM ContentItems "
                              + "LEFT JOIN TextContentItems ON ContentItems.Id = TextContentItems.ContentItemId "
                              + "LEFT JOIN ImageContentItems ON ContentItems.Id = ImageContentItems.ContentItemId "
                              + "LEFT JOIN VideoContentItems ON ContentItems.Id = VideoContentItems.ContentItemId "
                              + "LEFT JOIN FolderContentItems ON ContentItems.Id = FolderContentItems.ContentItemId "
                              + "WHERE Id = " + id;
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    // Return null if no results
                    if (!reader.Read()) { return null; }

                    // Map to a c# model
                    output = ContentItemMaps.MapDbToEntity(reader);
                }
                finally
                {
                    sqlConn.Close();
                }
            }

            // Get the category and parent content item from db
            output.Category = _projectService.GetCategory(output.CategoryId);
            if (output.ParentContentItemId.HasValue) { output.ParentContentItem = GetContentItem(output.ParentContentItemId.Value); }

            return output;
        }

        // Find a content item by its url name
        public ContentItem GetContentItemByUrlName(long categoryId, string urlName, long? parentContentItemId)
        {
            ContentItem output;

            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM ContentItems "
                              + "LEFT JOIN TextContentItems ON ContentItems.Id = TextContentItems.ContentItemId "
                              + "LEFT JOIN ImageContentItems ON ContentItems.Id = ImageContentItems.ContentItemId "
                              + "LEFT JOIN VideoContentItems ON ContentItems.Id = VideoContentItems.ContentItemId "
                              + "LEFT JOIN FolderContentItems ON ContentItems.Id = FolderContentItems.ContentItemId "
                              + "WHERE CategoryId = " + categoryId
                              + " AND UrlName = '" + urlName + "'"
                              + (parentContentItemId.HasValue ? " AND ParentContentItemId = " + parentContentItemId.Value.ToString() : " AND ParentContentItemId IS NULL");
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    if (!reader.Read()) { return null; }

                    output = ContentItemMaps.MapDbToEntity(reader);
                }
                finally
                {
                    sqlConn.Close();
                }
            }

            output.Category = _projectService.GetCategory(output.CategoryId);
            if (output.ParentContentItemId.HasValue) { output.ParentContentItem = GetContentItem(output.ParentContentItemId.Value); }

            return output;
        }

        // Create a new content item 
        public void InsertContentItem(string title, long categoryId, short contentItemTypeId, long? parentContentItemId, string urlName,
                                      string text = null,
                                      string imageUrl = null,
                                      string videoUrl = null)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    // Insert into the base table and get the ID of the newly inserted row
                    var query = "INSERT INTO ContentItems (Title, CategoryId, ContentItemTypeId, ParentContentItemId, UrlName) OUTPUT INSERTED.ID VALUES('" + title.Replace("'", "''") + "', " + categoryId + ", " + contentItemTypeId + ", " + (parentContentItemId.HasValue ? parentContentItemId.Value.ToString() : "NULL") + ", '" + urlName + "');";
                    var command = new SqlCommand(query, sqlConn);
                    var id = command.ExecuteScalar();

                    string specificQuery;
                    SqlCommand specificCommand;
                    // Insert into the correct table based on the type id, and include the newly inserted id as a reference from the base table
                    switch (contentItemTypeId)
                    {
                        case 1:
                            specificQuery = "INSERT INTO TextContentItems (ContentItemId, Text) VALUES(" + id + ", '" + text.Replace("'", "''") + "')";
                            specificCommand = new SqlCommand(specificQuery, sqlConn);
                            specificCommand.ExecuteNonQuery();
                            break;
                        case 2:
                            specificQuery = "INSERT INTO ImageContentItems (ContentItemId, ImageUrl) VALUES(" + id + ", '" + imageUrl + "')";
                            specificCommand = new SqlCommand(specificQuery, sqlConn);
                            specificCommand.ExecuteNonQuery();
                            break;
                        case 3:
                            specificQuery = "INSERT INTO VideoContentItems (ContentItemId, VideoUrl) VALUES(" + id + ", '" + videoUrl + "')";
                            specificCommand = new SqlCommand(specificQuery, sqlConn);
                            specificCommand.ExecuteNonQuery();
                            break;
                        case 4:
                            specificQuery = "INSERT INTO FolderContentItems (ContentItemId) VALUES(" + id + ")";
                            specificCommand = new SqlCommand(specificQuery, sqlConn);
                            specificCommand.ExecuteNonQuery();
                            break;
                        default:
                            throw new NotImplementedException("Do not recognise the content item type id");
                    }
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        // Delete a content item by its id
        public void Delete(long contentItemId)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    // Delete all rows linked to the content item, then finally delete the content item itself
                    var query = "DELETE FROM TextContentItems WHERE ContentItemId = " + contentItemId + ";" +
                                "DELETE FROM ImageContentItems WHERE ContentItemId = " + contentItemId + ";" +
                                "DELETE FROM VideoContentItems WHERE ContentItemId = " + contentItemId + ";" +
                                "DELETE FROM FolderContentItems WHERE ContentItemId = " + contentItemId + ";" +
                                "DELETE FROM ContentItems WHERE Id = " + contentItemId + ";";
                    var command = new SqlCommand(query, sqlConn);
                    command.ExecuteNonQuery();
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }
    }
}