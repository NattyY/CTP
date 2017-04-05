using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CTP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            // Map urls to the controller actions
            #region Projects
            routes.MapRoute(
                name: "Create Project",
                url: "projects/createproject",
                defaults: new { controller = "Projects", action = "CreateProject" }
            );

            routes.MapRoute(
                name: "Create Category",
                url: "projects/createcategory",
                defaults: new { controller = "Projects", action = "CreateCategory" }
            );

            routes.MapRoute(
                name: "Create Content Item",
                url: "projects/createcontentitem",
                defaults: new { controller = "Projects", action = "CreateContentItem" }
            );

            routes.MapRoute(
                name: "Delete Content Item",
                url: "projects/deletecontentitem",
                defaults: new { controller = "Projects", action = "DeleteContentItem" }
            );

            routes.MapRoute(
                name: "Projects Homepage",
                url: "projects/{projectName}",
                defaults: new { controller = "Projects", action = "Categories" }
            );

            routes.MapRoute(
                name: "Project Category",
                url: "projects/{projectName}/{categoryName}",
                defaults: new { controller = "Projects", action = "Category" }
            );

            // * allows for infinite url parameters to be passed into Controller Action: http://stackoverflow.com/a/7515690
            routes.MapRoute(
                name: "Content Item",
                url: "projects/{projectName}/{categoryName}/{*contentNames}",
                defaults: new { controller = "Projects", action = "ContentItem" }
            );
            #endregion Projects

            #region Writing
            routes.MapRoute(
                name: "Create Writing",
                url: "writing/createwriting",
                defaults: new { controller = "Writing", action = "CreateWriting" }
            );

            routes.MapRoute(
                name: "Edit Writing",
                url: "writing/editwriting",
                defaults: new { controller = "Writing", action = "EditWriting" }
            );

            routes.MapRoute(
                name: "Project Drawer",
                url: "writing/projectdrawer",
                defaults: new { controller = "Writing", action = "ProjectDrawer" }
            );

            routes.MapRoute(
                name: "Create Writing Page",
                url: "writing/create-new/{projectName}",
                defaults: new { controller = "Writing", action = "CreateWritingPage", projectName = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "View Writing",
                url: "writing/{projectName}/{writingName}",
                defaults: new { controller = "Writing", action = "ViewWriting" }
            );

            routes.MapRoute(
                name: "Edit Writing Form",
                url: "writing/{projectName}/{writingName}/edit",
                defaults: new { controller = "Writing", action = "Edit" }
            );

            routes.MapRoute(
                name: "Writing Homepage",
                url: "writing/{projectName}",
                defaults: new { controller = "Writing", action = "Index" }
            );

            #endregion Writing

            #region Profile
            routes.MapRoute(
                name: "View Public Writing",
                url: "profile/{username}/{projectName}/{writingName}",
                defaults: new { controller = "Writing", action = "ViewPublicWriting" }
            );

            routes.MapRoute(
                name: "View Profile",
                url: "profile/{username}",
                defaults: new { controller = "Home", action = "Profile" }
            );
            #endregion Profile

            // Default url config
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
