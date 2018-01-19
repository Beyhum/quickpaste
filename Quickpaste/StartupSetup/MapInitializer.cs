using AutoMapper;
using Quickpaste.Models;
using Quickpaste.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.StartupSetup
{

    public class MapInitializer
    {
        /// <summary>
        /// Initializes Automapper mappings
        /// </summary>
        public static void Initialize()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Paste, PasteGetDto>();
                config.CreateMap<PasteAnonPostDto, PastePostDto>();
                config.CreateMap<RegisterModel, User>();
            });
        }
    }
}
