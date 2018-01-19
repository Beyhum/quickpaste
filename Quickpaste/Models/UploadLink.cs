using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Models
{
    /// <summary>
    /// When an UploadLink is created (by an authenticated user), any unauthenticated user can create a paste by using the UploadLink's QuickLink.
    /// An UploadLink can only be used once to create a Paste, after which it is deleted
    /// </summary>
    public class UploadLink
    {

        [JsonProperty("quick_link")]
        [Key]
        [RegularExpression("[a-zA-Z\\d]+")]
        [Required]
        public string QuickLink { get; set; }

        [JsonProperty("allow_file")]
        [Required]
        public bool AllowFile { get; set; } = true;
        


    }
}
