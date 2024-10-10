using Biograf_Repository.DTO;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment> AddPaymentAsync(PaymentDTO paymentDto);
        Task<Payment?> GetPaymentByIdAsync(int id);
    }
}
