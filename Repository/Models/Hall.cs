using Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Models
{
    public class Hall
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int CinemaId { get; set; }
        [Required]
        public Cinema Cinema { get; set; }     
        public ICollection<Showtime>? Showtimes { get; set; }       
        public ICollection<Seat>? Seats { get; set; }
    }
}
