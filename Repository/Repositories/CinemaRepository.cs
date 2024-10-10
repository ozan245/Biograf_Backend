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
    public class CinemaRepository: GenericRepository<Cinema>, ICinemaRepository
    {
        public CinemaRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Cinema>> GetCinemasByMovie(int movieId)
        {
            try
            {               
                if (movieId <= 0)
                {
                    throw new ArgumentException("Invalid movieId value.");
                }

                var cinemas = await _context.Showtimes
                    .Where(st => st.MovieId == movieId)
                    .Select(st => new Cinema
                    {
                        Id = st.Hall.Cinema.Id,
                        Name = st.Hall.Cinema.Name,
                        Location = st.Hall.Cinema.Location
                    })
                    .Distinct()
                    .ToListAsync();
                
                if (cinemas == null || !cinemas.Any())
                {
                    throw new Exception($"No cinemas found for the movie with ID {movieId}.");
                }

                return cinemas;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {               
                throw new Exception($"An error occurred while retrieving cinemas for the movie: {ex.Message}");
            }
        }
    }
}
