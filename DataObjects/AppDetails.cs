using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public static class AppDetails
    {
        public static string AppPath { get; set; }
        public static string ImagePath
        {
            get
            {
                return AppPath + @"images\";
            }
        }
    }
}
