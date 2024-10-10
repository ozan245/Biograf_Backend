using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Payment
    {
        public int Id { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public ICollection<Ticket>? Tickets { get; set; }
    }
}
