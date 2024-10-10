using Biograf_Repository.DTO;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Interfaces
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        Task<IEnumerable<Seat>> GetSeatsByHallIdAsync(int hallId);
        Task<Seat> GetSeatByRowAndNumberAsync(string row, int number);
        Task DeleteSeatByRowAndNumberAsync(string row, int number);
        Task<IEnumerable<SeatDTO>> GetAvailableSeatsByShowtime(int showtimeId);
        Task UpdateSeatsAsync(List<SeatDTO> seatDtos);
        Task AddReservationSeatsAsync(List<ReservationSeat> reservationSeats);
        Task<IEnumerable<SeatDTO>> GetSeatsWithReservationStatus(int hallId, int showtimeId);
    }
}
