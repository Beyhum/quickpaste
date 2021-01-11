using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quickpaste.Models
{
    public class PasteTests
    {
        [Fact]
        public void ToCreateSetsIdToValidStringGuid()
        {
            var createdPaste = Paste.ToCreate("any", "any", "any", true);
            
            var parsedGuid = Guid.Parse(createdPaste.Id);
            Assert.Equal(parsedGuid.ToString(), createdPaste.Id);
        }

        [Fact]
        public void ToCreateMapsFieldsCorrectly()
        {
            string quickLink = "quickLink", message = "message", blobUrl = "blobUrl";
            bool publicStatus = false;

            var createdPaste = Paste.ToCreate(quickLink, message, blobUrl, publicStatus);

            Assert.Equal(quickLink, createdPaste.QuickLink);
            Assert.Equal(message, createdPaste.Message);
            Assert.Equal(blobUrl, createdPaste.BlobUrl);
            Assert.Equal(publicStatus, createdPaste.IsPublic);
        }
    }
}