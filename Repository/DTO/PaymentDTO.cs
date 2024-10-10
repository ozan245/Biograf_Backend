using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.DTO
{
    public class PaymentDTO
    {
        public int? Id { get; set; }
        public double? Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int UserId { get; set; }
        public List<TicketDTO>? Tickets { get; set; }
    }
}
