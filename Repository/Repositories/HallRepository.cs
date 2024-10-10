using Biograf_Repository.Interfaces;
using Biograf_Repository.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Repositories
{
    public class HallRepository: GenericRepository<Hall>, IHallRepository
    {
        public HallRepository(DataContext context) : base(context) { }

        public async Task<int?> GetHallIdByShowtimeIdAsync(int showtimeId)
        {
            try
            {             
                if (showtimeId <= 0)
                {
                    throw new ArgumentException("Invalid showtimeId value.");
                }

                var hallId = await _context.Showtimes.Where(st => st.Id == showtimeId).Select(st => st.HallId).FirstOrDefaultAsync();
              
                if (hallId == 0)
                {
                    throw new Exception($"No hall found for the showtime with ID {showtimeId}.");
                }

                return hallId;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the hall ID for the showtime: {ex.Message}");
            }
        }

    }
}
