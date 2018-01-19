using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Models.Dtos
{
    /// <summary>
    /// Model to register a user when none exist in the database. 
    /// The DefaultUsername and DefaultPasscode are only used to setup the first user's credentials (Username and Password properties)
    /// DefaultUsername and DefaultPasscode cannot be used to authenticate into the application
    /// </summary>
    public class RegisterModel
    {

        [Required]
        [JsonProperty("username")]
        public string Username { get; set; }

        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }

        [Required]
        [JsonProperty("default_username")]
        public string DefaultUsername { get; set; }

        [Required]
        [JsonProperty("default_passcode")]
        public string DefaultPasscode { get; set; }
    }
}
