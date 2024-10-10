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
    public class ReservationRepository: GenericRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    throw new ArgumentException("Invalid userId value.");
                }

                var reservations = await _context.Reservations.Where(r => r.UserId == userId).ToListAsync();

                if (reservations == null || !reservations.Any())
                {
                    throw new Exception($"No reservations found for the user with ID {userId}.");
                }
                return reservations;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving reservations for user with ID {userId}: {ex.Message}");
            }
        }

        public async Task<Reservation> AddReservationWithSeatsAsync(ReservationDTO reservationDto)
        {
            try
            {
                // Validate the ReservationDTO
                if (reservationDto == null)
                {
                    throw new ArgumentNullException(nameof(reservationDto), "ReservationDTO cannot be null.");
                }

                var reservation = new Reservation
                {
                    ShowtimeId = reservationDto.ShowtimeId,
                    ReservationTime = DateTime.Now,
                    UserId = reservationDto.UserId
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                
                var reservationSeats = reservationDto.SeatIds.Select(seatId => new ReservationSeat
                {
                    ReservationId = reservation.Id,
                    SeatId = seatId,
                    ShowtimeId = reservationDto.ShowtimeId
                }).ToList();

                _context.ReservationSeats.AddRange(reservationSeats);
                await _context.SaveChangesAsync();

                return reservation;
            }          
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding the reservation: {ex.Message}");
            }
        }

    }
}
