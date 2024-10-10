using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biograf_Repository.DTO;
using Biograf_Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;

namespace Biograf_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimesController : ControllerBase
    {
        private readonly IGenericRepository<Showtime> _showtimeRepository;
        private readonly IShowtimeRepository _ishowtimeRepository;
        private readonly IMapper _mapper;

        public ShowtimesController(IGenericRepository<Showtime> showtimeRepository, IMapper mapper, IShowtimeRepository ishowtimeRepository)
        {
            _showtimeRepository = showtimeRepository;
            _mapper = mapper;
            _ishowtimeRepository = ishowtimeRepository;
        }

        [HttpGet("GetAllShowtimes")]
        public async Task<ActionResult<IEnumerable<ShowtimeDTO>>> GetShowtimes()
        {
            try
            {
                var showtimes = await _showtimeRepository.GetAllAsync();
            
                if (showtimes == null || !showtimes.Any())
                {
                    return NotFound("No showtimes available.");
                }
                var showtimeDtos = _mapper.Map<IEnumerable<ShowtimeDTO>>(showtimes);
                return Ok(showtimeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetShowtimeDetailsByShowtimeId/{showtimeId}")]
        public async Task<IActionResult> GetShowtimeDetails(int showtimeId)
        {
            try
            {
                if (showtimeId <= 0)
                {
                    return BadRequest("Invalid showtime ID.");
                }

                var showtimeDetails = await _ishowtimeRepository.GetShowtimeDetailsAsync(showtimeId);
                if (showtimeDetails == null)
                {
                    return NotFound($"Showtime with ID {showtimeId} not found.");
                }
                return Ok(showtimeDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetShowtimeById/{id}")]
        public async Task<ActionResult<ShowtimeDTO>> GetShowtime(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid showtime ID.");
                }

                var showtime = await _showtimeRepository.GetByIdAsync(id);
                if (showtime == null)
                {
                    return NotFound($"Showtime with ID {id} not found.");
                }
                var showtimeDto = _mapper.Map<ShowtimeDTO>(showtime);
                return Ok(showtimeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetShowtimessByMovies/{movieId}/Cinemas/{cinemaId}/Showtimes")]
        public async Task<ActionResult<IEnumerable<Showtime>>> GetShowtimesByMovieAndCinema(int movieId, int cinemaId)
        {
            try
            {
                if (movieId <= 0)
                {
                    return BadRequest("Invalid movie ID.");
                }

                if (cinemaId <= 0)
                {
                    return BadRequest("Invalid cinema ID.");
                }

                var showtimes = await _ishowtimeRepository.GetShowtimesByMovieAndCinema(movieId, cinemaId);

                if (showtimes == null || !showtimes.Any())
                {
                    return NotFound("No showtimes found for the selected movie and cinema.");
                }
                return Ok(showtimes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddShowtime")]
        public async Task<ActionResult> AddShowtime(ShowtimeDTO showtimeDto)
        {
            try
            {
                if (showtimeDto == null)
                {
                    return BadRequest("Showtime data cannot be null.");
                }

                var showtime = _mapper.Map<Showtime>(showtimeDto);
                await _showtimeRepository.AddAsync(showtime);
                return CreatedAtAction(nameof(GetShowtime), new { id = showtime.Id }, showtimeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateShowtimeById/{id}")]
        public async Task<ActionResult> UpdateShowtime(int id, ShowtimeDTO showtimeDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid showtime ID.");
                }

                if (showtimeDto == null)
                {
                    return BadRequest("Showtime data cannot be null.");
                }

                var existingShowtime = await _showtimeRepository.GetByIdAsync(id);
                if (existingShowtime == null)
                {
                    return NotFound($"Showtime with ID {id} not found.");
                }
                _mapper.Map(showtimeDto, existingShowtime);
                await _showtimeRepository.UpdateAsync(existingShowtime);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteShowtimeById/{id}")]
        public async Task<ActionResult> DeleteShowtime(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid showtime ID.");
                }

                var showtime = await _showtimeRepository.GetByIdAsync(id);
                if (showtime == null)
                {
                    return NotFound($"Showtime with ID {id} not found.");
                }
                await _showtimeRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
