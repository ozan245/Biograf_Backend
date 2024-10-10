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
    public class SeatsController : ControllerBase
    {
        private readonly IGenericRepository<Seat> _seatRepository;
        private readonly ISeatRepository _iseatRepository;
        private readonly IMapper _mapper;

        public SeatsController(IGenericRepository<Seat> seatRepository, IMapper mapper, ISeatRepository iseatrepository)
        {
            _seatRepository = seatRepository;
            _iseatRepository = iseatrepository;
            _mapper = mapper;
        }

        [HttpGet("GetAllSeats")]
        public async Task<ActionResult<IEnumerable<SeatDTO>>> GetSeats()
        {
            try
            {
                var seats = await _seatRepository.GetAllAsync();

                if (seats == null || !seats.Any())
                {
                    return NotFound("No seats available.");
                }
                var seatDtos = _mapper.Map<IEnumerable<SeatDTO>>(seats);
                return Ok(seatDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetSeatById/{id}")]
        public async Task<ActionResult<SeatDTO>> GetSeat(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid seat ID.");
                }

                var seat = await _seatRepository.GetByIdAsync(id);
                if (seat == null)
                {
                    return NotFound($"Seat with ID {id} not found.");
                }
                var seatDto = _mapper.Map<SeatDTO>(seat);
                return Ok(seatDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAvailableSeatsByShowtimeId/{showtimeId}")]
        public async Task<IActionResult> GetAvailableSeats(int showtimeId)
        {
            try
            {
                if (showtimeId <= 0)
                {
                    return BadRequest("Invalid showtime ID.");
                }

                var seats = await _iseatRepository.GetAvailableSeatsByShowtime(showtimeId);
             
                if (seats == null || !seats.Any())
                {
                    return NotFound("No available seats found for this showtime.");
                }
                return Ok(seats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetSeatsByHall/{hallId}/Showtime/{showtimeId}")]
        public async Task<IActionResult> GetSeatsByHallAndShowtime(int hallId, int showtimeId)
        {
            try
            {
                if (hallId <= 0)
                {
                    return BadRequest("Invalid hall ID.");
                }

                if (showtimeId <= 0)
                {
                    return BadRequest("Invalid showtime ID.");
                }

                var seats = await _iseatRepository.GetSeatsWithReservationStatus(hallId, showtimeId);

                if (seats == null || !seats.Any())
                { 
                    return NotFound("No seats found for the specified hall and showtime.");
                }
                return Ok(seats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ReserveSeats")]
        public async Task<IActionResult> ReserveSeats([FromBody] List<ReservationSeat> reservationSeats)
        {
            try
            {
                if (reservationSeats == null || !reservationSeats.Any())
                {
                    return BadRequest("No seats provided for reservation.");
                }
                await _iseatRepository.AddReservationSeatsAsync(reservationSeats);
                return Ok("Seats reserved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetSeatsByHallId/{hallId}")]
        public async Task<ActionResult<IEnumerable<SeatDTO>>> GetSeatsByHall(int hallId)
        {
            try
            {
                if (hallId <= 0)
                {
                    return BadRequest("Invalid hall ID.");
                }

                var seats = await _iseatRepository.GetSeatsByHallIdAsync(hallId);

                if (seats == null || !seats.Any())
                {
                    return NotFound("No seats found for this hall.");
                }
                var seatDtos = _mapper.Map<IEnumerable<SeatDTO>>(seats);
                return Ok(seatDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddSeat")]
        public async Task<ActionResult> AddSeat(SeatDTO seatDto)
        {
            try
            {
                if (seatDto == null)
                {
                    return BadRequest("Seat data cannot be null.");
                }
                var seat = _mapper.Map<Seat>(seatDto);
                await _seatRepository.AddAsync(seat);
                return CreatedAtAction(nameof(GetSeat), new { id = seat.Id }, seatDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateSeatById/{id}")]
        public async Task<ActionResult> UpdateSeat(int id, SeatDTO seatDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid seat ID.");
                }

                if (seatDto == null)
                {
                    return BadRequest("Seat data cannot be null.");
                }

                var existingSeat = await _seatRepository.GetByIdAsync(id);
                if (existingSeat == null)
                {
                    return NotFound($"Seat with ID {id} not found.");
                }
                _mapper.Map(seatDto, existingSeat);
                await _seatRepository.UpdateAsync(existingSeat);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteSeatByRowByNumber")]
        public async Task<IActionResult> DeleteSeat(string row, int number)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row) || number <= 0)
                {
                    return BadRequest("Invalid seat row or number.");
                }

                var seat = await _iseatRepository.GetSeatByRowAndNumberAsync(row, number);
                if (seat == null)
                {
                    return NotFound($"Seat with Row {row} and Number {number} not found.");
                }
                await _iseatRepository.DeleteSeatByRowAndNumberAsync(row, number);
                return Ok("Seat deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteSeatById/{id}")]
        public async Task<ActionResult> DeleteSeat(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid seat ID.");
                }

                var seat = await _seatRepository.GetByIdAsync(id);
                if (seat == null)
                {
                    return NotFound($"Seat with ID {id} not found.");
                }
                await _seatRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
