using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.DTO
{
    public class ShowtimeDTO
    {
        public DateTime Time { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public List<int>? SeatIds { get; set; }
    }
}
