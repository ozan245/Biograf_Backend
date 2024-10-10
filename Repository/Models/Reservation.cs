using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        [Required]
        public DateTime ReservationTime { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public int ShowtimeId { get; set; }
        [Required]
        public Showtime Showtime { get; set; }
        public ICollection<ReservationSeat>? ReservationSeats { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }
    }
}
