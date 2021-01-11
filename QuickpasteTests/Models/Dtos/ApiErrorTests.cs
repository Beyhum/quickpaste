using Xunit;
using Quickpaste.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quickpaste.Models.Dtos
{
    public class ApiErrorTests
    {
        [Fact]
        public void MapsFieldsCorrectly()
        {
            string displayText = "displayable error msg";
            
            var apiError = new ApiError(displayText);
            
            Assert.Equal(displayText, apiError.DisplayText);
        }
    }
}