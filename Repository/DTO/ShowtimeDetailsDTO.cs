using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.DTO
{
    public class ShowtimeDetailsDTO
    {
        public int ShowtimeId { get; set; }
        public string MovieTitle { get; set; }  
        public string CinemaName { get; set; }  
        public string HallName { get; set; }  
        public DateTime Time { get; set; }
    }
}
