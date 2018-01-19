using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Models.Dtos
{
    public class SharedLinkPostDto
    {


        [JsonProperty("duration_in_minutes")]
        [Required]
        public int DurationInMinutes { get; set; }
    }
}
