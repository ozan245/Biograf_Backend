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
    public class PaymentRepositoryTest : IDisposable
    {
        private readonly DataContext _context;
        private readonly IPaymentRepository _repository;
        private readonly DbContextOptions<DataContext> _options;

        public PaymentRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;

            _context = new DataContext(_options);
            _repository = new PaymentRepository(_context);

            _context.Database.EnsureCreated();
        }


        [Fact]
        public async Task AddPaymentAsync_ThrowsArgumentNullExceptionForNullPaymentDto()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _repository.AddPaymentAsync(null);
            });
        }

        [Fact]
        public async Task GetPaymentByIdAsync_ReturnsNullForInvalidPaymentId()
        {
            // Arrange
            var invalidPaymentId = 999;

            // Act
            var result = await _repository.GetPaymentByIdAsync(invalidPaymentId);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task AddPaymentAsync_AddsPaymentSuccessfully()
        {
            // Arrange
            var paymentDto = new PaymentDTO
            {
                Amount = 100,
                UserId = 1
            };

            var user = new User { Id = 1, Name = "User1", Email = "user1@mail.com", Password = "123456", Role = "User" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.AddPaymentAsync(paymentDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paymentDto.Amount, (double)result.Amount);
            Assert.Equal(paymentDto.UserId, result.UserId);
            Assert.True(result.PaymentDate <= DateTime.Now); 
        }

        [Fact]
        public async Task GetPaymentByIdAsync_ReturnsCorrectPayment()
        {
            // Arrange
            var user = new User { Id = 1, Name = "User1", Email = "user1@mail.com", Password = "123456", Role = "User" };
            var showtime = new Showtime { Id = 1, MovieId = 1, HallId = 1, Time = DateTime.Now };
            var payment = new Payment
            {
                Amount = 100.50,
                PaymentDate = DateTime.Now,
                UserId = user.Id
            };

            await _context.Users.AddAsync(user);
            await _context.Showtimes.AddAsync(showtime);
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPaymentByIdAsync(payment.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(payment.Id, result?.Id);
            Assert.Equal(payment.Amount, result?.Amount);
            Assert.Equal(payment.UserId, result?.UserId);
        }



        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    }
}
