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
    public class SeatRepository: GenericRepository<Seat>, ISeatRepository
    {
        public SeatRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Seat>> GetSeatsByHallIdAsync(int hallId)
        {
            try
            {
                if (hallId <= 0)
                {
                    throw new ArgumentException("Invalid hallId value.");
                }

                var seats = await _context.Seats.Where(s => s.HallId == hallId).ToListAsync();

                if (seats == null || !seats.Any())
                {
                    throw new Exception($"No seats found for the hall with ID {hallId}.");
                }
                return seats;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving seats for the hall with ID {hallId}: {ex.Message}");
            }
        }

        public async Task<Seat> GetSeatByRowAndNumberAsync(string row, int number)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row))
                {
                    throw new ArgumentException("Row cannot be null or empty.");
                }

                if (number <= 0)
                {
                    throw new ArgumentException("Seat number must be greater than zero.");
                }

                var seat = await _context.Seats.FirstOrDefaultAsync(s => s.Row == row && s.Number == number);

                if (seat == null)
                {
                    throw new Exception($"Seat in row '{row}' with number {number} not found.");
                }
                return seat;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the seat in row '{row}' with number {number}: {ex.Message}");
            }
        }

        public async Task DeleteSeatByRowAndNumberAsync(string row, int number)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row))
                {
                    throw new ArgumentException("Row cannot be null or empty.");
                }

                if (number <= 0)
                {
                    throw new ArgumentException("Seat number must be greater than zero.");
                }
                var seat = await GetSeatByRowAndNumberAsync(row, number);
          
                if (seat == null)
                {
                    throw new Exception($"Seat in row '{row}' with number {number} not found.");
                }
                _context.Seats.Remove(seat);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the seat in row '{row}' with number {number}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<SeatDTO>> GetAvailableSeatsByShowtime(int showtimeId)
        {
            try
            {               
                if (showtimeId <= 0)
                {
                    throw new ArgumentException("Invalid showtimeId value.");
                }

                var availableSeats = await _context.Seats.Where(s => !s.IsReserved && s.Hall.Showtimes.Any(st => st.Id == showtimeId)).Select(s => new SeatDTO
                    {
                        Id = s.Id,
                        Row = s.Row,
                        Number = s.Number,
                        IsReserved = s.IsReserved,
                        HallId = s.HallId
                    })
                    .ToListAsync();

                if (availableSeats == null || !availableSeats.Any())
                {
                    throw new Exception($"No available seats found for the showtime with ID {showtimeId}.");
                }
                return availableSeats;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving available seats for the showtime with ID {showtimeId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<SeatDTO>> GetSeatsWithReservationStatus(int hallId, int showtimeId)
        {

            try
            {
                if (hallId <= 0)
                {
                    throw new ArgumentException("Invalid hallId value.");
                }

                if (showtimeId <= 0)
                {
                    throw new ArgumentException("Invalid showtimeId value.");
                }

                var reservedSeatIds = await _context.ReservationSeats.Where(rs => rs.ShowtimeId == showtimeId).Select(rs => rs.SeatId).ToListAsync();
             
                var seats = await _context.Seats.Where(s => s.HallId == hallId).Select(s => new SeatDTO
                    {
                        Id = s.Id,
                        Row = s.Row,
                        Number = s.Number,
                        IsReserved = reservedSeatIds.Contains(s.Id),
                        HallId = s.HallId
                    })
                    .ToListAsync();

                if (seats == null || !seats.Any())
                {
                    throw new Exception($"No seats found for the hall with ID {hallId}.");
                }

                return seats;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving seats for the hall with ID {hallId} and showtime ID {showtimeId}: {ex.Message}");
            }
        }
        public async Task UpdateSeatsAsync(List<SeatDTO> seatDtos)
        {
            try
            {
                if (seatDtos == null || !seatDtos.Any())
                {
                    throw new ArgumentException("SeatDTO list cannot be null or empty.");
                }

                foreach (var seatDto in seatDtos)
                {
                    var seat = await _context.Seats.FindAsync(seatDto.Id);

                    if (seat == null)
                    {
                        throw new Exception($"Seat with ID {seatDto.Id} not found.");
                    }

                    seat.IsReserved = seatDto.IsReserved;
                    _context.Seats.Update(seat);
                }

                await _context.SaveChangesAsync();
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating seats: {ex.Message}");
            }
        }
        public async Task AddReservationSeatsAsync(List<ReservationSeat> reservationSeats)
        {
            try
            {  
                if (reservationSeats == null || !reservationSeats.Any())
                {
                    throw new ArgumentException("ReservationSeats list cannot be null or empty.");
                }

                _context.ReservationSeats.AddRange(reservationSeats);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding reservation seats: {ex.Message}");
            }
        }

    }
}
