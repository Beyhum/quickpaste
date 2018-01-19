using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Models.Dtos
{
    public class SettingsGetDto
    {
        [JsonProperty("max_file_bytes")]
        public uint MaxFileBytes { get; set; }
    }
}
