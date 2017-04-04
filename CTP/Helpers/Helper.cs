using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CTP.Helpers
{
    public static class Helper
    {
        public static string SqlConnectionString = WebConfigurationManager.ConnectionStrings["MyDatabase"].ConnectionString;
    }
}