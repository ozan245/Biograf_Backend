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
    public class TicketsController : ControllerBase
    {
        private readonly IGenericRepository<Ticket> _ticketRepository;
        private readonly ITicketRepository _iticketRepository;
        private readonly IMapper _mapper;

        public TicketsController(IGenericRepository<Ticket> ticketRepository, IMapper mapper, ITicketRepository iticketRepository)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
            _iticketRepository = iticketRepository;
        }

        [HttpGet("GetAllTickets")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTickets()
        {
            try
            {
                var tickets = await _ticketRepository.GetAllAsync();

                if (tickets == null || !tickets.Any())
                {
                    return NotFound("No tickets available.");
                }
                var ticketDtos = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
                return Ok(ticketDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetTicketById/{id}")]
        public async Task<ActionResult<TicketDTO>> GetTicket(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid ticket ID.");
                }

                var ticket = await _ticketRepository.GetByIdAsync(id);
                if (ticket == null)
                {
                    return NotFound($"Ticket with ID {id} not found.");
                }
                var ticketDto = _mapper.Map<TicketDTO>(ticket);
                return Ok(ticketDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetTicketDetailsByPaymentId/{paymentId}")]
        public async Task<IActionResult> GetTicketDetails(int paymentId)
        {
            try
            {
                if (paymentId <= 0)
                {
                    return BadRequest("Invalid payment ID.");
                }

                var ticketDetails = await _iticketRepository.GetTicketDetailsByPaymentIdAsync(paymentId);

                if (ticketDetails == null)
                {
                    return NotFound($"Ticket details for payment ID {paymentId} not found.");
                }
                return Ok(ticketDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("(\"GetTicketRepo\"){id}")]
        public async Task<IActionResult> GetTicketRepo(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid ticket ID.");
                }

                var ticket = await _iticketRepository.GetTicketByIdAsync(id);

                if (ticket == null)
                {
                    return NotFound($"Ticket with ID {id} not found.");
                }
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetUserTickets/{userId}")]
        public async Task<IActionResult> GetUserTickets(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var tickets = await _iticketRepository.GetTicketsByUserAsync(userId);

                if (tickets == null || !tickets.Any())
                {
                    return NotFound($"No tickets found for user with ID {userId}.");
                }
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddTicket")]
        public async Task<ActionResult> AddTicket(TicketDTO ticketDto)
        {
            try
            {
                if (ticketDto == null)
                {
                    return BadRequest("Ticket data cannot be null.");
                }

                var ticket = _mapper.Map<Ticket>(ticketDto);
                await _ticketRepository.AddAsync(ticket);
                return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticketDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddTicketsRepo")]
        public async Task<IActionResult> AddTickets([FromBody] TicketDTO ticketDto)
        {
            try
            {
                if (ticketDto == null || !ticketDto.SeatIds.Any())
                {
                    return BadRequest("Invalid ticket data.");
                }

                await _iticketRepository.AddTicketsAsync(ticketDto);
                return Ok(new { message = "Tickets created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateTicketById/{id}")]
        public async Task<ActionResult> UpdateTicket(int id, TicketDTO ticketDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid ticket ID.");
                }

                if (ticketDto == null)
                {
                    return BadRequest("Ticket data cannot be null.");
                }

                var existingTicket = await _ticketRepository.GetByIdAsync(id);
                if (existingTicket == null)
                {
                    return NotFound($"Ticket with ID {id} not found.");
                }
                _mapper.Map(ticketDto, existingTicket);
                await _ticketRepository.UpdateAsync(existingTicket);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteTicketById/{id}")]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid ticket ID.");
                }

                var ticket = await _ticketRepository.GetByIdAsync(id);
                if (ticket == null)
                {
                    return NotFound($"Ticket with ID {id} not found.");
                }
                await _ticketRepository.DeleteAsync(id);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
