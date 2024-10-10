using Biograf_Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Repository.Data;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biograf_Repository.DTO;

namespace Biograf_Repository.Repositories
{
    public class MovieRepository : GenericRepository<Movie>, IMovieRepository
    {      
        public MovieRepository(DataContext context) : base(context) { }


        //public async Task AddMovieAsync(Movie movie, List<int> genreIds)
        //{
        //    if (movie.ImageFile != null)
        //    {
        //        var fileName = Path.GetFileNameWithoutExtension(movie.ImageFile.FileName);
        //        var extension = Path.GetExtension(movie.ImageFile.FileName);
        //        var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
        //        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await movie.ImageFile.CopyToAsync(fileStream);
        //        }

        //        movie.ImagePath = $"/images/{uniqueFileName}";
        //    }

        //    await _context.Movies.AddAsync(movie);
        //    await _context.SaveChangesAsync();

        //    if (genreIds != null && genreIds.Any())
        //    {
        //        foreach (var genreId in genreIds)
        //        {
        //            var existingGenre = await _context.Genres.FindAsync(genreId);
        //            if (existingGenre == null)
        //            {
        //                throw new Exception($"Genre with Id {genreId} not found.");
        //            }

        //            var newMovieGenre = new MovieGenre
        //            {
        //                MovieId = movie.Id,
        //                GenreId = genreId
        //            };
        //            _context.MovieGenres.Add(newMovieGenre);
        //        }

        //        await _context.SaveChangesAsync();
        //    }
        //}

        public async Task<IEnumerable<Movie>> GetMoviesWithGenresAsync()
        {
            try
            {
                var movies = await _context.Movies
                                           .Include(m => m.MovieGenres)
                                           .ThenInclude(mg => mg.Genre)
                                           .ToListAsync();
            
                if (movies == null || !movies.Any())
                {
                    throw new Exception("No movies with genres found.");
                }
                return movies;
            }
            catch (Exception ex)
            {              
                throw new Exception($"An error occurred while retrieving movies with genres: {ex.Message}");
            }
        }

        public async Task<List<MovieDTO>> GetActiveMoviesAsync()
        {
            try
            {
                var movies = await _context.Movies
                    .Include(m => m.MovieGenres)  
                    .ThenInclude(mg => mg.Genre)  
                    .Where(m => m.IsActive)    
                    .Select(m => new MovieDTO
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Description = m.Description,
                        Duration = m.Duration,
                        ImagePath = m.ImagePath,
                        GenreIds = m.MovieGenres.Select(mg => mg.GenreId).ToList()
                    })
                    .ToListAsync();

                
                if (movies == null || !movies.Any())
                {
                    throw new Exception("No active movies found.");
                }

                return movies;
            }
            catch (Exception ex)
            {               
                throw new Exception($"An error occurred while retrieving active movies: {ex.Message}");
            }
        }

        public async Task<Movie> GetMovieByTitleAsync(string title)
        {
            try
            {               
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new ArgumentException("Title cannot be null or empty.");
                }
                var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Title == title);
              
                if (movie == null)
                {
                    throw new Exception($"Movie with title '{title}' not found.");
                }

                return movie;
            }
            catch (ArgumentException ex)
            {              
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {               
                throw new Exception($"An error occurred while retrieving the movie: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Movie>> SearchMoviesByTitleAsync(string title)
        {
            try
            {               
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new ArgumentException("Title cannot be null or empty.");
                }

                var movies = await _context.Movies
                                           .Where(m => m.Title.Contains(title))
                                           .ToListAsync();
               
                if (movies == null || !movies.Any())
                {
                    throw new Exception($"No movies found with title containing '{title}'.");
                }

                return movies;
            }
            catch (ArgumentException ex)
            {             
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {              
                throw new Exception($"An error occurred while searching for movies: {ex.Message}");
            }
        }

        public async Task<Movie> DeleteMovieByTitleAsync(string title)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new ArgumentException("Title cannot be null or empty.");
                }
                var movie = await _context.Set<Movie>().FirstOrDefaultAsync(m => m.Title == title);
              
                if (movie == null)
                {
                    throw new ArgumentException($"Movie with title '{title}' not found.");
                }

                _context.Set<Movie>().Remove(movie);
                await _context.SaveChangesAsync();

                return movie;
            }
            catch (ArgumentException ex)
            {               
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {             
                throw new Exception($"An error occurred while deleting the movie: {ex.Message}");
            }
        }

        public async Task<List<MovieDTO>> GetActiveMoviesByGenreAsync(int genreId)
        {
            try
            {             
                if (genreId <= 0)
                {
                    throw new ArgumentException("Invalid genreId value.");
                }

                var movies = await _context.MovieGenres
                    .Where(mg => mg.GenreId == genreId && mg.Movie.IsActive)
                    .Include(mg => mg.Movie)
                    .Select(mg => new MovieDTO
                    {
                        Id = mg.Movie.Id,
                        Title = mg.Movie.Title,
                        Description = mg.Movie.Description,
                        Duration = mg.Movie.Duration,
                        ImagePath = mg.Movie.ImagePath
                    })
                    .ToListAsync();
               
                if (movies == null || movies.Count == 0)
                {
                    throw new Exception("No active movies found for this genre.");
                }

                return movies;
            }
            catch (ArgumentException ex)
            {               
                throw new Exception($"Parameter error: {ex.Message}");
            }
            catch (Exception ex)
            {              
                throw new Exception($"An error occurred while retrieving movies: {ex.Message}");
            }
        }

    }
}
