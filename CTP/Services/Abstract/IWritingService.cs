using CTP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTP.Services.Abstract
{
    public interface IWritingService
    {
        IEnumerable<Writing> GetWritingByProjectId(int projectId);
        void InsertWriting(string title, string content, bool isPublic, int projectId, string urlName);
        Writing GetWriting(long writingid);
        void UpdateWriting(long writingId, string title, string content, bool isPublic, DateTime lastModified);
        IEnumerable<Writing> GetPublicWriting();
    }
}