using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.DTO
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public DateTime ReservationTime { get; set; }
        public List<int> SeatIds { get; set; }

    }
}
