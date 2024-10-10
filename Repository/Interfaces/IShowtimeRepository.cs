using Biograf_Repository.DTO;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Interfaces
{
    public interface IShowtimeRepository : IGenericRepository<Showtime>
    {
        Task<IEnumerable<Showtime>> GetShowtimesByMovieIdAsync(int movieId);
        Task<IEnumerable<Showtime>> GetShowtimesByMovieAndCinema(int movieId, int cinemaId);
        Task<ShowtimeDetailsDTO> GetShowtimeDetailsAsync(int showtimeId);
    }
}
