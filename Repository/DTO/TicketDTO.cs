using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.DTO
{
    public class TicketDTO
    {
        public int? Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int ShowtimeId { get; set; }      
        public List<int> SeatIds { get; set; }
        public int PaymentId { get; set; }
        public int ReservationId { get; set; }
    }
}
