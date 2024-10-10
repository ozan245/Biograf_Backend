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
    public class UsersController : ControllerBase
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IGenericRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();

                if (users == null || !users.Any())
                {
                    return NotFound("No users available.");
                }
                var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }
                var userDto = _mapper.Map<UserDTO>(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddUser")]
        public async Task<ActionResult<UserDTO>> AddUser(UserDTO userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return BadRequest("User data cannot be null.");
                }

                var user = _mapper.Map<User>(userDto);
                user.Role = "User";
                await _userRepository.AddAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost ("AddAdmin")]
        public async Task<ActionResult<UserDTO>> AddAdmin(UserDTO userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return BadRequest("User data cannot be null.");
                }

                var user = _mapper.Map<User>(userDto);
                user.Role = "Admin";
                await _userRepository.AddAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateUserById/{id}")]
        public async Task<ActionResult> UpdateUser(int id, UserDTO userDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                if (userDto == null)
                {
                    return BadRequest("User data cannot be null.");
                }

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }
                _mapper.Map(userDto, existingUser);
                await _userRepository.UpdateAsync(existingUser);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteUserById/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }
                await _userRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
