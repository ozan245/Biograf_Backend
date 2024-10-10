using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }       
        public ICollection<MovieGenre>? MovieGenres { get; set; }
    }
}
