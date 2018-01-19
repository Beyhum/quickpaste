using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Services.AuthServices
{
    public class AuthSettings
    {
        public string Key { get; set; }
        public int Duration { get; set; }

    }
}
