using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> AuthenticateUserAsync(string email, string password);
    }
}
