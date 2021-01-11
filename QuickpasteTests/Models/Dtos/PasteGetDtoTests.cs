using Xunit;
using Quickpaste.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quickpaste.Models.Dtos
{
    public class PasteGetDtoTests
    {
        [Fact]
        public void MapsFieldsCorrectly()
        {
            StartupSetup.MapInitializer.Initialize();
            Paste paste = Paste.ToCreate("quickLink", "msg", "blobUrl", true);

            var pasteGetDto = new PasteGetDto(paste);
            
            Assert.Equal(paste.Id, pasteGetDto.Id);
            Assert.Equal(paste.QuickLink, pasteGetDto.QuickLink);
            Assert.Equal(paste.Message, pasteGetDto.Message);
            Assert.Equal(paste.BlobUrl, pasteGetDto.BlobUrl);
            Assert.Equal(paste.CreatedAt, pasteGetDto.CreatedAt);
        }

        [Fact]
        public void UrlEncodesQuickLink()
        {
            StartupSetup.MapInitializer.Initialize();
            string quickLinkWithEncodeableChars = "url/with/encodeable characters";
            Paste paste = Paste.ToCreate(quickLinkWithEncodeableChars, "any", "any", true);

            var pasteGetDto = new PasteGetDto(paste);

            string expectedQuickLink = "url%2Fwith%2Fencodeable+characters";
            Assert.Equal(expectedQuickLink, pasteGetDto.QuickLink);

        }
    }
}