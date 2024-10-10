using Biograf_Repository.DTO;
using Biograf_Repository.Interfaces;
using Biograf_Repository.Models;
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
    public class TicketRepositoryTest : IDisposable
    {
        private readonly DataContext _context;
        private readonly ITicketRepository _repository;
        private readonly DbContextOptions<DataContext> _options;

        public TicketRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;

            _context = new DataContext(_options);
            _repository = new TicketRepository(_context);

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetTicketByIdAsync_ReturnsCorrectTicket()
        {
            // Arrange
            var movie = new Movie { Id = 1, Title = "Inception", Duration = 50, IsActive = true };
            var cinema = new Cinema { Id = 1, Name = "Cinema 1", Location = "Kastrup" };
            var hall = new Hall { Id = 1, Name = "Hall 1", CinemaId = 1 };           
            var showtime = new Showtime { Id = 1, MovieId = 1, HallId = 1, Time = DateTime.Now };
            var seat = new Seat { Id = 1, Row = "A", Number = 1, HallId = 1, IsReserved = false };
            var payment = new Payment { Id = 1, Amount = 50, PaymentDate = DateTime.Now };
            var reservation = new Reservation { Id = 1, UserId = 1, ShowtimeId = 1, ReservationTime = DateTime.Now, };

            await _context.Movies.AddAsync(movie);
            await _context.Cinemas.AddAsync(cinema);
            await _context.Halls.AddAsync(hall);
            await _context.Showtimes.AddAsync(showtime);
            await _context.Seats.AddAsync(seat);
            await _context.Payments.AddAsync(payment);
            await _context.Reservations.AddAsync(reservation);
            _context.SaveChanges();

            var ticket = new Ticket
            {
                Id = 1,
                PurchaseDate = DateTime.Now,
                ShowtimeId = showtime.Id,
                SeatId = seat.Id,
                PaymentId = payment.Id,
                ReservationId = reservation.Id
            };

            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTicketByIdAsync(ticket.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.Id, result?.Id);
        }

        [Fact]
        public async Task GetTicketsByUserAsync_ReturnsTicketsForCorrectUser()
        {
            // Arrange
            var user = new User { Id = 1, Name = "user1", Email = "den@mail.com" ,Password = "123456", Role = "User" };
            var movie = new Movie { Id = 1, Title = "Inception", Duration = 50, IsActive = true };
            var cinema = new Cinema { Id = 1, Name = "Cinema 1", Location = "Kastrup" };
            var hall = new Hall { Id = 1, Name = "Hall 1", CinemaId = 1 };
            var showtime = new Showtime { Id = 1, MovieId = 1, HallId = 1, Time = DateTime.Now };
            var seat = new Seat { Id = 1, Row = "A", Number = 1, HallId = 1, IsReserved = false };
            var payment = new Payment { Id = 1, Amount = 50, PaymentDate = DateTime.Now, UserId = 1 };
            var reservation = new Reservation { Id = 1, UserId = 1, ShowtimeId = 1, ReservationTime = DateTime.Now };

            await _context.Users.AddAsync(user);
            await _context.Movies.AddAsync(movie);
            await _context.Cinemas.AddAsync(cinema);
            await _context.Halls.AddAsync(hall);
            await _context.Showtimes.AddAsync(showtime);
            await _context.Seats.AddAsync(seat);
            await _context.Payments.AddAsync(payment);
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            var ticket = new Ticket
            {
                Id = 1,
                PurchaseDate = DateTime.Now,
                ShowtimeId = showtime.Id,
                SeatId = seat.Id,
                PaymentId = payment.Id,
                ReservationId = reservation.Id
            };

            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTicketsByUserAsync(reservation.UserId);

            // Assert
            Assert.NotEmpty(result);
            Assert.All(result, t => Assert.Equal(reservation.UserId, t.Reservation.UserId));
        }


        [Fact]
        public async Task AddTicketsAsync_AddsTicketSuccessfully()
        {
            // Arrange

            var movie = new Movie { Id = 1, Title = "Inception", Duration = 50, IsActive = true };
            var cinema = new Cinema { Id = 1, Name = "Cinema 1", Location = "Kastrup" };
            var hall = new Hall { Id = 1, Name = "Hall 1", CinemaId = 1 };
            var showtime = new Showtime { Id = 1, MovieId = 1, HallId = 1, Time = DateTime.Now };
            var seat = new Seat { Id = 1, Row = "A", Number = 1, HallId = 1, IsReserved = false };
            var payment = new Payment { Id = 1, Amount = 50, PaymentDate = DateTime.Now };
            var reservation = new Reservation { Id = 1, UserId = 1, ShowtimeId = 1, ReservationTime = DateTime.Now, };

            await _context.Movies.AddAsync(movie);
            await _context.Cinemas.AddAsync(cinema);
            await _context.Halls.AddAsync(hall);
            await _context.Showtimes.AddAsync(showtime);
            await _context.Seats.AddAsync(seat);
            await _context.Payments.AddAsync(payment);
            await _context.Reservations.AddAsync(reservation);
            _context.SaveChanges();


            var ticketDto = new TicketDTO
            {              
                PurchaseDate = DateTime.Now,
                ShowtimeId = 1,
                SeatIds = new List<int> { 1 },
                PaymentId = 1,
                ReservationId = 1
            };

            
            var ticket = new Ticket
            {
                Id = 1,
                PurchaseDate = ticketDto.PurchaseDate,
                ShowtimeId = ticketDto.ShowtimeId,
                PaymentId = ticketDto.PaymentId,
                ReservationId = ticketDto.ReservationId               
            };

            // Act
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
           
            var result = await _repository.GetTicketByIdAsync(ticket.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.Id, result?.Id);
        }


        [Fact]
        public async Task GetTicketDetailsByPaymentIdAsync_ReturnsCorrectDetails()
        {
            // Arrange
            var user = new User { Id = 1, Name = "user1", Email = "den@mail.com", Password = "123456", Role = "User" };
            var movie = new Movie { Id = 1, Title = "Inception", Duration = 50, IsActive = true };
            var cinema = new Cinema { Id = 1, Name = "Cinema 1", Location = "Kastrup" };
            var hall = new Hall { Id = 1, Name = "Hall 1", CinemaId = 1 };
            var showtime = new Showtime { Id = 1, MovieId = 1, HallId = 1, Time = DateTime.Now };
            var seat = new Seat { Id = 1, Row = "A", Number = 1, HallId = 1, IsReserved = false };
            var payment = new Payment { Id = 1, Amount = 50, PaymentDate = DateTime.Now, UserId = 1 };
            var reservation = new Reservation { Id = 1, UserId = 1, ShowtimeId = 1, ReservationTime = DateTime.Now };


            var ticket = new Ticket
            {
                PurchaseDate = DateTime.Now,
                ShowtimeId = showtime.Id,
                SeatId = seat.Id,
                PaymentId = payment.Id,
                ReservationId = reservation.Id
            };

            await _context.Users.AddAsync(user);
            await _context.Movies.AddAsync(movie);
            await _context.Cinemas.AddAsync(cinema);
            await _context.Halls.AddAsync(hall);
            await _context.Showtimes.AddAsync(showtime);
            await _context.Seats.AddAsync(seat);
            await _context.Payments.AddAsync(payment);
            await _context.Reservations.AddAsync(reservation);
            await _repository.AddAsync(ticket);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTicketDetailsByPaymentIdAsync(payment.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.PurchaseDate, result.PurchaseDate);         
        }

        [Fact]
        public async Task GetTicketByIdAsync_ReturnsNullForInvalidId()
        {
            // Arrange
            var invalidTicketId = 999; 

            // Act
            var result = await _repository.GetTicketByIdAsync(invalidTicketId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTicketsByUserAsync_ReturnsEmptyForInvalidUserId()
        {
            // Arrange
            var invalidUserId = 999; 

            // Act
            var result = await _repository.GetTicketsByUserAsync(invalidUserId);

            // Assert
            Assert.Empty(result); 
        }

        [Fact]
        public async Task AddTicketsAsync_ThrowsExceptionForDuplicateTicket()
        {
            // Arrange
            var user = new User { Id = 1, Name = "user1", Email = "den@mail.com", Password = "123456", Role = "User" };
            var movie = new Movie { Id = 1, Title = "Inception", Duration = 50, IsActive = true };
            var cinema = new Cinema { Id = 1, Name = "Cinema 1", Location = "Kastrup" };
            var hall = new Hall { Id = 1, Name = "Hall 1", CinemaId = 1 };
            var showtime = new Showtime { Id = 1, MovieId = 1, HallId = 1, Time = DateTime.Now };
            var seat = new Seat { Id = 1, Row = "A", Number = 1, HallId = 1, IsReserved = false };
            var payment = new Payment { Id = 1, Amount = 50, PaymentDate = DateTime.Now, UserId = 1 };
            var reservation = new Reservation { Id = 1, UserId = 1, ShowtimeId = 1, ReservationTime = DateTime.Now };

            await _context.Users.AddAsync(user);
            await _context.Movies.AddAsync(movie);
            await _context.Cinemas.AddAsync(cinema);
            await _context.Halls.AddAsync(hall);
            await _context.Showtimes.AddAsync(showtime);
            await _context.Seats.AddAsync(seat);
            await _context.Payments.AddAsync(payment);
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            var ticket = new Ticket
            {
                Id = 1,
                PurchaseDate = DateTime.Now,
                ShowtimeId = showtime.Id,
                SeatId = seat.Id,
                PaymentId = payment.Id,
                ReservationId = reservation.Id
            };

            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
           
            var duplicateTicket = new Ticket
            {
                Id = 1,
                PurchaseDate = DateTime.Now,
                ShowtimeId = showtime.Id,
                SeatId = seat.Id,
                PaymentId = payment.Id,
                ReservationId = reservation.Id
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _context.Tickets.AddAsync(duplicateTicket); 
                await _context.SaveChangesAsync();
            });
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
