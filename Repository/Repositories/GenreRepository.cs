using Biograf_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Repositories
{
    public class GenreRepository: GenericRepository<Genre>, IGenreRepository
    {
        public GenreRepository(DataContext context) : base(context) { }
    }
}
