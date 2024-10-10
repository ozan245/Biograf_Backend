using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.DTO
{
    public class TicketDetails
    {
        public class TicketDetailsDTO
        {             
            public DateTime PurchaseDate { get; set; }
            public string CinemaName { get; set; }
            public string HallName { get; set; }
            public string MovieTitle { get; set; }
            public DateTime ShowtimeTime { get; set; }
            public List<string> Seats { get; set; }           
        }

    }
}
