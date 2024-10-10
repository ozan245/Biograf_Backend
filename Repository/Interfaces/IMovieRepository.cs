using Biograf_Repository.DTO;
using Microsoft.AspNetCore.Http;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Interfaces
{
    public interface IMovieRepository : IGenericRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetMoviesWithGenresAsync();
        Task<List<MovieDTO>> GetActiveMoviesAsync();
        Task<Movie> GetMovieByTitleAsync(string title);
        Task<IEnumerable<Movie>> SearchMoviesByTitleAsync(string title);
        Task<Movie> DeleteMovieByTitleAsync(string title);
        Task<List<MovieDTO>> GetActiveMoviesByGenreAsync(int genreId);




    }
}
