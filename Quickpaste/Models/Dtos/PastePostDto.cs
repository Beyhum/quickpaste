using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Models.Dtos
{
    /// <summary>
    /// DTO for creating a Paste as an authenticated user
    /// </summary>
    public class PastePostDto
    {
        [FromForm(Name = "message")]
        [JsonProperty("message")]
        public string Message { get; set; }

        [FromForm(Name = "quick_link")]
        [JsonProperty("quick_link")]
        [RegularExpression("[a-zA-Z\\d]+")]
        [Required]
        public string QuickLink { get; set; }

        [FromForm(Name = "is_public")]
        [JsonProperty("is_public")]
        [Required]
        public bool IsPublic { get; set; } = true;

        [FromForm(Name = "file_to_upload")]
        [JsonProperty("file_to_upload")]
        public IFormFile FileToUpload { get; set; }


    }
}
