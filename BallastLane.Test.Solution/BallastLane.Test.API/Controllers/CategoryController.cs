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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<CategoryDTO>>
            {
                Success = true,
                Message = "OK",
                Data    = result
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Category with id {id} was not found.",
                    Data    = null!
                });

            return Ok(new ApiResponse<CategoryDTO>
            {
                Success = true,
                Message = "OK",
                Data    = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDTO dto)
        {
            var id = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<int>
            {
                Success = true,
                Message = "Category created successfully.",
                Data    = id
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDTO dto)
        {
            dto.Id = id;
            await _categoryService.UpdateAsync(dto);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Category updated successfully.",
                Data    = null!
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Category deleted successfully.",
                Data    = null!
            });
        }
    }
}
