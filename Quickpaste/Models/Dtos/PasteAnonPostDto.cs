using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Models.Dtos
{
    /// <summary>
    /// DTO for creating a Paste as an anonymous (unauthenticated) user
    /// </summary>
    public class PasteAnonPostDto
    {
        [FromForm(Name ="message")]
        [JsonProperty("message")]
        public string Message { get; set; }

        [FromForm(Name = "file_to_upload")]
        [JsonProperty("file_to_upload")]
        public IFormFile FileToUpload { get; set; }
    }
}
