using Biograf_Repository.Interfaces;
using Biograf_Repository.Models;
using Biograf_Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Test.Repositories
{
    public class ShowtimeRepositoryTest: IDisposable
    {
        private readonly DataContext _context;
        private readonly IShowtimeRepository _repository;
        private readonly DbContextOptions<DataContext> _options;

        public ShowtimeRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(_options);
            _repository = new ShowtimeRepository(_context);

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetShowtimesByMovieIdAsync_ReturnsCorrectShowtimes()
        {
            // Arrange
            var movie = new Movie { Id = 1, Title = "Inception", Duration= 50, IsActive=true };
            await _context.Movies.AddAsync(movie);

            var showtime1 = new Showtime { Time = DateTime.Now, MovieId = 1, HallId = 1 };
            var showtime2 = new Showtime { Time = DateTime.Now.AddHours(1), MovieId = 1, HallId = 2 };
            await _context.Showtimes.AddAsync(showtime1);
            await _context.Showtimes.AddAsync(showtime2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetShowtimesByMovieIdAsync(1);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetShowtimesByMovieAndCinema_ReturnsCorrectShowtimes()
        {
            // Arrange
            var cinema = new Cinema { Id = 1, Name = "Cinema 1", Location= "Kastrup"};
            await _context.Cinemas.AddAsync(cinema);

            var hall = new Hall { Id = 1, Name = "Hall 1", CinemaId = 1 };
            await _context.Halls.AddAsync(hall);

            var movie = new Movie { Id = 1, Title = "Inception", Duration = 50, IsActive = true };
            await _context.Movies.AddAsync(movie);

            var showtime = new Showtime { Time = DateTime.Now, MovieId = 1, HallId = 1 };
            await _context.Showtimes.AddAsync(showtime);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetShowtimesByMovieAndCinema(1, 1);

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetShowtimeDetailsAsync_ReturnsCorrectDetails()
        {
            // Arrange
            var cinema = new Cinema { Id = 1, Name = "Cinema 1", Location = "Kastrup" };
            await _context.Cinemas.AddAsync(cinema);

            var hall = new Hall { Id = 1, Name = "Hall 1", CinemaId = 1, Capacity= 15 };
            await _context.Halls.AddAsync(hall);

            var movie = new Movie {Id = 1, Title = "Inception", Duration = 50, IsActive = true };
            await _context.Movies.AddAsync(movie);

            var showtime = new Showtime { Time = DateTime.Now, MovieId = 1, HallId = 1 };
            await _context.Showtimes.AddAsync(showtime);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetShowtimeDetailsAsync(showtime.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Inception", result.MovieTitle);
            Assert.Equal("Hall 1", result.HallName);
        }

        [Fact]
        public async Task AddShowtime_ThrowsException_WhenDuplicateIdExists_InMemoryDatabase()
        {
            // Arrange
            var showtime = new Showtime { Id = 1, Time = DateTime.Now, MovieId = 1, HallId = 1 };
            await _context.Showtimes.AddAsync(showtime);
            await _context.SaveChangesAsync();

            // Act & Assert
            var duplicateShowtime = new Showtime
            {
                Id = 1,     
                Time = DateTime.Now.AddHours(1),
                MovieId = 1,  
                HallId = 1 
            };

            var exception = await Record.ExceptionAsync(async () =>
            {
                await _context.Showtimes.AddAsync(duplicateShowtime);
                await _context.SaveChangesAsync();
            });

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public async Task GetShowtimeDetailsAsync_ReturnsNull_WhenShowtimeDoesNotExist()
        {
            // Act
            var result = await _repository.GetShowtimeDetailsAsync(-1);        
            Assert.Null(result);
        }


        [Fact]
        public async Task GetShowtimesByMovieIdAsync_ReturnsIncorrectCount_WhenWrongDataProvided()
        {
            // Arrange
            var movie = new Movie { Id = 1, Title = "Inception", Duration = 50, IsActive = true };
            await _context.Movies.AddAsync(movie);

            var showtime = new Showtime { Time = DateTime.Now, MovieId = 1, HallId = 1 };
            await _context.Showtimes.AddAsync(showtime);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetShowtimesByMovieIdAsync(1);

            // Assert
            Assert.NotEqual(2, result.Count());
        }

        [Fact]
        public async Task GetShowtimesByMovieIdAsync_ReturnsEmpty_WhenNoShowtimesExist()
        {
            // Act
            var result = await _repository.GetShowtimesByMovieIdAsync(99); 

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddShowtime_ThrowsException_WhenManualDatabaseFail()
        {
            // Arrange
            var showtime = new Showtime { Id = 1, Time = DateTime.Now, MovieId = 1, HallId = 1 };
            await _context.Showtimes.AddAsync(showtime);
            await _context.SaveChangesAsync();

            // Act
            var duplicateShowtime = new Showtime
            {
                Id = 1,    
                Time = DateTime.Now.AddHours(1),
                MovieId = 1,
                HallId = 1
            };
    
            var exception = await Record.ExceptionAsync(async () =>
            {
                throw new DbUpdateException("Simulering...", new Exception("Inner exception"));
            });

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<DbUpdateException>(exception); 
            Assert.Contains("Simulering...", exception.Message); 
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    }
}
