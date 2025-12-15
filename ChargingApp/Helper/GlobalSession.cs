using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChargingApp.Helper;

namespace ChargingApp.Helper
{
    public static class GlobalSession
    {
        public static long CurrentUserAppId { get; set; }
        public static long? CurrentEmployeeId { get; set; }
        public static string CurrentUsername { get; set; }
        public static string Fullname { get; set; }
    }

}
