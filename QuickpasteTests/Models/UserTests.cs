using Quickpaste.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Quickpaste.Models
{
    public class UserTests
    {
        [Fact]
        public void CreatesUserFromRegisterModel()
        {
            StartupSetup.MapInitializer.Initialize();
            var registerModel = new Dtos.RegisterModel() { Username = "uname", Password = "pword" };
            
            var createdUser = new User(registerModel);

            Assert.Equal(registerModel.Username, createdUser.Username);
            Assert.Equal(registerModel.Password, createdUser.Password);
        }
    }
}