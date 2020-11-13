using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace OTS_API.Utilities
{
    public static class Config
    {
        public static string connStr = "server=124.70.177.180;port=3306;user=root;password=a123456; database=education_system;";
        public static string publicFilePath = "../../FileRoot/Public/";
        public static string privateFilePath = "../../FileRoot/Private/";
        public static string tempFilePath = "../../FileRoot/Temp/";
        public static string wwwrootPath = "../../wwwroot";
    }
}
