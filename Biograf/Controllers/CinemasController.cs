using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biograf_Repository.DTO;
using Biograf_Repository.Interfaces;
using Biograf_Repository.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;

namespace Biograf_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CinemasController : ControllerBase
    {
        private readonly IGenericRepository<Cinema> _cinemaRepository;
        private readonly ICinemaRepository _icinemaRepository;
        private readonly IMapper _mapper;

        public CinemasController(IGenericRepository<Cinema> cinemaRepository, IMapper mapper, ICinemaRepository icinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
            _mapper = mapper;
            _icinemaRepository = icinemaRepository;
        }


        [HttpGet("GetAllCinemas")]
        public async Task<ActionResult<IEnumerable<CinemaDTO>>> GetCinemas()
        {
            try
            {
                var cinemas = await _cinemaRepository.GetAllAsync();
           
                if (cinemas == null || !cinemas.Any())
                {
                    return NotFound("No cinemas available.");
                }

                var cinemaDtos = _mapper.Map<IEnumerable<CinemaDTO>>(cinemas);
                return Ok(cinemaDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetCinemaById/{id}")]
        public async Task<ActionResult<CinemaDTO>> GetCinema(int id)
        {
            try
            {                
                if (id <= 0)
                {
                    return BadRequest("Invalid cinema ID.");
                }

                var cinema = await _cinemaRepository.GetByIdAsync(id);
                if (cinema == null)
                {
                    return NotFound($"Cinema with ID {id} not found.");
                }

                var cinemaDto = _mapper.Map<CinemaDTO>(cinema);
                return Ok(cinemaDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetCinemasByMovieId/{movieId}")]
        //[HttpGet("movie/{movieId}/cinemas")]
        public async Task<ActionResult<IEnumerable<Cinema>>> GetCinemasByMovie(int movieId)
        {
            try
            {
                if (movieId <= 0)
                {
                    return BadRequest("Invalid movie ID.");
                }

                var cinemas = await _icinemaRepository.GetCinemasByMovie(movieId);

                if (cinemas == null || !cinemas.Any())
                {
                    return NotFound($"No cinemas found for movie with ID {movieId}.");
                }

                return Ok(cinemas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("AddCinema")]
        public async Task<ActionResult> AddCinema(CinemaDTO cinemaDto)
        {
            try
            {               
                if (cinemaDto == null)
                {
                    return BadRequest("Cinema data cannot be null.");
                }

                var cinema = _mapper.Map<Cinema>(cinemaDto);
                await _cinemaRepository.AddAsync(cinema);

                return CreatedAtAction(nameof(GetCinema), new { id = cinema.Id }, cinemaDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateCinemaById/{id}")]
        public async Task<ActionResult> UpdateCinema(int id, CinemaDTO cinemaDto)
        {
            try
            {              
                if (id <= 0)
                {
                    return BadRequest("Invalid cinema ID.");
                }
             
                if (cinemaDto == null)
                {
                    return BadRequest("Cinema data cannot be null.");
                }

                var existingCinema = await _cinemaRepository.GetByIdAsync(id);
                if (existingCinema == null)
                {
                    return NotFound($"Cinema with ID {id} not found.");
                }

                _mapper.Map(cinemaDto, existingCinema);
                await _cinemaRepository.UpdateAsync(existingCinema);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteCinemaById/{id}")]
        public async Task<ActionResult> DeleteCinema(int id)
        {
            try
            {              
                if (id <= 0)
                {
                    return BadRequest("Invalid cinema ID.");
                }

                var cinema = await _cinemaRepository.GetByIdAsync(id);
                if (cinema == null)
                {
                    return NotFound($"Cinema with ID {id} not found.");
                }

                await _cinemaRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteCinemaByName/{name}")]
        public async Task<IActionResult> DeleteCinemaByName(string name)
        {
            try
            {               
                await _cinemaRepository.DeleteByNameAsync(name);
                return Ok(new { message = $"Cinema with name '{name}' has been deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while deleting cinema: {ex.Message}" });
            }
        }

        [HttpPut("UpdateCinemaByName/{name}")]
        public async Task<IActionResult> UpdateCinemaByName(string name, [FromBody] CinemaDTO cinemaDto)
        {
            try
            {
                var existingCinema = await _cinemaRepository.GetByNameAsync(name);

                if (existingCinema == null)
                {
                    return NotFound(new { message = $"Cinema with name '{name}' not found." });
                }

               
                existingCinema.Name = cinemaDto.Name;
                existingCinema.Location= cinemaDto.Location;

                await _cinemaRepository.UpdateAsync(existingCinema);

                return Ok(new { message = $"Cinema with name '{name}' has been updated." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while updating the cinema: {ex.Message}" });
            }
        }

    }
}
