using Biograf_Repository.Models;
using Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Interfaces
{
    public interface IHallRepository : IGenericRepository<Hall>
    {
        Task<int?> GetHallIdByShowtimeIdAsync(int showtimeId);
    }
}
