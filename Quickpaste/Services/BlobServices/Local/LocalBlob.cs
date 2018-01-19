using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Services.BlobServices.Local
{
    public class LocalBlob
    {
        [Key]
        public string BlobName { get; set; }

        [Required]
        public byte[] BlobData { get; set; }

        [Required]
        public string ContentType { get; set; }


        [Required]
        public bool IsPublic { get; set; }

        
        private LocalBlob() { }

        public LocalBlob(string fileName, byte[] fileData, string contentType, bool isPublic)
        {
            this.BlobName = fileName;
            this.ContentType = contentType;
            this.IsPublic = isPublic;
            this.BlobData = fileData;
        }
    }
}
