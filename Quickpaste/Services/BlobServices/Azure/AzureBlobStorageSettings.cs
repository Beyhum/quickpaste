using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Services.BlobServices.Azure
{
    public class AzureBlobStorageSettings
    {
        public string Account { get; set; }
        public string Key { get; set; }
        public string PublicContainerName { get; set; }
        public string PrivateContainerName { get; set; }
    }
}
