using Biograf_Repository.Interfaces;
using Biograf_Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Test.Repositories
{
    public class MovieRepositoryTest : IDisposable
    {
        private readonly DataContext _context;
        private readonly IMovieRepository _repository;
        private readonly DbContextOptions<DataContext> _options;

        public MovieRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(_options);
            _repository = new MovieRepository(_context);

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetMovieByTitleAsync_ReturnsCorrectMovie()
        {
            // Arrange
            var movie = new Movie { Id=1, Title = "Inception", Duration = 148, IsActive = true };
            await _repository.AddAsync(movie);

            // Act
            var result = await _repository.GetMovieByTitleAsync("Inception");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Inception", result.Title);
        }

        [Fact]
        public async Task SearchMoviesByTitleAsync_ReturnsCorrectMovies()
        {
            // Arrange
            var movie1 = new Movie { Id = 1, Title = "Inception", Duration = 148, IsActive = true };
            var movie2 = new Movie { Id = 2, Title = "Inside Out", Duration = 95, IsActive = true };
            await _repository.AddAsync(movie1);
            await _repository.AddAsync(movie2);

            // Act
            var result = await _repository.SearchMoviesByTitleAsync("In");

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task DeleteMovieByTitleAsync_RemovesMovieSuccessfully()
        {
            // Arrange
            var movie = new Movie { Title = "Inception", Duration = 148, IsActive = true };
            await _repository.AddAsync(movie);

            // Act
            var result = await _repository.DeleteMovieByTitleAsync("Inception");
            var deletedMovie = await _repository.GetMovieByTitleAsync("Inception");

            // Assert
            Assert.NotNull(result);
            Assert.Null(deletedMovie);
        }

        [Fact]
        public async Task GetActiveMoviesByGenreAsync_ReturnsCorrectMovies()
        {
            // Arrange
            var genre = new Genre { Id = 1, Name = "Action" };
            var movie = new Movie { Title = "Inception", Duration = 148, IsActive = true };
            var movieGenre = new MovieGenre { Movie = movie, Genre = genre };
            movie.MovieGenres = new List<MovieGenre> { movieGenre };

            await _context.Genres.AddAsync(genre);
            await _repository.AddAsync(movie);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetActiveMoviesByGenreAsync(1);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("Inception", result[0].Title);
        }

        [Fact]
        public async Task SearchMoviesByTitleAsync_ReturnsEmpty_WhenNoMoviesMatch()
        {
            // Arrange
            var movie1 = new Movie { Id = 1, Title = "Inception", Duration = 148, IsActive = true };
            var movie2 = new Movie { Id = 2, Title = "Interstellar", Duration = 169, IsActive = true };
            await _repository.AddAsync(movie1);
            await _repository.AddAsync(movie2);

            // Act
            var result = await _repository.SearchMoviesByTitleAsync("NonExistent");

            // Assert
            Assert.Empty(result);
        }        

        [Fact]
        public async Task GetMovieByTitleAsync_ReturnsNull_WhenMovieDoesNotExist()
        {
            // Arrange
            var nonExistentTitle = "NonExistentMovie";

            // Act
            var result = await _repository.GetMovieByTitleAsync(nonExistentTitle);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetActiveMoviesByGenreAsync_ReturnsEmpty_WhenNoActiveMoviesInGenre()
        {
            // Arrange
            var genre = new Genre { Id = 1, Name = "Action" };
            var movie = new Movie { Title = "Inactive Movie", Duration = 120, IsActive = false };
            var movieGenre = new MovieGenre { Movie = movie, Genre = genre };
            movie.MovieGenres = new List<MovieGenre> { movieGenre };

            await _context.Genres.AddAsync(genre);
            await _repository.AddAsync(movie);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetActiveMoviesByGenreAsync(1);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAsync_ThrowsException_WhenMovieTitleIsNullDatabaseFail()
        {
            // Arrange
            var movie = new Movie { Id=1,Title = null, Duration = 120, IsActive = true };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await _repository.AddAsync(movie);
            });
        }

        [Fact]
        public async Task DeleteMovieByTitleAsync_ThrowsArgumentException_WhenTitleIsNullOrEmpty()
        {
            // Arrangez
            string invalidTitle = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _repository.DeleteMovieByTitleAsync(invalidTitle);
            });
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
