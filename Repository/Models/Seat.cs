using Biograf_Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Seat
    {
        public int Id { get; set; }
        [Required]
        public string Row { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public bool IsReserved { get; set; }
        [Required]
        public int HallId { get; set; }
        [Required]
        public Hall Hall { get; set; }
        public ICollection<ReservationSeat>? ReservationSeats { get; set; }
    }
}
