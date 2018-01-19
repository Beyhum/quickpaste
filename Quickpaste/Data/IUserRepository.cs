using Quickpaste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Data
{
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="userToCreate"></param>
        /// <returns>The created user on success, otherwise null</returns>
        Task<User> Create(User userToCreate);

        /// <summary>
        /// Deletes a user with the given username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>true on success, false on failure</returns>
        Task<bool> Delete(string username);

        /// <summary>
        /// Gets the user with the given username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>User if found, null otherwise</returns>
        Task<User> Get(string username);

        /// <summary>
        /// Count the amount of existing users
        /// </summary>
        /// <returns>number of existing users</returns>
        Task<int> Count();
    }
}
