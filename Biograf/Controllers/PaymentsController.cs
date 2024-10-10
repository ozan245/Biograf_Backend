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
    public class PaymentsController : ControllerBase
    {
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IPaymentRepository _ipaymentRepository;
        private readonly IMapper _mapper;

        public PaymentsController(IGenericRepository<Payment> paymentRepository, IMapper mapper, IPaymentRepository ipaymentRepository)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _ipaymentRepository = ipaymentRepository;
        }

        [HttpGet("GetAllPayments")]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetPayments()
        {
            try
            {
                var payments = await _paymentRepository.GetAllAsync();

                if (payments == null || !payments.Any())
                {
                    return NotFound("No payments available.");
                }
                var paymentDtos = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
                return Ok(paymentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetPaymentById/{id}")]
        public async Task<ActionResult<PaymentDTO>> GetPayment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid payment ID.");
                }

                var payment = await _paymentRepository.GetByIdAsync(id);
                if (payment == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }
                var paymentDto = _mapper.Map<PaymentDTO>(payment);
                return Ok(paymentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddPaymentWithTickets")]
        public async Task<IActionResult> AddPaymentWithTickets([FromBody] PaymentDTO paymentDto)
        {
            try
            {
                if (paymentDto == null)
                {
                    return BadRequest("Payment data cannot be null.");
                }

                var payment = await _ipaymentRepository.AddPaymentAsync(paymentDto);
                return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
            }           
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("(\"GetPaymentRepo\"){id}")]
        public async Task<IActionResult> GetPaymentrepo(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid payment ID.");
                }
                var payment = await _ipaymentRepository.GetPaymentByIdAsync(id);

                if (payment == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("AddPayment")]
        public async Task<ActionResult> AddPayment(PaymentDTO paymentDto)
        {
            try
            {
                if (paymentDto == null)
                {
                    return BadRequest("Payment data cannot be null.");
                }
                var payment = _mapper.Map<Payment>(paymentDto);
                await _paymentRepository.AddAsync(payment);
                return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, paymentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdatePaymentById/{id}")]
        public async Task<ActionResult> UpdatePayment(int id, PaymentDTO paymentDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid payment ID.");
                }

                if (paymentDto == null)
                {
                    return BadRequest("Payment data cannot be null.");
                }
                var existingPayment = await _paymentRepository.GetByIdAsync(id);

                if (existingPayment == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }
                _mapper.Map(paymentDto, existingPayment);
                await _paymentRepository.UpdateAsync(existingPayment);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeletePaymentById/{id}")]
        public async Task<ActionResult> DeletePayment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid payment ID.");
                }
                var payment = await _paymentRepository.GetByIdAsync(id);
                if (payment == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }
                await _paymentRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
