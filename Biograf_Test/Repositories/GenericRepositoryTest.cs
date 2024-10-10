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
    public class GenericRepositoryTest : IDisposable
    {
        private readonly DataContext _context;
        private readonly IGenericRepository<Genre> _repository;
        private readonly IGenericRepository<Movie> _movierepository;
        private readonly DbContextOptions<DataContext> _options;

        public GenericRepositoryTest()
        {
            
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(_options);          
            _repository = new GenericRepository<Genre>(_context);
            _movierepository = new GenericRepository<Movie>(_context);
            _context.Database.EnsureCreated();
        }


        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectGenre()
        {
            // Arrange
            var genre = new Genre { Id = 1, Name = "Comedy" };
            await _repository.AddAsync(genre);

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Comedy", result.Name);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsCorrectGenre()
        {
            // Arrange
            var genre = new Genre { Id = 1, Name = "Horror" };
            await _repository.AddAsync(genre);

            // Act
            var result = await _repository.GetByNameAsync("Horror");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Horror", result.Name);
        }

        [Fact]
        public async Task AddAsync_AddsGenreSuccessfully()
        {
            // Arrange
            var genre = new Genre { Id = 2, Name = "Drama" };

            // Act
            await _repository.AddAsync(genre);

            // Assert
            var result = await _repository.GetByIdAsync(2);
            Assert.NotNull(result);
            Assert.Equal("Drama", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesGenreSuccessfully()
        {
            // Arrange
            var genre = new Genre { Id = 1, Name = "Romance" };
            await _repository.AddAsync(genre);

            // Act
            genre.Name = "Sci-Fi";
            await _repository.UpdateAsync(genre);

            // Assert
            var result = await _repository.GetByIdAsync(1);
            Assert.Equal("Sci-Fi", result.Name);
        }

        [Fact]
        public async Task DeleteAsync_DeletesGenreSuccessfully()
        {
            // Arrange
            var genre = new Genre { Id = 1, Name = "Documentary" };
            await _repository.AddAsync(genre);

            // Act
            await _repository.DeleteAsync(1);

            // Assert
            var result = await _repository.GetByIdAsync(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task AddMovieAsync_AddMovieSuccessfully()
        {
            // Arrange
            var entity = new Movie { Id = 1, Title = "Inception", Duration = 148, IsActive = true };

            // Act
            await _movierepository.AddAsync(entity); 
            var result = await _movierepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result); 
            Assert.Equal(1, result.Id); 
            Assert.Equal("Inception", result.Title);
        }

        [Fact]
        public async Task DeleteMovieAsync_DeletesMovieSuccessfully()
        {
            // Arrange
            var entity = new Movie { Id = 1, Title = "Inception", Duration = 148, IsActive = true };

            await _movierepository.AddAsync(entity);
            var addedEntity = await _movierepository.GetByIdAsync(1); 
            Assert.NotNull(addedEntity);

            // Act
            await _repository.DeleteAsync(1); 
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddMovieAsync_ThrowsArgumentNullExceptionForNullEntity()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _movierepository.AddAsync(null);
            });
        }

        [Fact]
        public async Task DeleteMovieAsync_DoesNotThrowExceptionForNonExistentEntity()
        {
            // Arrange
            var invalidId = 999;

            // Act
            await _movierepository.DeleteAsync(invalidId);

            // Assert
            var result = await _movierepository.GetByIdAsync(invalidId);
            Assert.Null(result);

        }

        public void Dispose()
        {         
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    }
}

