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
    public class Showtime
    {
        public int Id { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public int MovieId { get; set; }
        [Required]
        public Movie Movie { get; set; }
        [Required]
        public int HallId { get; set; }
        [Required]
        public Hall Hall { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }       
        public ICollection<ReservationSeat>? ReservationSeats { get; set; }
    }
}
