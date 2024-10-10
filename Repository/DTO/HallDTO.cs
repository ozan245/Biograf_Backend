using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.DTO
{
    public class HallDTO
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int CinemaId { get; set; }
        public List<SeatDTO>? Seats { get; set; }
    }
}
