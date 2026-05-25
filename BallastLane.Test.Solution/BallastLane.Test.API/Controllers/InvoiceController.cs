using BallastLane.Test.Application.Common;
using BallastLane.Test.Application.DTOs;
using BallastLane.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BallastLane.Test.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _invoiceService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<InvoiceDTO>>
            {
                Success = true,
                Message = "OK",
                Data    = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _invoiceService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Invoice with id {id} was not found.",
                    Data    = null!
                });

            return Ok(new ApiResponse<InvoiceDTO>
            {
                Success = true,
                Message = "OK",
                Data    = result
            });
        }

        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var result = await _invoiceService.GetByCustomerIdAsync(customerId);
            return Ok(new ApiResponse<IEnumerable<InvoiceDTO>>
            {
                Success = true,
                Message = "OK",
                Data    = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceDTO dto)
        {
            dto.CreatedByUserId = GetCurrentUserId();
            var id = await _invoiceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<int>
            {
                Success = true,
                Message = "Invoice created successfully.",
                Data    = id
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _invoiceService.DeleteAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Invoice deleted successfully.",
                Data    = null!
            });
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
                     ?? User.FindFirst(ClaimTypes.NameIdentifier);

            return claim is not null && int.TryParse(claim.Value, out var id) ? id : 0;
        }
    }
}
