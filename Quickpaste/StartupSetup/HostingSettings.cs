using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.StartupSetup
{
    public class HostingSettings
    {
        public string Hostname { get; set; }
        public string ReverseProxyHost { get; set; }
        public bool RequireSSL { get; set; }
    }
}
