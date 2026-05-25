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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<ProductDTO>>
            {
                Success = true,
                Message = "OK",
                Data    = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Product with id {id} was not found.",
                    Data    = null!
                });

            return Ok(new ApiResponse<ProductDTO>
            {
                Success = true,
                Message = "OK",
                Data    = result
            });
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var result = await _productService.GetByCategoryIdAsync(categoryId);
            return Ok(new ApiResponse<IEnumerable<ProductDTO>>
            {
                Success = true,
                Message = "OK",
                Data    = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDTO dto)
        {
            var id = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<int>
            {
                Success = true,
                Message = "Product created successfully.",
                Data    = id
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDTO dto)
        {
            dto.Id = id;
            await _productService.UpdateAsync(dto);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product updated successfully.",
                Data    = null!
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product deleted successfully.",
                Data    = null!
            });
        }
    }
}
