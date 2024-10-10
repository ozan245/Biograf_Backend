using Biograf_Repository.DTO;
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
    public class ReservationRepositoryTest
    {
        private readonly DataContext _context;
        private readonly IReservationRepository _repository;
        private readonly DbContextOptions<DataContext> _options;
        public ReservationRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(_options);
            _repository = new ReservationRepository(_context);

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetReservationsByUserIdAsync_ReturnsReservationsForCorrectUser()
        {
            // Arrange
            var user = new User { Id = 1, Name = "user1", Email = "den@mail.com", Password = "123456", Role = "User" };
            var showtime = new Showtime { Id = 1, MovieId = 1, HallId = 1, Time = DateTime.Now };

            var reservation1 = new Reservation { Id = 1, UserId = user.Id, ShowtimeId = showtime.Id, ReservationTime = DateTime.Now };
            var reservation2 = new Reservation { Id = 2, UserId = user.Id, ShowtimeId = showtime.Id, ReservationTime = DateTime.Now };

            await _context.Users.AddAsync(user);
            await _context.Showtimes.AddAsync(showtime);
            await _context.Reservations.AddRangeAsync(reservation1, reservation2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetReservationsByUserIdAsync(user.Id);

            // Assert
            Assert.NotEmpty(result);
            Assert.All(result, r => Assert.Equal(user.Id, r.UserId));
        }

        [Fact]
        public async Task AddReservationWithSeatsAsync_AddsReservationAndSeatsSuccessfully()
        {
            // Arrange
            var user = new User { Id = 1, Name = "user1", Email = "den@mail.com", Password = "123456", Role = "User" };
            var showtime = new Showtime { Id = 1, MovieId = 1, HallId = 1, Time = DateTime.Now };
            var seat1 = new Seat { Id = 1, Row = "A", Number = 1, HallId = 1 };
            var seat2 = new Seat { Id = 2, Row = "A", Number = 2, HallId = 1 };

            await _context.Users.AddAsync(user);
            await _context.Showtimes.AddAsync(showtime);
            await _context.Seats.AddRangeAsync(seat1, seat2);
            await _context.SaveChangesAsync();

            var reservationDto = new ReservationDTO
            {
                UserId = user.Id,
                ShowtimeId = showtime.Id,
                SeatIds = new List<int> { seat1.Id, seat2.Id }
            };

            // Act
            var result = await _repository.AddReservationWithSeatsAsync(reservationDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(showtime.Id, result.ShowtimeId);

            var reservationSeats = await _context.ReservationSeats.Where(rs => rs.ReservationId == result.Id).ToListAsync();
            Assert.Equal(2, reservationSeats.Count);
            Assert.Contains(reservationSeats, rs => rs.SeatId == seat1.Id);
            Assert.Contains(reservationSeats, rs => rs.SeatId == seat2.Id);
        }

        [Fact]
        public async Task GetReservationsByUserIdAsync_ReturnsEmptyForInvalidUserId()
        {
            // Arrange
            var invalidUserId = -1;
            var user = new User { Id = 1, Name = "user1", Email = "den@mail.com", Password = "123456", Role = "User" };
            var showtime = new Showtime { Id = 1, MovieId = 1, HallId = 1, Time = DateTime.Now };

            var reservation = new Reservation { Id = 1, UserId = user.Id, ShowtimeId = showtime.Id, ReservationTime = DateTime.Now };

            await _context.Users.AddAsync(user);
            await _context.Showtimes.AddAsync(showtime);
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetReservationsByUserIdAsync(invalidUserId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddReservationWithSeatsAsync_ThrowsExceptionForMissingSeats()
        {
            // Arrange
            var user = new User { Id = 1, Name = "user1", Email = "den@mail.com", Password = "123456", Role = "User" };
            var showtime = new Showtime { Id = 1, MovieId = 1, HallId = 1, Time = DateTime.Now };

            await _context.Users.AddAsync(user);
            await _context.Showtimes.AddAsync(showtime);
            await _context.SaveChangesAsync();

            var reservationDto = new ReservationDTO
            {
                UserId = user.Id,
                ShowtimeId = showtime.Id                
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _repository.AddReservationWithSeatsAsync(reservationDto);
            });
        }



        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
