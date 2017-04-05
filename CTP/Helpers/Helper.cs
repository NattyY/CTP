using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CTP.Helpers
{
    public static class Helper
    {
        // Get the connection string from the web.config
        public static string SqlConnectionString = WebConfigurationManager.ConnectionStrings["MyDatabase"].ConnectionString;
    }
}