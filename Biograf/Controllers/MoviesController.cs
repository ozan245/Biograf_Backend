using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Biograf_Repository;
using Biograf_Repository.DTO;
using AutoMapper;
using Biograf_Repository.Interfaces;
using Biograf_Repository.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Biograf_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IGenericRepository<Movie> _genericMovieRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MoviesController(IGenericRepository<Movie> genericMovieRepository, IMovieRepository movieRepository, IMapper mapper, DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _genericMovieRepository = genericMovieRepository;
            _movieRepository = movieRepository;
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetMoviesWithGenres")]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMoviesWithGenres()
        {
            try
            {
                var movies = await _movieRepository.GetMoviesWithGenresAsync();
               
                if (movies == null || !movies.Any())
                {
                    return NotFound("No movies with genres available.");
                }

                var movieDtos = _mapper.Map<IEnumerable<MovieDTO>>(movies);
                return Ok(movieDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }      

        [HttpGet("GetAllMovies")]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetAllMovies()
        {
            try
            {
                var movies = await _genericMovieRepository.GetAllAsync();

                if (movies == null || !movies.Any())
                {
                    return NotFound("No movies available.");
                }
                var movieDtos = _mapper.Map<IEnumerable<MovieDTO>>(movies);
                return Ok(movieDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       
        [HttpGet("(\"GetMovieById\"){id}")]
        public async Task<ActionResult<MovieDTO>> GetMovieById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid movie ID.");
                }

                var movie = await _genericMovieRepository.GetByIdAsync(id);
                if (movie == null)
                {
                    return NotFound($"Movie with ID {id} not found.");
                }
                var movieDto = _mapper.Map<MovieDTO>(movie);
                return Ok(movieDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetActiveMovies")]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetActiveMovies()
        {
            try
            {
                var activeMovies = await _movieRepository.GetActiveMoviesAsync();

                if (activeMovies == null || !activeMovies.Any())
                {
                    return NotFound("No active movies available.");
                }
                var movieDtos = _mapper.Map<IEnumerable<MovieDTO>>(activeMovies);
                return Ok(movieDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetActiveMoviesByGenre/{genreId}")]
        public async Task<IActionResult> GetActiveMoviesByGenre(int genreId)
        {
            try
            {
                if (genreId <= 0)
                {
                    return BadRequest("Invalid genre ID.");
                }

                var movies = await _movieRepository.GetActiveMoviesByGenreAsync(genreId);

                if (movies == null || !movies.Any())
                {
                    return NotFound("No active movies found for the selected genre.");
                }
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddMovie")]
        public async Task<IActionResult> AddMovie([FromForm] MovieDTO movieDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var movie = new Movie
                {
                    Title = movieDto.Title,
                    Description = movieDto.Description,
                    Duration = movieDto.Duration,
                    IsActive = movieDto.IsActive
                };

                if (movieDto.ImageFile != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(movieDto.ImageFile.FileName);
                    var extension = Path.GetExtension(movieDto.ImageFile.FileName);
                    var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await movieDto.ImageFile.CopyToAsync(fileStream);
                    }
                    movie.ImagePath = $"/images/{uniqueFileName}";
                }
                await _genericMovieRepository.AddAsync(movie);
                await _context.SaveChangesAsync();

                if (movieDto.GenreIds != null && movieDto.GenreIds.Any())
                {
                    foreach (var genreId in movieDto.GenreIds)
                    {
                        var existingGenre = await _context.Genres.FindAsync(genreId);
                        if (existingGenre == null)
                        {
                            return BadRequest($"Genre with ID {genreId} not found.");
                        }

                        var newMovieGenre = new MovieGenre
                        {
                            MovieId = movie.Id,
                            GenreId = genreId
                        };
                        _context.MovieGenres.Add(newMovieGenre);
                    }
                    await _context.SaveChangesAsync();
                }
                return Ok(new { message = "Movie added successfully", movie });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       
        [HttpPut("(\"UpdateMovie\"){id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromForm] MovieDTO movieDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid movie ID.");
                }

                var existingMovie = await _genericMovieRepository.GetByIdAsync(id);
                if (existingMovie == null)
                {
                    return NotFound($"Movie with ID {id} not found.");
                }

                existingMovie.Title = movieDto.Title;
                existingMovie.Description = movieDto.Description;
                existingMovie.Duration = movieDto.Duration;
                existingMovie.IsActive = movieDto.IsActive;

                if (movieDto.ImageFile != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(movieDto.ImageFile.FileName);
                    var extension = Path.GetExtension(movieDto.ImageFile.FileName);
                    var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await movieDto.ImageFile.CopyToAsync(fileStream);
                    }

                    existingMovie.ImagePath = $"/images/{uniqueFileName}";
                }
                await _genericMovieRepository.UpdateAsync(existingMovie);

                var existingMovieGenres = _context.MovieGenres.Where(mg => mg.MovieId == id).ToList();
                _context.MovieGenres.RemoveRange(existingMovieGenres);
                await _context.SaveChangesAsync();

                if (movieDto.GenreIds != null && movieDto.GenreIds.Any())
                {
                    foreach (var genreId in movieDto.GenreIds)
                    {
                        var existingGenre = await _context.Genres.FindAsync(genreId);
                        if (existingGenre == null)
                        {
                            return BadRequest($"Genre with ID {genreId} not found.");
                        }

                        var newMovieGenre = new MovieGenre
                        {
                            MovieId = existingMovie.Id,
                            GenreId = genreId
                        };
                        _context.MovieGenres.Add(newMovieGenre);
                    }
                    await _context.SaveChangesAsync();
                }
                return Ok(new { message = "Movie updated successfully", movie = existingMovie });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("UpdateMovieByTitle/{title}")]
        public async Task<IActionResult> UpdateMovie(string title, [FromForm] MovieDTO movieDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    return BadRequest("Movie title cannot be null or empty.");
                }

                if (movieDto == null)
                {
                    return BadRequest("Movie data cannot be null.");
                }

                var existingMovie = await _movieRepository.GetMovieByTitleAsync(title);
                if (existingMovie == null)
                {
                    return NotFound($"Movie with title '{title}' not found.");
                }

                existingMovie.Title = movieDto.Title;
                existingMovie.Description = movieDto.Description;
                existingMovie.Duration = movieDto.Duration;
                existingMovie.IsActive = movieDto.IsActive;

                if (movieDto.ImageFile != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(movieDto.ImageFile.FileName);
                    var extension = Path.GetExtension(movieDto.ImageFile.FileName);
                    var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await movieDto.ImageFile.CopyToAsync(fileStream);
                    }

                    existingMovie.ImagePath = $"/images/{uniqueFileName}";
                }
                await _movieRepository.UpdateAsync(existingMovie);

                var existingMovieGenres = _context.MovieGenres.Where(mg => mg.MovieId == existingMovie.Id).ToList();
                _context.MovieGenres.RemoveRange(existingMovieGenres);
                await _context.SaveChangesAsync();

                if (movieDto.GenreIds != null && movieDto.GenreIds.Any())
                {
                    foreach (var genreId in movieDto.GenreIds)
                    {
                        var existingGenre = await _context.Genres.FindAsync(genreId);
                        if (existingGenre == null)
                        {
                            return BadRequest($"Genre with Id {genreId} not found.");
                        }

                        var newMovieGenre = new MovieGenre
                        {
                            MovieId = existingMovie.Id,
                            GenreId = genreId
                        };
                        _context.MovieGenres.Add(newMovieGenre);
                    }
                    await _context.SaveChangesAsync();
                }
                return Ok(new { message = "Movie updated successfully", movie = existingMovie });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("SearchMovieByTitle/{title}")]
        public async Task<IActionResult> SearchByTitle(string title)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    return BadRequest("Title cannot be null or empty.");
                }

                var movies = await _movieRepository.SearchMoviesByTitleAsync(title);
              
                if (movies == null || !movies.Any())
                {
                    return NotFound("No movies found with the given title.");
                }
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("DeleteMovieById/{id}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid movie ID.");
                }

                var movie = await _genericMovieRepository.GetByIdAsync(id);
                if (movie == null)
                {
                    return NotFound($"Movie with ID {id} not found.");
                }

                await _genericMovieRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteMovieByTitle/{title}")]
        public async Task<IActionResult> DeleteMovieByTitle(string title)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    return BadRequest("Movie title cannot be null or empty.");
                }
                await _movieRepository.DeleteMovieByTitleAsync(title);
                return Ok(new { message = $"Movie with title '{title}' has been deleted." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
