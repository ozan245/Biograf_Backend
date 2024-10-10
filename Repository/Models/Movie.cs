using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }       
        public string? Description { get; set; }
        [Required]
        public int Duration { get; set; }       
        public string? ImagePath { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [JsonIgnore]
        public ICollection<MovieGenre>? MovieGenres { get; set; }     
        public ICollection<Showtime>? Showtimes { get; set; }
    }
}
