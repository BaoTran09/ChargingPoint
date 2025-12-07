using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargingApp.Helper
{
    internal class EmailConfig
    {
      
            public string SmtpHost { get; set; }
            public int SmtpPort { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public bool EnableSsl { get; set; }
        

    }
}
