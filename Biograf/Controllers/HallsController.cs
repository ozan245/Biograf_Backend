using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biograf_Repository.Models;
using Repository.Data;
using AutoMapper;
using Biograf_Repository.Interfaces;
using Biograf_Repository.DTO;
using Biograf_Repository.Repositories;

namespace Biograf_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallsController : ControllerBase
    {
        private readonly IGenericRepository<Hall> _hallRepository;
        private readonly IHallRepository _ihallRepository;
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public HallsController(IGenericRepository<Hall> hallRepository, DataContext context, IMapper mapper, IHallRepository ihallRepository)
        {
            _hallRepository = hallRepository;
            _mapper = mapper;
            _context = context;
            _ihallRepository = ihallRepository;
        }

        [HttpGet("GetAllHalls")]
        public async Task<ActionResult<IEnumerable<HallDTO>>> GetHalls()
        {
            try
            {
                var halls = await _context.Halls.Include(h => h.Seats).ToListAsync();
             
                if (halls == null || !halls.Any())
                {
                    return NotFound("No halls available.");
                }

                var hallDtos = _mapper.Map<IEnumerable<HallDTO>>(halls);
                return Ok(hallDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetHallById{id}")]
        public async Task<ActionResult<HallDTO>> GetHall(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid hall ID.");
                }

                var hall = await _hallRepository.GetByIdAsync(id);
                if (hall == null)
                {
                    return NotFound($"Hall with ID {id} not found.");
                }

                var hallDto = _mapper.Map<HallDTO>(hall);
                return Ok(hallDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetHallIdByShowtimeId/{showtimeId}")]
        public async Task<IActionResult> GetHallIdByShowtimeId(int showtimeId)
        {
            try
            {             
                var hallId = await _ihallRepository.GetHallIdByShowtimeIdAsync(showtimeId);

                if (hallId == null)
                {
                    return NotFound("No hall found for the specified showtime.");
                }
                return Ok(hallId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddHall")]
        public async Task<ActionResult> AddHall(HallDTO hallDto)
        {
            try
            {
                if (hallDto == null)
                {
                    return BadRequest("Hall data cannot be null.");
                }

                var hall = _mapper.Map<Hall>(hallDto);
                await _hallRepository.AddAsync(hall);

                return CreatedAtAction(nameof(GetHall), new { id = hall.Id }, hallDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateHallById/{id}")]
        public async Task<ActionResult> UpdateHall(int id, HallDTO hallDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid hall ID.");
                }

                if (hallDto == null)
                {
                    return BadRequest("Hall data cannot be null.");
                }

                var existingHall = await _hallRepository.GetByIdAsync(id);
                if (existingHall == null)
                {
                    return NotFound($"Hall with ID {id} not found.");
                }

                _mapper.Map(hallDto, existingHall);
                await _hallRepository.UpdateAsync(existingHall);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteHallById/{id}")]
        public async Task<ActionResult> DeleteHall(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid hall ID.");
                }

                var hall = await _hallRepository.GetByIdAsync(id);
                if (hall == null)
                {
                    return NotFound($"Hall with ID {id} not found.");
                }

                await _hallRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteHallByName/{name}")]
        public async Task<IActionResult> DeleteHallByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Hall name cannot be null or empty.");
                }
                await _hallRepository.DeleteByNameAsync(name);
                return Ok(new { message = $"Hall with name '{name}' has been deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateHallByName/{name}")]
        public async Task<IActionResult> UpdateHallByName(string name, [FromBody] HallDTO hallDto)
        {
            try
            {               
                var existingHall = await _hallRepository.GetByNameAsync(name);

                if (existingHall == null)
                {
                    return NotFound(new { message = $"Hall with name '{name}' not found." });
                }
               
                existingHall.Name = hallDto.Name;
                existingHall.Capacity = hallDto.Capacity;
                existingHall.CinemaId = hallDto.CinemaId;
              
                await _hallRepository.UpdateAsync(existingHall);

                return Ok(new { message = $"Hall with name '{name}' has been updated." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error while updating the hall: {ex.Message}" });
            }
        }

    }
}
