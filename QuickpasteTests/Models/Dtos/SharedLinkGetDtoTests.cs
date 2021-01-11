using Xunit;
using Quickpaste.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quickpaste.Models.Dtos
{
    public class SharedLinkGetDtoTests
    {
        [Fact]
        public void MapsFieldsCorrectly()
        {
            int durInMins = 100;
            string blobUrl = "blobUrl";

            var createdSharedLink = new SharedLinkGetDto(durInMins, blobUrl);
            
            Assert.Equal(durInMins, createdSharedLink.DurationInMinutes);
            Assert.Equal(blobUrl, createdSharedLink.BlobUrl);
        }
    }
}