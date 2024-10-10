using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        public int ShowtimeId { get; set; }
        [Required]
        public Showtime Showtime { get; set; }
        [Required]
        public int SeatId { get; set; }
        [Required]
        public Seat Seat { get; set; }
        [Required]
        public int PaymentId { get; set; }
        [Required]
        public Payment Payment { get; set; }
        [Required]
        public int ReservationId { get; set; }  
        [Required]
        public Reservation Reservation { get; set; }

    }
}
