using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class MovieGenre
    {
        [Required]
        public int MovieId { get; set; }
        [Required]
        public Movie Movie { get; set; }
        [Required]
        public int GenreId { get; set; }
        [Required]
        public Genre Genre { get; set; }
    }
}
