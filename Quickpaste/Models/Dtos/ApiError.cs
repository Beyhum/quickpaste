using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Models.Dtos
{
    /// <summary>
    /// Generic Api error with a property to display an error message to the client
    /// </summary>
    public class ApiError
    {
        [JsonProperty("display_text")]
        public string DisplayText { get; set; }

        public ApiError(string displayText)
        {
            this.DisplayText = displayText;
        }

        public ApiError(ModelStateDictionary invalidModelState): this(invalidModelState.First().Value.Errors.First().ErrorMessage)
        {
            
        }

        public static ApiError Unexpected()
        {
            return new ApiError("An unexpected error occured");
        }
    }
}
