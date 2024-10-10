using Biograf_Repository.DTO;
using Biograf_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Repositories
{
    public class ShowtimeRepository : GenericRepository<Showtime>, IShowtimeRepository
    {
         public ShowtimeRepository(DataContext context) : base(context) { }

         public async Task<IEnumerable<Showtime>> GetShowtimesByMovieIdAsync(int movieId)
         {
            try
            {
                if (movieId <= 0)
                {
                    throw new ArgumentException("Invalid movieId value.");
                }

                var showtimes = await _context.Showtimes.Where(s => s.MovieId == movieId).ToListAsync();

                if (showtimes == null || !showtimes.Any())
                {
                    throw new Exception($"No showtimes found for the movie with ID {movieId}.");
                }
                return showtimes;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving showtimes for the movie with ID {movieId}: {ex.Message}");
            }
        }
        public async Task<IEnumerable<Showtime>> GetShowtimesByMovieAndCinema(int movieId, int cinemaId)
        {
            try
            {
                if (movieId <= 0)
                {
                    throw new ArgumentException("Invalid movieId value.");
                }

                if (cinemaId <= 0)
                {
                    throw new ArgumentException("Invalid cinemaId value.");
                }

                var showtimes = await _context.Showtimes.Where(st => st.MovieId == movieId && st.Hall.CinemaId == cinemaId).ToListAsync();

                if (showtimes == null || !showtimes.Any())
                {
                    throw new Exception($"No showtimes found for movie ID {movieId} in cinema ID {cinemaId}.");
                }
                return showtimes;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving showtimes for movie ID {movieId} and cinema ID {cinemaId}: {ex.Message}");
            }
        }

        public async Task<ShowtimeDetailsDTO> GetShowtimeDetailsAsync(int showtimeId)
        {
            try
            {
                if (showtimeId <= 0)
                {
                    throw new ArgumentException("Invalid showtimeId value.");
                }

                var showtimeDetails = await (from s in _context.Showtimes 
                join m in _context.Movies on s.MovieId equals m.Id
                join h in _context.Halls on s.HallId equals h.Id
                join c in _context.Cinemas on h.CinemaId equals c.Id
                where s.Id == showtimeId
                select new ShowtimeDetailsDTO
                {
                  MovieTitle = m.Title,
                  CinemaName = c.Name,
                  HallName = h.Name,
                  Time = s.Time
                }).FirstOrDefaultAsync();

                if (showtimeDetails == null)
                {
                    throw new Exception($"No showtime details found for ID {showtimeId}.");
                }
                return showtimeDetails;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving showtime details for ID {showtimeId}: {ex.Message}");
            }
        }


    }
}
