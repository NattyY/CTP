using CTP.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CTP.Models.Entities;
using System.Data.SqlClient;
using CTP.Models.Maps;

namespace CTP.Services
{
    public class WritingService : IWritingService
    {
        private IProjectService _projectService = new ProjectService();

        public IEnumerable<Writing> GetPublicWriting()
        {
            var output = new List<Writing>();

            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM Writing WHERE IsPublic = 1";
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        output.Add(WritingMaps.MapDbToEntity(reader));
                    }
                }
                finally
                {
                    sqlConn.Close();
                }
            }

            foreach (var writing in output)
            {
                writing.Project = _projectService.GetProject(writing.ProjectId);
            }

            return output;
        }

        public Writing GetWriting(long writingid)
        {
            Writing output;
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM Writing WHERE Id = " + writingid;
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    if (!reader.Read()) { return null; }

                    output = WritingMaps.MapDbToEntity(reader);
                }
                finally
                {
                    sqlConn.Close();
                }
            }

            var project = _projectService.GetProject(output.ProjectId);
            output.Project = project;

            return output;
        }

        public IEnumerable<Writing> GetWritingByProjectId(int projectId)
        {
            var output = new List<Writing>();

            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "SELECT * FROM Writing WHERE ProjectId = " + projectId;
                    var command = new SqlCommand(query, sqlConn);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        output.Add(WritingMaps.MapDbToEntity(reader));
                    }
                }
                finally
                {
                    sqlConn.Close();
                }
            }

            var project = _projectService.GetProject(projectId);
            foreach (var writing in output)
            {
                writing.Project = project;
            }

            return output;
        }

        public void InsertWriting(string title, string content, bool isPublic, int projectId, string urlName)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "INSERT INTO Writing (Title, Content, IsPublic, ProjectId, UrlName) VALUES('" + title.Replace("'", "''") + "', '" + content.Replace("'", "''") + "', " + (isPublic ? "1" : "0") + ", " + projectId + ", '" + urlName + "');";
                    var command = new SqlCommand(query, sqlConn);
                    command.ExecuteNonQuery();
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public void UpdateWriting(long writingId, string title, string content, bool isPublic, DateTime lastModified)
        {
            using (var sqlConn = new SqlConnection(Helpers.Helper.SqlConnectionString))
            {
                sqlConn.Open();
                try
                {
                    var query = "UPDATE Writing SET Title = '" + title.Replace("'", "''") + "', Content = '" + content.Replace("'", "''") + "', IsPublic = " + (isPublic ? "1" : "0") + ", LastModified = '" + lastModified.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE Id = " + writingId;
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