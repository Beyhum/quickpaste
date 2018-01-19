using Quickpaste.Models.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public User() { }

        public User(RegisterModel registerModel)
        {
            AutoMapper.Mapper.Map(registerModel, this);
        }

    }
}
