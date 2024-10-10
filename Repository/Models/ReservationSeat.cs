using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class ReservationSeat
    {
        public int ReservationId { get; set; }
        [Required]
        public Reservation Reservation { get; set; }
        [Required]
        public int SeatId { get; set; }
        [Required]
        public Seat Seat { get; set; }
        [Required]
        public int ShowtimeId { get; set; }
        [Required]
        public Showtime Showtime { get; set; }
    }
}
