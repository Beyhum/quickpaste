using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Models.Dtos
{
    /// <summary>
    /// A Shared Link represents a URL to a blob that is normally private. 
    /// Using the SharedLink's BlobUrl, any unauthenticated user can access the blob for the DurationInMinutes
    /// </summary>
    public class SharedLinkGetDto
    {

        [JsonProperty("duration_in_minutes")]
        public int DurationInMinutes { get; set; }

        [JsonProperty("blob_url")]
        public string BlobUrl { get; set; }

        public SharedLinkGetDto(int durInMins, string blobUrl)
        {
            DurationInMinutes = durInMins;
            BlobUrl = blobUrl;
        }
    }
}
