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
    public class GenresController : ControllerBase
    {
        private readonly IGenericRepository<Genre> _genreRepository;
        private readonly IMapper _mapper;

        public GenresController(IGenericRepository<Genre> genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAllGenres")]
        public async Task<ActionResult<IEnumerable<GenreDTO>>> GetGenres()
        {
            try
            {
                var genres = await _genreRepository.GetAllAsync();

                if (genres == null || !genres.Any())
                {
                    return NotFound("No genres available.");
                }

                var genreDtos = _mapper.Map<IEnumerable<GenreDTO>>(genres);
                return Ok(genreDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetGenreById/{id}")]
        public async Task<ActionResult<GenreDTO>> GetGenre(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid genre ID.");
                }

                var genre = await _genreRepository.GetByIdAsync(id);
                if (genre == null)
                {
                    return NotFound($"Genre with ID {id} not found.");
                }

                var genreDto = _mapper.Map<GenreDTO>(genre);
                return Ok(genreDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddGenre")]
        public async Task<ActionResult> AddGenre(GenreDTO genreDto)
        {
            try
            {
                if (genreDto == null)
                {
                    return BadRequest("Genre data cannot be null.");
                }

                var genre = _mapper.Map<Genre>(genreDto);
                await _genreRepository.AddAsync(genre);

                return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genreDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateGenreById/{id}")]
        public async Task<ActionResult> UpdateGenre(int id, GenreDTO genreDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid genre ID.");
                }

                if (genreDto == null)
                {
                    return BadRequest("Genre data cannot be null.");
                }

                var existingGenre = await _genreRepository.GetByIdAsync(id);
                if (existingGenre == null)
                {
                    return NotFound($"Genre with ID {id} not found.");
                }
                _mapper.Map(genreDto, existingGenre);
                await _genreRepository.UpdateAsync(existingGenre);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteGenreById/{id}")]
        public async Task<ActionResult> DeleteGenre(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid genre ID.");
                }

                var genre = await _genreRepository.GetByIdAsync(id);
                if (genre == null)
                {
                    return NotFound($"Genre with ID {id} not found.");
                }
                await _genreRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("DeleteGenreByName/{name}")]
        public async Task<IActionResult> DeleteGenreByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Genre name cannot be null or empty.");
                }
                await _genreRepository.DeleteByNameAsync(name);
                return Ok(new { message = $"Genre with name '{name}' has been deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("UpdateGenreByName/{name}")]
        public async Task<IActionResult> UpdateGenreByName(string name, [FromBody] GenreDTO genreDto)
        {
            try
            {
                var existingGenre = await _genreRepository.GetByNameAsync(name);

                if (existingGenre == null)
                {
                    return NotFound(new { message = $"Genre with name '{name}' not found." });
                }

                existingGenre.Name = genreDto.Name;

                await _genreRepository.UpdateAsync(existingGenre);

                return Ok(new { message = $"Genre with name '{name}' has been updated." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while updating the genre: {ex.Message}" });
            }
        }
    }
}
