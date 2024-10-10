using Biograf_Repository.DTO;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biograf_Repository.DTO.TicketDetails;

namespace Biograf_Repository.Interfaces
{
    public interface ITicketRepository: IGenericRepository<Ticket>
    {

        Task<Ticket?> GetTicketByIdAsync(int id);
        Task<IEnumerable<Ticket>> GetTicketsByUserAsync(int userId);
        Task AddTicketsAsync(TicketDTO ticketDto);
        Task<TicketDetailsDTO> GetTicketDetailsByPaymentIdAsync(int paymentId);
    }
}
