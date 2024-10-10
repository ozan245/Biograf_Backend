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
using static Biograf_Repository.DTO.TicketDetails;

namespace Biograf_Repository.Repositories
{
    public class TicketRepository: GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(DataContext context) : base(context) { }

        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Invalid ticket ID value.");
                }

                var ticket = await _context.Tickets.Include(t => t.Showtime).ThenInclude(s => s.Movie).FirstOrDefaultAsync(t => t.Id == id);
     
                if (ticket == null)
                {
                    throw new Exception($"Ticket with ID {id} not found.");
                }
                return ticket;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the ticket with ID {id}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    throw new ArgumentException("Invalid user ID value.");
                }

                var tickets = await _context.Tickets.Include(t => t.Showtime).ThenInclude(s => s.Movie).Where(t => t.Payment.UserId == userId).ToListAsync();

                if (tickets == null || !tickets.Any())
                {
                    throw new Exception($"No tickets found for user ID {userId}.");
                }
                return tickets;
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving tickets for user ID {userId}: {ex.Message}");
            }
        }

        public async Task AddTicketsAsync(TicketDTO ticketDto)
        {
            try
            {
                if (ticketDto == null)
                {
                    throw new ArgumentNullException(nameof(ticketDto), "TicketDTO cannot be null.");
                }             

                foreach (var seatId in ticketDto.SeatIds)
                {
                    var ticket = new Ticket
                    {
                        PurchaseDate = ticketDto.PurchaseDate,
                        ShowtimeId = ticketDto.ShowtimeId,
                        SeatId = seatId,
                        PaymentId = ticketDto.PaymentId,
                        ReservationId = ticketDto.ReservationId
                    };

                    _context.Tickets.Add(ticket);
                }

                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }            
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while adding tickets: {ex.Message}");
            }
        }

        public async Task<TicketDetailsDTO> GetTicketDetailsByPaymentIdAsync(int paymentId)
        {
            try
            {
                if (paymentId <= 0)
                {
                    throw new ArgumentException("Invalid payment ID value.");
                }

                var ticket = await _context.Tickets
                    .Include(t => t.Reservation)
                    .ThenInclude(r => r.ReservationSeats)
                    .ThenInclude(rs => rs.Seat)
                    .Include(t => t.Showtime)
                    .ThenInclude(s => s.Hall)
                    .ThenInclude(h => h.Cinema)
                    .Include(t => t.Showtime.Movie)
                    .FirstOrDefaultAsync(t => t.PaymentId == paymentId);

                if (ticket == null)
                {
                    throw new Exception($"No ticket found for payment ID {paymentId}.");
                }

                return new TicketDetailsDTO
                {
                    PurchaseDate = ticket.PurchaseDate,
                    CinemaName = ticket.Showtime.Hall.Cinema.Name,
                    HallName = ticket.Showtime.Hall.Name,
                    ShowtimeTime = ticket.Showtime.Time,
                    Seats = ticket.Reservation.ReservationSeats.Select(rs => rs.Seat.Row + rs.Seat.Number).ToList(),
                    MovieTitle = ticket.Showtime.Movie.Title
                };
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving ticket details for payment ID {paymentId}: {ex.Message}");
            }
        }

    }
}
