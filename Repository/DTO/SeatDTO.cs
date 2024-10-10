using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.DTO
{
    public class SeatDTO
    {
        public int? Id { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public bool IsReserved { get; set; }
        public int HallId { get; set; }
    }
}
