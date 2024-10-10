using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.DTO
{
    public class MovieDTO
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Duration { get; set; }       
        public bool IsActive { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public List<int> GenreIds { get; set; }
        public string? ImagePath { get; set; }

    }
}
