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
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    { 
        public PaymentRepository(DataContext context) : base(context) { }

        public async Task<Payment> AddPaymentAsync(PaymentDTO paymentDto)
        {
            try
            {
                if (paymentDto == null)
                {
                    throw new ArgumentNullException(nameof(paymentDto), "PaymentDTO cannot be null.");
                }

                var payment = new Payment
                {
                    Amount = (double)paymentDto.Amount,
                    PaymentDate = DateTime.Now,
                    UserId = paymentDto.UserId
                };

                // Add payment to the database
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return payment;
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }           
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the payment: {ex.Message}");
            }
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            try
            {               
                if (id <= 0)
                {
                    throw new ArgumentException("Invalid ID value.");
                }

                var payment = await _context.Payments.Include(p => p.Tickets).ThenInclude(t => t.Showtime).FirstOrDefaultAsync(p => p.Id == id);

                if (payment == null)
                {
                    throw new Exception($"Payment with ID {id} not found.");
                }

                return payment;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the payment with ID {id}: {ex.Message}");
            }
        }
    }
}
