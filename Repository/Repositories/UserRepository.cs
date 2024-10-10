using Biograf_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using Repository.Data;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Repositories
{
    public class UserRepository: GenericRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context) { }

        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return null;
                }
                
                if (user.Password != password)
                {                   
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while authenticating the user: {ex.Message}");
            }
        }
    }
}
