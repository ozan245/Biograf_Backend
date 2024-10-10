using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    public class ReservationsController : ControllerBase
    {
        private readonly IGenericRepository<Reservation> _reservationRepository;
        private readonly IReservationRepository _ireservationRepository;
        private readonly IMapper _mapper;

        public ReservationsController(IGenericRepository<Reservation> reservationRepository, IMapper mapper, IReservationRepository ireservationRepository)
        {
            _reservationRepository = reservationRepository;
            _ireservationRepository = ireservationRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAllReservations")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations()
        {
            try
            {
                var reservations = await _reservationRepository.GetAllAsync();

                if (reservations == null || !reservations.Any())
                {
                    return NotFound("No reservations available.");
                }
                var reservationDtos = _mapper.Map<IEnumerable<ReservationDTO>>(reservations);
                return Ok(reservationDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetReservationById/{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid reservation ID.");
                }

                var reservation = await _reservationRepository.GetByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }
                var reservationDto = _mapper.Map<ReservationDTO>(reservation);
                return Ok(reservationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddReservationWithSeats")]
        public async Task<IActionResult> AddReservationWithSeats([FromBody] ReservationDTO reservationDto)
        {
            try
            {
                if (reservationDto == null || reservationDto.SeatIds == null || !reservationDto.SeatIds.Any())
                {
                    return BadRequest("Invalid reservation data.");
                }
                var addedReservation = await _ireservationRepository.AddReservationWithSeatsAsync(reservationDto);
                return Ok(new { message = "Reservation added successfully", reservationId = addedReservation.Id });
            }
            catch (JsonException jsonEx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "JSON Serialization Error", error = jsonEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPost("AddReservation")]
        public async Task<ActionResult> AddReservation(ReservationDTO reservationDto)
        {
            try
            {
                if (reservationDto == null)
                {
                    return BadRequest("Reservation data cannot be null.");
                }

                var reservation = _mapper.Map<Reservation>(reservationDto);
                await _reservationRepository.AddAsync(reservation);
                return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateReservationById/{id}")]
        public async Task<ActionResult> UpdateReservation(int id, ReservationDTO reservationDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid reservation ID.");
                }

                if (reservationDto == null)
                {
                    return BadRequest("Reservation data cannot be null.");
                }

                var existingReservation = await _reservationRepository.GetByIdAsync(id);
                if (existingReservation == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }

                _mapper.Map(reservationDto, existingReservation);
                await _reservationRepository.UpdateAsync(existingReservation);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteReservationById{id}")]
        public async Task<ActionResult> DeleteReservation(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid reservation ID.");
                }

                var reservation = await _reservationRepository.GetByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }
                await _reservationRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
