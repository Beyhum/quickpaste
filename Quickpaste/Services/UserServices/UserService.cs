using CryptoHelper;
using Microsoft.Extensions.Options;
using Quickpaste.Data;
using Quickpaste.Models;
using Quickpaste.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Quickpaste.Services.UserServices
{
    public class UserService
    {

        private readonly UserSettings _userSettings;

        private readonly IUserRepository _userRepository;

        public UserService(IOptions<UserSettings> userSettings, IUserRepository userRepository)
        {
            _userSettings = userSettings.Value;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Checks if the given username and password combination is valid
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>true if valid, false otherwise</returns>
        public async Task<bool> ValidateCredentials(string username, string password)
        {
            User userToAuthenticate = await _userRepository.Get(username);
            if(userToAuthenticate == null)
            {
                return false;
            }
            bool isMatch = Crypto.VerifyHashedPassword(userToAuthenticate.Password, password);

            return isMatch;
        }

        /// <summary>
        /// Checks if the given credentials match the default username and passcode to initialize an account
        /// </summary>
        /// <param name="defaultUsername"></param>
        /// <param name="defaultPasscode"></param>
        /// <returns>true if valid, false otherwise</returns>
        public bool ValidateDefaults(string defaultUsername, string defaultPasscode)
        {
            return (defaultUsername == _userSettings.DefaultUsername && defaultPasscode == _userSettings.DefaultPasscode);
        }


        /// <summary>
        /// Used to setup an account for a new Quickpaste app. Given the default username and passcode, an account with a desired username and password is created.
        /// The default username/password are found in the appsettings.json file by default.
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns>A Tuple with the result and reason for the response</returns>
        public async Task<(bool result, UserServiceResp reason)> Register(RegisterModel registerModel)
        {
            // If an account has already been set up, deny
            if (await _userRepository.Count() > 0)
            {
                return (false, UserServiceResp.AccountLimitReached);
            }
            else if (!ValidateDefaults(registerModel.DefaultUsername, registerModel.DefaultPasscode))
            {
                return (false, UserServiceResp.InvalidDefaults); // the default username and passcode required to setup an account are invalid
            }
            else if (await _userRepository.Get(registerModel.Username) != null)
            {
                return (false, UserServiceResp.AccountExists);
            }

            registerModel.Password = Crypto.HashPassword(registerModel.Password); // Hash and salt password before saving it

            User userToCreate = new User(registerModel);
            User createdUser = await _userRepository.Create(userToCreate);
            if(createdUser == null)
            {
                return (false, UserServiceResp.InternalError);
            }
            return (true, UserServiceResp.Ok);


        }
    }

    /// <summary>
    /// Used to return the reason for the UserService's responses to the caller
    /// e.g. if an internal error occurs the Service can return a tuple with a null and a UserServiceResp.InternalError
    /// </summary>
    public enum UserServiceResp
    {
        /// <summary>
        /// Action was successful
        /// </summary>
        Ok,
        /// <summary>
        /// The default username/passcode required to setup an account were invalid
        /// </summary>
        InvalidDefaults,
        /// <summary>
        /// An account with the given username exists
        /// </summary>
        AccountExists,
        /// <summary>
        /// The maximum amount of accounts that can be created has been reached
        /// </summary>
        AccountLimitReached,
        /// <summary>
        /// An unexpected error occured
        /// </summary>
        InternalError
    }
}
