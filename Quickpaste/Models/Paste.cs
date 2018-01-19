using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Models
{
    /// <summary>
    /// A Paste can have a message (text) and a blob (file) URL associated with it.
    /// It can be queried either through its Id or QuickLink
    /// </summary>
    public class Paste
    {

        public string Id { get; private set; }
        
        public string BlobUrl { get; set; }

        public string Message { get; set; }

        public string QuickLink { get; private set; }

        [Required]
        public bool IsPublic { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; private set; }

        private Paste()
        {

        }

        /// <summary>
        /// Creates a new Paste object with ID null to create/persist to the database
        /// </summary>
        /// <param name="quickLink"></param>
        /// <param name="message"></param>
        /// <param name="blobUrl"></param>
        /// <param name="isPublic"></param>
        /// <returns>Paste to create</returns>
        public static Paste ToCreate(string quickLink, string message, string blobUrl, bool isPublic)
        {

            return new Paste(null, quickLink, message, blobUrl, isPublic, DateTimeOffset.Now);
        }


        

        private Paste(string id, string quickLink, string message, string blobUrl, bool isPublic, DateTimeOffset createdAt)
        {
            this.Id = id;
            this.QuickLink = quickLink;
            this.Message = message;
            this.BlobUrl = blobUrl;
            this.IsPublic = isPublic;
            this.CreatedAt = createdAt;

        }


    }
}
