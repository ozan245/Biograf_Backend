using Biograf_Repository.DTO;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Interfaces
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId);
        Task<Reservation> AddReservationWithSeatsAsync(ReservationDTO reservationDto);
    }
}
