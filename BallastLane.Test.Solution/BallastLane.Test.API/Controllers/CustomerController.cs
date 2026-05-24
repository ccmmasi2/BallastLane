using BallastLane.Test.Application.Common;
using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.Test.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _customerService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<CustomerDTO>>
            {
                Success = true,
                Message = "OK",
                Data    = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Customer with id {id} was not found.",
                    Data    = null!
                });

            return Ok(new ApiResponse<CustomerDTO>
            {
                Success = true,
                Message = "OK",
                Data    = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerDTO dto)
        {
            var id = await _customerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<int>
            {
                Success = true,
                Message = "Customer created successfully.",
                Data    = id
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerDTO dto)
        {
            dto.Id = id;
            await _customerService.UpdateAsync(dto);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Customer updated successfully.",
                Data    = null!
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.DeleteAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Customer deleted successfully.",
                Data    = null!
            });
        }
    }
}
