using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Quickpaste.Models.Dtos
{
    public class PasteGetDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("blob_url")]
        public string BlobUrl { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        private string _quickLink;
        [JsonProperty("quick_link")]
        public string QuickLink
        {
            get { return _quickLink; }
            set { _quickLink = WebUtility.UrlEncode(value); }
        }

        [JsonProperty("is_public")]
        public bool IsPublic { get; set; }


        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        public PasteGetDto(Paste paste)
        {
            Mapper.Map(paste, this);
        }
    }
}
